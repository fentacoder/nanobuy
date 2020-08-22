using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NanoShop.Core.Repositories.Products;
using NanoShop.PayPal;
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

namespace NanoShop.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private readonly IAuthorizationService _authorizationService;
        private readonly IProductRepository _productRepository;
        private readonly IPayPalPayment _payPalPayment;

        public HomeController(Microsoft.Extensions.Configuration.IConfiguration configuration, IAuthorizationService authorizationService, IProductRepository productRepository,
            IPayPalPayment payPalPayment)
        {
            _configuration = configuration;
            _authorizationService = authorizationService;
            _productRepository = productRepository;
            _payPalPayment = payPalPayment;
        }
        public IActionResult Index()
        {
            var productList = _productRepository.GetAll().OrderByDescending(x => x.CreatedDateTime).Skip(0).Take(10).ToList();
            return View(productList);
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
                var apiContext = GetAPIContext();
                var payerId = Request.Query["payerId"];
                if (string.IsNullOrWhiteSpace(payerId))
                {
                    var baseUrl = new Uri(Request.GetDisplayUrl()).Scheme + "://" + new Uri(Request.GetDisplayUrl()).Authority + "/Home/PaymentWithPayPal?";
                    var guid = Convert.ToString(new Random().Next(1000)) + DateTime.Now.Ticks;
                    var orderDetails = JsonConvert.DeserializeObject<ProductOrder>(HttpContext.Session.GetString("ProductDetails"));
                    var createPayment = _payPalPayment.CreatePayment(apiContext, baseUrl + "guid=" + guid, orderDetails);
                    if (createPayment != null)
                    {
                        var approvalLink = createPayment.links.FirstOrDefault(x => x.rel.ToLower().Trim() == "approval_url");
                        var paypalRedirectUrl = string.Empty;
                        if (approvalLink != null)
                            paypalRedirectUrl = approvalLink.href;

                        HttpContext.Session.SetString(guid, createPayment.id);
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
                    var guid = Request.Query["guid"];
                    var executePayment = _payPalPayment.ExecutePayment(apiContext, payerId, HttpContext.Session.GetString(guid));
                    if (executePayment.state.ToLower() != "approved")
                    {
                        paymentResponseModel.Message = "Payment Fail";
                        return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                    }
                    else
                    {
                        HttpContext.Session.SetString(guid, string.Empty);
                        HttpContext.Session.SetString("ProductDetails", string.Empty);
                        paymentResponseModel.Message = "Payment Successful";

                        return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                    }
                }
            }
            catch (Exception e)
            {
                paymentResponseModel.Message = "Error occured while processing payment";
                //PayPalLogger.Log("Exception: " + e.Message);
            }
            return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
        }

        public string GetAccessToken()
        {
            var clientId = _configuration.GetSection("AppSettings:PayPal:ClientId").Value;
            var secretId = _configuration.GetSection("AppSettings:PayPal:SecretId").Value;
            var accessTokend = new OAuthTokenCredential(clientId, secretId, GetConfig());
            var asas = accessTokend.GetAccessToken();
            return asas;
        }

        public APIContext GetAPIContext()
        {

            var apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }

        public Dictionary<string, string> GetConfig()
        {
            var config = new Dictionary<string, string>();
            config.Add("mode", _configuration.GetSection("AppSettings:PayPal:Mode").Value);
            config.Add("connectionTimeout", _configuration.GetSection("AppSettings:PayPal:ConnectionTimeout").Value);
            config.Add("requestRetries", _configuration.GetSection("AppSettings:PayPal:RequestRetries").Value);
            config.Add("clientId", _configuration.GetSection("AppSettings:PayPal:ClientId").Value);
            config.Add("secretId", _configuration.GetSection("AppSettings:PayPal:SecretId").Value);
            return config;
        }


        //stripe
        public IActionResult StripePayment(string stripeEmail, string stripeToken)
        {
            var paymentResponseModel = new PaymentResponseModel();
            try
            {
                var customers = new CustomerService();
                var charges = new ChargeService();
                var customer = customers.Create(new CustomerCreateOptions
                {
                    Email = stripeEmail,
                    Source = stripeToken
                });
                var orderDetails = JsonConvert.DeserializeObject<ProductOrder>(HttpContext.Session.GetString("ProductDetails"));
                var charge = charges.Create(new ChargeCreateOptions
                {
                    Amount = Convert.ToInt64(orderDetails.Price*100),
                    Description = "Stripe Payment",
                    Currency = "usd",
                    Customer = customer.Id,
                    ReceiptEmail = stripeEmail,
                    Metadata = new Dictionary<string, string>() {
                {"Product Name", orderDetails.Name},
                {"Price",orderDetails.Price.ToString()}
            }
                });
                if (charge.Status == "succeeded")
                {
                    HttpContext.Session.SetString("ProductDetails", string.Empty);
                    string balanceTransactionId = charge.BalanceTransactionId;
                    paymentResponseModel.Message = "Payment Successful";
                    return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                }
                else
                {
                    paymentResponseModel.Message = "Payment Fail";
                    return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
                }
            }
            catch (Exception e)
            {
                paymentResponseModel.Message = "Error occured while processing payment";
                RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
            }

            return RedirectToAction("PaymentStatus", "Home", paymentResponseModel);
        }

    }
}
