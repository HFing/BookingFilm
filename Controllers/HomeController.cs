using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookingFilm.Controllers
{
    public class HomeController : Controller
    {
		private readonly BookingFilmTicketsEntities1 _context;
		public HomeController()
		{
			_context = new BookingFilmTicketsEntities1();
		}
		// GET: Home
		public ActionResult Index()
		{
			var phimList = _context.Phims.ToList();

			var user = Session["User"] as KhachHang; // Lấy user từ Session
			ViewBag.User = user;
			return View(phimList);
		}


		public ActionResult Logout()
		{
			Session["User"] = null; // Xóa người dùng khỏi phiên
			return RedirectToAction("Index", "Home");
		}

	}
}