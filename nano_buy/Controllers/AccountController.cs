using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using NanoShop.Core.Common;
using NanoShop.Core.Entities;
using NanoShop.Web.VM.Account;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NanoShop.Web.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private UserManager<ApplicationUser> UserManager { get; }
        private SignInManager<ApplicationUser> SignInManager { get; }

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Errors", string.Join(",", ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToList()));
                return View();
            }
            string errors = string.Empty;
            var user = await UserManager.FindByEmailAsync(input.Email);
            if (user != null)
            {
                ViewBag.Message = new List<string>() { $"Email '{input.Email}' already taken, try a different one!" };
                return View();
            }

            var newUser = new ApplicationUser()
            {
                FirstName = input.UserName,
                LastName = string.Empty,
                Email = input.Email,
                UserName = input.UserName,
                IsActive = true,
                EmailConfirmed = true
            };
            var result = await UserManager.CreateAsync(newUser, input.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                errors = string.Join(",", result.Errors.Select(x => x.Description).ToList());
                ViewBag.Message = errors;
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await UserManager.FindByIdAsync(userId);
                await SignInManager.SignOutAsync();
                await UserManager.DeleteAsync(user);
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Message = "Could not found user account";
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM input, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Errors", string.Join(",", ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage)).ToList()));
                return View();
            }
            string errors = string.Empty;
            var user = await UserManager.FindByEmailAsync(input.Email);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    errors = "Email is not confirmed";
                    ViewBag.Message = errors;
                    return View();
                }
                else if (!user.IsActive)
                {
                    errors = "Account is not active";
                    ViewBag.Message = errors;
                    return View();
                }

                var isPasswordCorrect = await UserManager.CheckPasswordAsync(user, input.Password);
                if (!isPasswordCorrect)
                {
                    errors = "Email/Password is not correct";
                    ViewBag.Message = errors;
                    return View();
                }
                await SignInManager.SignOutAsync();
                await SignInManager.SignInAsync(user, input.RememberMe);
                return RedirectUser(returnUrl);
            }
            else
                errors = "Do you have account?, if not please register";
            ViewBag.Message = errors;
            return View();
        }

        [AllowAnonymous]
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
                return Json(true);
            else
                return Json($"Email {email} already taken, try a different one!");
        }

        private IActionResult RedirectUser(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
    }
}