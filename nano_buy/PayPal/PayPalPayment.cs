using nano_buy.Models.Products;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoShop.PayPal
{
    public class PayPalPayment : IPayPalPayment
    {
        private Payment payment = null;
        public Payment CreatePayment(APIContext apiContext, string redirectURL, ProductOrder input)
        {
            try
            {
                var itemList = new ItemList() { items = new List<Item>() };
                var itemsList = new List<ProductOrder>();
                itemsList.Add(input);
                int productQuantity = 1;
                foreach (var item in itemsList)
                {
                    itemList.items.Add(new Item()
                    {
                        name = item.Name,
                        currency = "USD",
                        price = (item.Price / productQuantity).ToString(),
                        quantity = productQuantity.ToString(),
                        sku = "sku"
                    });
                }

                var payer = new Payer() { payment_method = "paypal" };
                var redirectURLs = new RedirectUrls()
                {
                    cancel_url = redirectURL,
                    return_url = redirectURL
                };
                var totalBill = itemsList.Sum(x => x.Price);

                var details = new Details()
                {
                    tax = "0",
                    shipping = "0",
                    subtotal = totalBill.ToString()
                };

                var amount = new Amount()
                {
                    currency = "USD",
                    total = Convert.ToString(Convert.ToDecimal(details.tax) + Convert.ToDecimal(details.shipping) + Convert.ToDecimal(details.subtotal)),
                    details = details
                };

                var transactionList = new List<Transaction>();
                transactionList.Add(new Transaction()
                {
                    description = "PizzaBites Transaction",
                    invoice_number = Convert.ToString(new Random().Next(999999)),
                    amount = amount,
                    item_list = itemList
                });
                payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirectURLs
                };
                return payment.Create(apiContext);
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            return null;
        }

        public Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }
    }
}
