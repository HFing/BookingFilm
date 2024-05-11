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
		public ActionResult Index(string searchTerm)
		{
			var now = DateTime.Now;
			var future = now.AddDays(7);

			// Lấy danh sách các lịch chiếu từ thời điểm hiện tại đến 7 ngày tới
			var upcomingLichChieuList = _context.LichChieux
				.Where(lc => lc.NgayChieu >= now && lc.NgayChieu <= future)
				.ToList();

			// Lấy danh sách các phim có lịch chiếu từ thời điểm hiện tại đến 7 ngày tới
			var upcomingPhimList = upcomingLichChieuList.Select(lc => lc.Phim).Distinct().ToList();

			// Lấy danh sách các lịch chiếu sau 7 ngày từ thời điểm hiện tại
			var futureLichChieuList = _context.LichChieux
				.Where(lc => lc.NgayChieu > future)
				.ToList();

			// Lấy danh sách các phim sẽ chiếu sau 7 ngày từ thời điểm hiện tại
			var futurePhimList = futureLichChieuList.Select(lc => lc.Phim).Distinct().ToList();

			// Lấy danh sách các phim chưa có lịch chiếu
			var noSchedulePhimList = _context.Phims
				.Where(p => !_context.LichChieux.Any(lc => lc.MaPhim == p.MaPhim))
				.ToList();

            // Lấy danh sách tất cả các phim
            var allPhimList = _context.Phims.ToList();
			List<Phim> searchPhim;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchPhim = allPhimList.Where(p => p.TenPhim.Contains(searchTerm)).ToList();
                ViewBag.SearchPhim = searchPhim;
				ViewBag.SearchTerm = searchTerm;
            }

            var user = Session["User"] as KhachHang; // Lấy user từ Session
			ViewBag.User = user;
			ViewBag.UpcomingPhimList = upcomingPhimList;
			ViewBag.FuturePhimList = futurePhimList;
			ViewBag.NoSchedulePhimList = noSchedulePhimList;
			ViewBag.AllPhimList = allPhimList;
			
			return View();
		}


		public ActionResult Logout()
		{
			Session["User"] = null; // Xóa người dùng khỏi phiên
			return RedirectToAction("Index", "Home");
		}

	}
}