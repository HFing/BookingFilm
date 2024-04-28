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
			var user = Session["User"] as KhachHang; // Lấy user từ Session
			return View(user);
		}

		public ActionResult GetMovies()
		{
			var cards = _context.Phims.Select(c => new
			{
				background = c.HinhPhim,
				display_background = c.HinhPhim,
				title = c.TenPhim,
				description = c.MoTa
			}).ToList();

			return Json(cards, JsonRequestBehavior.AllowGet);
		}
		public ActionResult Logout()
		{
			Session["User"] = null; // Xóa người dùng khỏi phiên
			return RedirectToAction("Index", "Home");
		}
	}
}