using System.Web.Mvc;

namespace CWS
{
    public class XssController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}