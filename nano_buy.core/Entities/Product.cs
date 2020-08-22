using NanoShop.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NanoShop.Core.Entities
{
    public class Product : AuditableEntity
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public decimal Price { get; set; }
    }
}
