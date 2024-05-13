using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
    public class CinemaRoomController : Controller
    {

		private readonly BookingFilmTicketsEntities1 _context;
		public CinemaRoomController()
		{
			_context = new BookingFilmTicketsEntities1();
		}
		private bool IsManager()
		{
			var quanLy = Session["User"] as QuanLy;
			return quanLy != null;
		}
		public ActionResult Index(string searchString)
        {
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var rooms = from r in _context.PhongChieux
                        select r;

            if (!String.IsNullOrEmpty(searchString))
            {
                rooms = rooms.Where(s => s.TenPC.Contains(searchString) || s.MaPC.ToString() == searchString);
            }

            return View(rooms.ToList());
        }
        public ActionResult Create()
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			ViewBag.MaRC = new SelectList(_context.RapChieux, "MaRC", "TenRC");
			return View();
		}

		// POST: CinemaRoom/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaPC,TenPC,SoHangGhe,SoGheMoiHang,MaRC")] PhongChieu phongChieu)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				_context.PhongChieux.Add(phongChieu);

				// Tạo số ghế tương ứng
				for (int i = 0; i < phongChieu.SoHangGhe; i++)
				{
					for (int j = 0; j < phongChieu.SoGheMoiHang; j++)
					{
						Ghe ghe = new Ghe
						{
							// Tên ghế theo dạng "{chữ cái hàng}{số ghế 2 chữ số}"
							TenGhe = $"{(char)('A' + i)}{j + 1:D2}",
							LoaiGhe = "Thường", // Thay đổi theo loại ghế thực tế của bạn
							TinhTrangGhe = false, // giả sử ban đầu tất cả ghế đều trống
							MaPC = phongChieu.MaPC
						};
						_context.Ghes.Add(ghe);
					}
				}

				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(phongChieu);
		}

		public ActionResult Delete(int id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var pc = _context.PhongChieux.Find(id);

			if (pc == null)
			{
				return HttpNotFound();
			}

			var ghes = _context.Ghes.Where(g => g.MaPC == id).ToList();

			foreach (var ghe in ghes)
			{
				_context.Ghes.Remove(ghe);
			}

			_context.PhongChieux.Remove(pc);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}


		public ActionResult Edit(int id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var pc = _context.PhongChieux.Find(id);
			if (pc == null)
			{
				return HttpNotFound();
			}
			ViewBag.MaRC = new SelectList(_context.RapChieux, "MaRC", "TenRC", pc.MaRC);
			return View(pc);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaPC,TenPC,SoHangGhe,SoGheMoiHang,MaRC")] PhongChieu phongChieu)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				_context.Entry(phongChieu).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			ViewBag.MaRC = new SelectList(_context.RapChieux, "MaRC", "TenRC", phongChieu.MaRC);
			return View(phongChieu);
		}
	}
}
