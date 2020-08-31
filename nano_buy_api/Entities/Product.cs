using System;
using System.Collections.Generic;
using System.Text;

namespace nano_buy_api.Entities
{
    public class Product
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public decimal Price { get; set; }
    }
}
