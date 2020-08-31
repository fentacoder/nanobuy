using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nano_buy.View_Models
{
    public class ViewProduct
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public decimal Price { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
