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
			ViewBag.RapChieuList = new SelectList(_context.RapChieux, "MaRC", "TenRC");
			var now = DateTime.Now;
			ViewBag.LichChieuList = new SelectList(_context.LichChieux
				.Where(lc => lc.MaPhim == id && (lc.NgayChieu > now || (lc.NgayChieu == now.Date && lc.SuatChieu > now.TimeOfDay)))
				, "MaLC", "NgayChieu");
			return View(phim);
		}

		[HttpPost]
		public ActionResult GetSeats(int MaRC, int MaLC)
		{
			// Lấy LichChieu tương ứng với MaLC
			var lichChieu = _context.LichChieux.Include(lc => lc.Phim).FirstOrDefault(lc => lc.MaLC == MaLC);

			if (lichChieu == null)
			{
				// Xử lý trường hợp không tìm thấy LichChieu
				return HttpNotFound();
			}

			// Lấy thông tin phim từ LichChieu
			var phim = lichChieu.Phim;

			// Lấy danh sách ghế từ PhongChieu
			var seats = _context.PhongChieux.FirstOrDefault(pc => pc.MaRC == MaRC)?.Ghes.ToList();

			// Truyền Phim và Ghes vào ViewBag
			ViewBag.Phim = phim;
			ViewBag.Ghes = seats;

			// Trả về view
			return View();
		}

	}
}