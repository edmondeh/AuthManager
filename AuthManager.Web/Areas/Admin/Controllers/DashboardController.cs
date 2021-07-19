using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AuthManager.Web.Areas.Admin.Controllers
{
    //[RoutePrefix("Admin")]
    [Area("Admin")]
    //[Route("/Admin")]
    //[Route("/Admin/[controller]")]
    public class DashboardController : Controller
    {
        // GET: DashboardController
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
