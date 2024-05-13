using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net.Sockets;
using System.Net;



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
			var user = Session["User"] as KhachHang;
			ViewBag.User = user;
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
			Session["Phim"] = lichChieu.Phim; // Lưu thông tin phim vào Session

			return View("GetSeats", lichChieu);
		}



		[HttpPost]
		public ActionResult SaveSelectedSeats(int[] selectedSeats)
		{
			Session["SelectedSeats"] = selectedSeats;
			return Json(new { success = true });
		}



		[HttpGet]
		public ActionResult CreateTicket()
		{
			var selectedSeats = Session["SelectedSeats"] as int[];
			var lichChieu = Session["LichChieu"] as LichChieu;
			var rapChieu = Session["RapChieu"] as RapChieu;
			var phim = Session["Phim"] as Phim;
			var user = Session["User"] as KhachHang;
			if (selectedSeats == null || phim == null)
			{
				// Redirect the user back to the first step of the booking process
				return RedirectToAction("Index");
			}

			foreach (var seatId in selectedSeats)
			{
				var ve = new Ve
				{
					MaPhim = phim.MaPhim,
					MaGhe = seatId,
					MaKH = user.MaKH,
					NgayDat = DateTime.Now,
					GiaVe = _context.Ghes.Find(seatId)?.GiaGhe,
				};

				// Find related event
				var suKien = _context.SuKiens
					.Where(sk => sk.NgayBatDau <= ve.NgayDat && sk.NgayKetThuc >= ve.NgayDat)
					.OrderByDescending(sk => sk.NgayBatDau)
					.FirstOrDefault();

				// Apply discount if event exists
				if (suKien != null)
				{
					ve.ThanhTien = ve.GiaVe * suKien.MucKhuyenMai;
				}
				else
				{
					ve.ThanhTien = ve.GiaVe;
				}

				_context.Ves.Add(ve);

				// Update seat status
				var ghe = _context.Ghes.Find(seatId);
				if (ghe != null)
				{
					ghe.TinhTrangGhe = true;
				}
			}

			_context.SaveChanges();

			return RedirectToAction("ShowTickets");
		}

		public ActionResult ShowTickets()
		{
			var khachHang = Session["User"] as KhachHang;
			ViewBag.User = khachHang;
			if (khachHang == null)
			{
				// Redirect the user back to the first step of the booking process
				return RedirectToAction("Index");
			}

			var today = DateTime.Today;
			// Lấy danh sách vé mà khách hàng này đã đặt trong ngày hôm nay
			var tickets = _context.Ves
				.Include(ve => ve.Phim) // Include Phim data
				.Where(ve => ve.MaKH == khachHang.MaKH && DbFunctions.TruncateTime(ve.NgayDat) == today)
				.ToList();

			return View(tickets);
		}

		
	}
}