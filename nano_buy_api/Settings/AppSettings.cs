using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoShop.Settings
{
    public class AppSettings
    {
        public string Host { get; set; }
        public int TenantId { get; set; }
        public string WorldPay_ServiceKey{ get; set; }
        public string WorldPay_ClientKey { get; set; }
    }
}
