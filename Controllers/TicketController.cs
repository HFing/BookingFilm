using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net.Sockets;



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
		public ActionResult GetSeats(int MaRC, int MaLC, string[] selectedSeats)
		{
			var lichChieu = _context.LichChieux.Include(lc => lc.PhongChieu.Ghes)
											   .Include(lc => lc.Phim)
											   .SingleOrDefault(lc => lc.MaLC == MaLC && lc.PhongChieu.MaRC == MaRC);
			if (lichChieu == null)
			{
				return HttpNotFound();
			}

			List<Ghe> ghes = new List<Ghe>();
			decimal totalGiaVe = 0;
			if (selectedSeats != null)
			{
				ghes = _context.Ghes.Where(ghe => selectedSeats.Contains(ghe.TenGhe)).ToList();
				totalGiaVe += ghes.Sum(ghe => ghe.GiaGhe.GetValueOrDefault()); // Tính tổng giá vé
			}

			var rapChieu = _context.RapChieux.Find(MaRC);
			Session["selectedSeats"] = selectedSeats;

			// Lưu chúng vào Session
			Session["RapChieu"] = rapChieu;
			Session["LichChieu"] = lichChieu;
			Session["SelectedSeats"] = selectedSeats; // Lưu số ghế đã chọn vào Session
			Session["Phim"] = lichChieu.Phim; // Lưu thông tin phim vào Session

			return View("GetSeats", lichChieu);
		}

		public ActionResult ChooseFood()
		{
			var doAns = _context.DoAns.ToList();
			var phim = System.Web.HttpContext.Current.Session["Phim"] as Phim;
			if (phim == null)
			{
				// Redirect the user back to the first step of the booking process
				return RedirectToAction("Index");
			}

			ViewBag.Phim = phim; // Set ViewBag.Phim

			return View(doAns);
		}

		[HttpPost]
		public ActionResult ChooseFood(FormCollection form)
		{
			var quantities = new Dictionary<string, int>();

			foreach (var key in form.AllKeys)
			{
				var quantity = int.Parse(form[key]);
				if (quantity > 0)
				{
					quantities[key] = quantity;
				}
			}

			// Retrieve the DoAn objects from the database
			var doAns = _context.DoAns.Where(da => quantities.Keys.Contains(da.TenDA)).ToList();

			// Store the DoAn objects and quantities in the session
			Session["DoAn"] = doAns;
			Session["Quantities"] = quantities;

			return RedirectToAction("CreateTicket");
		}

		[HttpPost]
		public ActionResult CreateTicket()
		{
			// Lấy thông tin từ Session
			var phim = System.Web.HttpContext.Current.Session["Phim"] as Phim;
			var rapChieu = System.Web.HttpContext.Current.Session["RapChieu"] as RapChieu;
			var lichChieu = System.Web.HttpContext.Current.Session["LichChieu"] as LichChieu;
			var selectedSeats = System.Web.HttpContext.Current.Session["SelectedSeats"] as string[];
			var doAn = System.Web.HttpContext.Current.Session["DoAn"] as DoAn;

			if (phim == null || rapChieu == null || lichChieu == null || selectedSeats == null || doAn == null)
			{
				return HttpNotFound();
			}

		   var ve = new Ve
			{
				MaPhim = phim.MaPhim,
				MaGhe = _context.Ghes.FirstOrDefault(ghe => selectedSeats.Contains(ghe.TenGhe)).MaGhe, // Giả sử rằng mỗi vé chỉ tương ứng với một ghế
				NgayDat = DateTime.Now,
				MaDoAn = doAn.MaDA,
				ThanhTien = doAn.GiaDA + _context.Ghes.Where(ghe => selectedSeats.Contains(ghe.TenGhe)).Sum(ghe => ghe.GiaGhe.GetValueOrDefault()) // Giả sử rằng ThanhTien là tổng giá của vé và đồ ăn
			};

			// Lưu vé vào cơ sở dữ liệu
			_context.Ves.Add(ve);
			_context.SaveChanges();

			// Xóa dữ liệu từ Session
			System.Web.HttpContext.Current.Session.Remove("Phim");
			System.Web.HttpContext.Current.Session.Remove("RapChieu");
			System.Web.HttpContext.Current.Session.Remove("LichChieu");
			System.Web.HttpContext.Current.Session.Remove("SelectedSeats");
			System.Web.HttpContext.Current.Session.Remove("DoAn");

			// Chuyển hướng đến trang xác nhận
			return RedirectToAction("Confirmation", new { id = ve.MaVe });
		}


	}
}