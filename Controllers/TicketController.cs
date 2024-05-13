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
				//totalGiaVe = ghes.Sum(ghe => ghe.GiaVe); // Tính tổng giá vé
			}

			// Lưu thông tin ghế vào TempData
			TempData["Ghes"] = ghes;
			TempData.Keep("Ghes");

			// Lưu tổng giá vé vào TempData
			TempData["TotalGiaVe"] = totalGiaVe;
			TempData.Keep("TotalGiaVe");

			// Lưu thông tin phim vào TempData
			TempData["Phim"] = lichChieu.Phim;
			TempData.Keep("Phim");

			return View("GetSeats", lichChieu);
		}

		public ActionResult ChooseFood()
		{
			var doAns = _context.DoAns.ToList();
			var phim = TempData["Phim"] as Phim;
			if (phim == null)
			{
				return HttpNotFound();
			}
			ViewBag.Phim = phim;
			TempData.Keep("Phim"); // Giữ lại dữ liệu cho yêu cầu tiếp theo

			return View(doAns);
		}

		[HttpPost]
		public ActionResult ChooseFood(int MaDoAn)
		{
			var doAn = _context.DoAns.Find(MaDoAn);
			if (doAn == null)
			{
				return HttpNotFound();
			}
			TempData["DoAn"] = doAn;
			TempData.Keep("DoAn");

            var ghes = TempData["Ghes"] as List<Ghe>;

            if (ghes == null)
            {
                return HttpNotFound();
            }

            // Truyền thông tin này đến view thông qua ViewBag
            ViewBag.SeatIds = ghes.Select(ghe => ghe.MaGhe).ToList();
            ViewBag.FoodId = doAn.MaDA;

            return View("ConfirmSelection");

            //return RedirectToAction("CreateTicket");
        }
		//[HttpPost]
		public ActionResult CreateTicket()
		{
			// Lấy thông tin từ TempData hoặc Session
			var phim = TempData["Phim"] as Phim;
			var ghe = TempData["Ghe"] as Ghe;
			var doAn = TempData["DoAn"] as DoAn;
			var khachHang = TempData["KhachHang"] as KhachHang; // Giả sử bạn đã lưu thông tin khách hàng

			if (phim == null || ghe == null || doAn == null || khachHang == null)
			{
				return HttpNotFound();
			}

			// Tạo vé mới
			var ve = new Ve
			{
				MaPhim = phim.MaPhim,
				MaGhe = ghe.MaGhe,
				MaKH = khachHang.MaKH,
				MaDoAn = doAn.MaDA,
				NgayDat = DateTime.Now,
				//GiaVe = phim.GiaVe, 
				//TongTien = phim.GiaVe + doAn.GiaDA,
			};

			// Thêm vé vào cơ sở dữ liệu
			_context.Ves.Add(ve);
			_context.SaveChanges();

			// Xóa thông tin đã lưu trong TempData hoặc Session
			TempData.Remove("Phim");
			TempData.Remove("Ghe");
			TempData.Remove("DoAn");
			TempData.Remove("KhachHang");

			// Chuyển hướng đến trang xác nhận hoặc trang chi tiết vé
			return RedirectToAction("Confirmation", new { id = ve.MaVe });
		}

        [HttpPost]
        public ActionResult ConfirmSelection()
        {
            // Lấy thông tin từ TempData
            var ghes = TempData["Ghes"] as List<Ghe>;
            var doAn = TempData["DoAn"] as DoAn;

            if (ghes == null || doAn == null)
            {
                return HttpNotFound();
            }

            // Truyền thông tin này đến view thông qua ViewBag
            ViewBag.SeatIds = ghes.Select(ghe => ghe.MaGhe).ToList();
            ViewBag.FoodId = doAn.MaDA;

            return View("ConfirmSelection");
        }

    }
}