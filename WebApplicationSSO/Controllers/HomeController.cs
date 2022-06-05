using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationSSO.Models;

namespace WebApplicationSSO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string SecureCookieName = "ajhsdfjhu";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            Request.Cookies.TryGetValue(SecureCookieName, out string username);
            ViewBag.Username = username==null ? null :  Base64Decode(username);
            return View();
        }

        public IActionResult Login(string returnURL)
        {
            Trace.WriteLine("return URL : " + returnURL);
            ViewBag.ReturnURL = returnURL;
            if(returnURL == null)
            {
                if(Request.Cookies.TryGetValue(SecureCookieName, out string username))
                {
                    if(username != null)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                if (Request.Cookies.TryGetValue(SecureCookieName, out string username))
                {
                    if (username != null)
                    {
                        Response.Headers.Add("Access-Control-Allow-Origin", returnURL);
                        Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                        Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST");
                        Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, *");
                        Response.Cookies.Append(SecureCookieName, Base64Encode(username), new Microsoft.AspNetCore.Http.CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(5)

                        });
                        return RedirectToAction(returnURL);
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login login)
        {
            if(login.RedirectURL == null)
            {
                Response.Cookies.Append(SecureCookieName, Base64Encode(login.Username), new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(5)

                });
                return RedirectToAction("Index", "Home");
            }
            Response.Headers.Add("Access-Control-Allow-Origin", login.RedirectURL);
            Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, *");
            Response.Cookies.Append(SecureCookieName, Base64Encode(login.Username) , new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(5)

            });
            return Redirect(login.RedirectURL);
            
        }
        public IActionResult LogOut()
        {
            Response.Cookies.Delete(SecureCookieName);
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
