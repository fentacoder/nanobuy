using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nano_buy.Models.Products
{
    public class ProductModel
    {
        public string Base64Image { get; set; }
        public string Base64ImageContentType { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
