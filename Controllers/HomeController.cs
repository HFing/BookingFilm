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
		public ActionResult Index(string searchTerm = null)
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

            if (!string.IsNullOrEmpty(searchTerm))
            {
                upcomingPhimList = upcomingPhimList.Where(p => p.TenPhim.Contains(searchTerm)).ToList();
                futurePhimList = futurePhimList.Where(p => p.TenPhim.Contains(searchTerm)).ToList();
                noSchedulePhimList = noSchedulePhimList.Where(p => p.TenPhim.Contains(searchTerm)).ToList();
                allPhimList = allPhimList.Where(p => p.TenPhim.Contains(searchTerm)).ToList();
            }

            var user = Session["User"] as KhachHang; // Lấy user từ Session
			ViewBag.User = user;
			ViewBag.UpcomingPhimList = upcomingPhimList;
			ViewBag.FuturePhimList = futurePhimList;
			ViewBag.NoSchedulePhimList = noSchedulePhimList;
			ViewBag.AllPhimList = allPhimList;

			return View();
		}

        public ActionResult FoodOrder()
        {
			var user = Session["User"] as KhachHang; // Lấy user từ Session
			ViewBag.User = user;
			// Get the list of food from the database
			var foodList = _context.DoAns.ToList();

            // Pass the list of food to the view
            return View(foodList);
        }

        [HttpPost]
        public ActionResult FoodOrder(FormCollection form)
        {
			var user = Session["User"] as KhachHang; // Lấy user từ Session
			ViewBag.User = user;
			if (user == null)
            {
                // Nếu người dùng chưa đăng nhập, hãy chuyển hướng họ đến trang đăng nhập
                return RedirectToAction("Login", "Home");
            }

            // Create a new order
            var order = new HoaDonDoAn
            {
                MaKH = user.MaKH,
                NgayDat = DateTime.Now,
                TongTien = 0m,
            };

            // Add each food item to the order
            foreach (var key in form.AllKeys)
            {
                var quantity = int.Parse(form[key]);
                if (quantity > 0)
                {
                    var food = _context.DoAns.Find(key);
                    if (food != null)
                    {
                        var orderDetail = new ChiTietHoaDon
                        {
                            MaDA = key,
                            SoLuong = quantity,
                        };
                        order.ChiTietHoaDons.Add(orderDetail);
                        if (food.GiaDA.HasValue)
                        {
                            order.TongTien += quantity * food.GiaDA.Value;
                        }
                    }
                }
            }

            // Save the order to the database
            _context.HoaDonDoAns.Add(order);
            _context.SaveChanges();

            return RedirectToAction("OrderSummary", new { id = order.MaHD });
        }

        public ActionResult OrderSummary(int id)
        {
			var user = Session["User"] as KhachHang; // Lấy user từ Session
			ViewBag.User = user;
			// Get the order from the database
			var order = _context.HoaDonDoAns.Include("ChiTietHoaDons").SingleOrDefault(hd => hd.MaHD == id);
            if (order == null)
            {
                // If the order does not exist, redirect to the home page
                return RedirectToAction("Index");
            }

            // Pass the order to the view
            return View(order);
        }


        public ActionResult Logout()
		{
			Session["User"] = null; // Xóa người dùng khỏi phiên
			return RedirectToAction("Index", "Home");
		}

	}
}