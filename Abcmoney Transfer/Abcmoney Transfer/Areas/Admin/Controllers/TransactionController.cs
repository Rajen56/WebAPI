using Microsoft.AspNetCore.Mvc;

namespace Abcmoney_Transfer.Areas.Admin.Controllers
{
    public class TransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
