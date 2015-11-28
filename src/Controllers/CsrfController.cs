using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.IO;

namespace CWS.Controllers
{
    public class CsrfController : Controller
    {
        private const string AuthCookieName = "AuthCookie";
        private const string Level1CompletedCookieName = "Level1Completed";
        public const string Level1CompletedKey = "5D916DDD-AA6D-4E0C-BED8-896F7C936FE4";
        private const string Level2CompletedCookieName = "Level2Completed";
        public const string Level2CompletedKey = "377d56d3-0d48-46d5-8658-e779f841bf23";
        private JavaScriptSerializer javaScriptSerializer;

        public CsrfController()
        {
            javaScriptSerializer = new JavaScriptSerializer();
        }

        [HttpGet]
        public ActionResult Level1()
        {
            SetAuthCookie();
            return View();
        }

        [HttpPost]
        [ActionName("Level1")]
        public ActionResult Level1Post()
        {
            if (!CheckAuthCookie()) {
                return View("Level1AuthFailed");
            }

            if (Request.Form.Get("hacker") != "true") {
                return View("Level1WrongParameter");
            }

            if (!IsCsrf()) {
                return View("Level1NotCsrf");
            }

            var cookie = new HttpCookie(Level1CompletedCookieName, Level1CompletedKey) {
                HttpOnly = true,
                Path = Url.Action("Level2"),
                Expires = DateTime.UtcNow.AddMonths(1)
            };
            Response.SetCookie(cookie);
            return View("Level1Success");
        }

        private bool CheckAuthCookie()
        {
            var authCookie = Request.Cookies.Get(AuthCookieName);
            if (authCookie != null && authCookie.Value != null)
                return true;
            return false;
        }

        private bool IsCsrf()
        {
            return Request.UrlReferrer == null || Request.UrlReferrer != Request.Url;
        }

        private void SetAuthCookie()
        {
            var cookie = new HttpCookie(AuthCookieName, Guid.NewGuid().ToString()) {
                HttpOnly = true,
                Path = Request.Url.AbsolutePath,
                Expires = DateTime.UtcNow.AddMonths(1)
            };
            Response.SetCookie(cookie);
        }

        [HttpGet]
        public ActionResult Level2()
        {
            var level1CompletedCookie = Request.Cookies.Get(Level1CompletedCookieName);
            if (level1CompletedCookie == null || level1CompletedCookie.Value != Level1CompletedKey) {
                return View("BackToLevel1");
            }

            SetAuthCookie();
            return View();
        }

        [HttpPost]
        [ActionName("Level2")]
        public ActionResult Level2Post()
        {
            if (!CheckAuthCookie()) {
                return View("Level2AuthFailed");
            }

            Level2Data data = null;
            using (var inputStream = new StreamReader(Request.InputStream)) {
                try {
                    data = javaScriptSerializer.Deserialize<Level2Data>(inputStream.ReadToEnd());
                } catch (Exception) {
                    return View("Level2BadJson");
                }
            }

            if (data == null || data.hacker != "true") {
                return View("Level2WrongParameter");
            }

            if (!IsCsrf()) {
                return View("Level2NotCsrf");
            }

            var cookie = new HttpCookie(Level2CompletedCookieName, Level2CompletedKey) {
                HttpOnly = true,
                Path = Url.Action("Level2"),
                Expires = DateTime.UtcNow.AddMonths(1)
            };
            Response.SetCookie(cookie);
            return View("Level2Success");
        }

        public class Level2Data
        {
            public string hacker { get; set; }
        }
    }
}
