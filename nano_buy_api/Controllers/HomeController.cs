using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using nano_buy_api.Entities;
using nano_buy_api.Models;
using nano_buy_api.Models.Products;
using NanoShop.PayPal;
using Newtonsoft.Json;
using System.Text.Json;
using PayPal.Api;
using Stripe;
using Product = nano_buy_api.Entities.Product;

namespace nano_buy_api.Controllers
{
    [Route("api/v1/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private DbSet<Product> _productContext;
        private IConfiguration _configuration;
        private readonly IPayPalPayment _payPalPayment;
        public HomeController(DbSet<Product> productContext,IConfiguration configuration, IPayPalPayment payPalPayment)
        {
            _productContext = productContext;
            _configuration = configuration;
            _payPalPayment = payPalPayment;
        }

        [HttpGet]
        [Route("getallproducts")]
        public IEnumerable<Product> getAllProducts()
        {
            //var productList = _productContext.sel.GetAll().OrderByDescending(x => x.CreatedDateTime).Skip(0).Take(10).ToList();
            return _productContext;
        }

        //paypal
        [HttpPost]
        [Route("paypal/pay")]
        public async Task<String> PaymentWithPayPal([FromBody] Dictionary<String,String> tempDict)
        {
            var paymentResponseModel = new PaymentResponseModel();
            var paymentInfoStr = tempDict["paymentInfo"];
            var orderDetailsStr = tempDict["orderDetails"];
            ProductOrder orderDetails = System.Text.Json.JsonSerializer.Deserialize<ProductOrder>(orderDetailsStr);
            try
            {
                var apiContext = GetAPIContext();
                if (string.IsNullOrWhiteSpace(paymentInfoStr))
                {
                    Payment createPayment = _payPalPayment.CreatePayment(apiContext, paymentInfoStr, orderDetails);
                    if (createPayment != null)
                    {
                        var approvalLink = createPayment.links.FirstOrDefault(x => x.rel.ToLower().Trim() == "approval_url");
                        var paypalRedirectUrl = string.Empty;
                        if (approvalLink != null)
                            paypalRedirectUrl = approvalLink.href;

                        return paypalRedirectUrl;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                paymentResponseModel.Message = "Error occured while processing payment";
                //PayPalLogger.Log("Exception: " + e.Message);
                return "";
            }
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
        [HttpPost]
        [Route("stripe/pay")]
        public String StripePayment([FromBody] Dictionary<String,String> tempDict)
        {
            var stripeEmail = tempDict["stripeEmail"];
            var stripeToken = tempDict["stripeToken"];
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
                    Amount = Convert.ToInt64(orderDetails.Price * 100),
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
                    return "Payment Successful";
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
