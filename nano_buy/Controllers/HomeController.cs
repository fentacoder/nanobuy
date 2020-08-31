using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using nano_buy.Models;
using nano_buy.Models.Products;
using Newtonsoft.Json;
using PayPal.Api;
using Stripe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using nano_buy.Utils;
using nano_buy_api.Entities;
using nano_buy.View_Models;

namespace NanoShop.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;
        private HttpClass _httpClass;

        public HomeController(Microsoft.Extensions.Configuration.IConfiguration configuration, IAuthorizationService authorizationService,
            HttpClass httpClass)
        {
            _configuration = configuration;
            _authorizationService = authorizationService;
            _httpClass = httpClass;
        }
        public async Task<IActionResult> Index()
        {
            using (HttpResponseMessage response = await _httpClass.ApiClient.GetAsync("api/v1/home/getallproducts"))
            {
                if (response.IsSuccessStatusCode)
                {
                    string tempStr = await response.Content.ReadAsStringAsync();
                    IEnumerable<ViewProduct> productList = JsonConvert.DeserializeObject<List<ViewProduct>>(tempStr);
                    productList = productList.OrderByDescending(x => x.CreatedDateTime).Skip(0).Take(10).ToList();
                    return View(productList);
                }
                else
                {
                    Console.WriteLine(response.ReasonPhrase);
                    return View();
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult PaymentStatus(PaymentResponseModel input)
        {
            ViewBag.Message = input.Message;
            return View();
        }
        [HttpPost]
        public JsonResult SaveProductOrder([FromBody]ProductOrder input)
        {
            HttpContext.Session.SetString("ProductDetails", string.Empty);
            HttpContext.Session.SetString("ProductDetails", JsonConvert.SerializeObject(input));
            return new JsonResult("Product Save");
        }

        //paypal
        public async Task<ActionResult> PaymentWithPayPal()
        {
            var paymentResponseModel = new PaymentResponseModel();
            try
            {
                var payerId = Request.Query["payerId"];
                if (string.IsNullOrWhiteSpace(payerId))
                {
                    var baseUrl = new Uri(Request.GetDisplayUrl()).Scheme + "://" + new Uri(Request.GetDisplayUrl()).Authority + "/Home/PaymentWithPayPal?";
                    var guid = Convert.ToString(new Random().Next(1000)) + DateTime.Now.Ticks;
                    var orderDetails = JsonConvert.DeserializeObject<ProductOrder>(HttpContext.Session.GetString("ProductDetails"));

                    Dictionary<String, String> tempDict = new Dictionary<string, string>
                    {
                        {"paymentInfo", baseUrl + "guid=" + guid},
                        {"orderDetails",orderDetails.ToString()}
                    };

                    var paymentJson = System.Text.Json.JsonSerializer.Serialize(orderDetails);

                    using (HttpResponseMessage response = await _httpClass.ApiClient.PostAsJsonAsync("api/v1/paypal/pay", paymentJson))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string paypalRedirectUrl = await response.Content.ReadAsStringAsync();
                            if(paypalRedirectUrl.Length > 0)
                            {
                                return Redirect(paypalRedirectUrl);
                            }
                            else
                            {
                                paymentResponseModel.Message = "Error occured while creating payment";
                                return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                            }
                        }
                        else
                        {
                            Console.WriteLine(response.ReasonPhrase);
                            paymentResponseModel.Message = "Error occured while creating payment";
                            return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                        }
                    }
                }
                else
                {
                    paymentResponseModel.Message = "Error occured while creating payment";
                    return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                }
            }
            catch (Exception e)
            {
                paymentResponseModel.Message = "Error occured while processing payment";
                //PayPalLogger.Log("Exception: " + e.Message);
            }
            return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
        }


        //stripe
        public async Task<IActionResult> StripePayment(string stripeEmail, string stripeToken)
        {
            var paymentResponseModel = new PaymentResponseModel();
            try
            {
                Dictionary<String, String> tempDict = new Dictionary<string, string>
                {
                    {"stripeEmail",stripeEmail },
                    {"stripeToken",stripeToken }
                };
                var stripeJson = System.Text.Json.JsonSerializer.Serialize(tempDict);
    
                using (HttpResponseMessage response = await _httpClass.ApiClient.PostAsJsonAsync("api/v1/stripe/pay", stripeJson))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string stripeResponse = await response.Content.ReadAsStringAsync();
                        if (stripeResponse.Length > 0)
                        {
                            paymentResponseModel.Message = "Payment Successful";
                            return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                        }
                        else
                        {
                            paymentResponseModel.Message = "Payment Fail";
                            return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                        }
                    }
                    else
                    {
                        Console.WriteLine(response.ReasonPhrase);
                        paymentResponseModel.Message = "Payment Fail";
                        return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                    }
                }
            }
            catch (Exception e)
            {
                paymentResponseModel.Message = "Payment Fail";
                return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
            }
        }

    }
}
