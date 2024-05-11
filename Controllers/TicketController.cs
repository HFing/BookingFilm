using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;



namespace BookingFilm.Controllers
{
    public class TicketController : Controller
    {
		// GET: Ticket
		private readonly BookingFilmTicketsEntities1 _context;
		public TicketController()
		{
			_context = new BookingFilmTicketsEntities1();
		}

		public ActionResult Index(int id)
		{
			var phim = _context.Phims.Find(id);
			if (phim == null)
			{
				return HttpNotFound();
			}

			var now = DateTime.Now;
			var future = now.AddDays(7);

			// Lấy danh sách rạp chiếu phim này trong vòng 7 ngày tới
			var rapChieuList = _context.RapChieux
				.Where(rc => rc.PhongChieux.Any(pc => pc.LichChieux.Any(lc => lc.MaPhim == id && lc.NgayChieu >= now && lc.NgayChieu <= future)));

			ViewBag.RapChieuList = new SelectList(rapChieuList, "MaRC", "TenRC");

			// Lấy danh sách lịch chiếu của phim này trong vòng 7 ngày tới
			var lichChieuList = _context.LichChieux
				.Where(lc => lc.MaPhim == id && lc.NgayChieu >= now && lc.NgayChieu <= future);

			ViewBag.LichChieuList = new SelectList(lichChieuList, "MaLC", "NgayChieu");

			return View(phim);
		}


		[HttpPost]
		public ActionResult GetSeats(int MaRC, int MaLC)
		{
			var lichChieu = _context.LichChieux.Include(lc => lc.PhongChieu.Ghes)
											   .Include(lc => lc.Phim)
											   .SingleOrDefault(lc => lc.MaLC == MaLC && lc.PhongChieu.MaRC == MaRC);
			if (lichChieu == null)
			{
				return HttpNotFound();
			}
			// Lưu thông tin phim vào TempData
			TempData["Phim"] = lichChieu.Phim;
			return View("GetSeats", lichChieu);
		}



		[HttpPost]
		public ActionResult CreateTicket(int MaLC, string[] selectedSeats)
		{
			var lichChieu = _context.LichChieux.Include(lc => lc.PhongChieu.Ghes)
											   .Include(lc => lc.Phim)
											   .SingleOrDefault(lc => lc.MaLC == MaLC);
			if (lichChieu == null)
			{
				return HttpNotFound();
			}

			// Tạo vé mới cho mỗi ghế đã chọn
			foreach (var seatId in selectedSeats)
			{
				var ghe = lichChieu.PhongChieu.Ghes.SingleOrDefault(g => g.TenGhe == seatId);
				if (ghe == null)
				{
					return HttpNotFound();
				}

				var ve = new Ve
				{
					MaGhe = ghe.MaGhe,
					MaPhim = lichChieu.Phim.MaPhim,
					// Đặt các thuộc tính khác của vé ở đây, ví dụ: GiaVe
				};

				_context.Ves.Add(ve);
			}

			_context.SaveChanges();

			return View("TicketDetails", lichChieu.Phim);
		}

		public ActionResult ChooseFood()
		{
			var doAns = _context.DoAns.ToList();
			var phim = TempData["Phim"] as Phim; // Thay 'Phim' bằng class thực tế của bạn
			ViewBag.Phim = phim;
			return View(doAns);
		}

	}
}