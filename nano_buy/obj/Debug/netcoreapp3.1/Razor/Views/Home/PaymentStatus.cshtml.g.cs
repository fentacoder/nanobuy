#pragma checksum "C:\Users\jamar\Documents\Visual Studio Projects\nanobuy\nano_buy\Views\Home\PaymentStatus.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "efd9711d4baa5296510d09bfe64c2e759c767910"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_PaymentStatus), @"mvc.1.0.view", @"/Views/Home/PaymentStatus.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\jamar\Documents\Visual Studio Projects\nanobuy\nano_buy\Views\_ViewImports.cshtml"
using TicketManagement.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\jamar\Documents\Visual Studio Projects\nanobuy\nano_buy\Views\_ViewImports.cshtml"
using TicketManagement.Web.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\jamar\Documents\Visual Studio Projects\nanobuy\nano_buy\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\jamar\Documents\Visual Studio Projects\nanobuy\nano_buy\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"efd9711d4baa5296510d09bfe64c2e759c767910", @"/Views/Home/PaymentStatus.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"ccdef610293fbe9287ca6350e106f446a3d7c291", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_PaymentStatus : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 2 "C:\Users\jamar\Documents\Visual Studio Projects\nanobuy\nano_buy\Views\Home\PaymentStatus.cshtml"
  
    ViewData["Title"] = "Payment Status";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>Payment Status</h1>\r\n<div class=\"row\">\r\n    <div class=\"col-sm-4\"></div>\r\n    <div class=\"col-sm-4\">\r\n        <p>");
#nullable restore
#line 10 "C:\Users\jamar\Documents\Visual Studio Projects\nanobuy\nano_buy\Views\Home\PaymentStatus.cshtml"
      Write(ViewBag.Message);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p><br /><br /><br />\r\n        <a href=\"/\">Redirect To Home</a>\r\n    </div>\r\n    <div class=\"col-sm-4\"></div>\r\n</div>\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IAuthorizationService authorizationService { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
