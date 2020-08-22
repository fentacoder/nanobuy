using nano_buy.Models.Products;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoShop.PayPal
{
    public interface IPayPalPayment
    {
        Payment CreatePayment(APIContext apiContext, string redirectURL, ProductOrder input);
        Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId);
    }
}
