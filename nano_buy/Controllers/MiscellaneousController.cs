using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NanoShop.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class MiscellaneousController : Controller
    {
        [Route("{statusCode}")]
        public IActionResult NotFound(int statusCode)
        {
            var message = string.Empty;
            switch (statusCode)
            {
                case 404:
                    message = "This page not found";
                    break;
                default:
                    message = "This page not exists";
                    break;
            }
            ViewBag.Message = message;
            return View();
        }

        public IActionResult Exception()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}