using System.Web.Mvc;

namespace CWS.Controllers
{
    public class XssDefenceController : Controller
    {
        [HttpGet]
        public ActionResult Level1()
        {
            return View();
        }
    }
}