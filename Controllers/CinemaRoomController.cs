using System;
using System.Collections.Generic;
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

		public ActionResult Index()
		{
			var phongChieu = _context.PhongChieux.ToList(); // Truy vấn dữ liệu từ database
			return View(phongChieu); // Truyền dữ liệu sang View
		}
		public ActionResult Create()
		{
			ViewBag.MaRC = new SelectList(_context.RapChieux, "MaRC", "TenRC");
			return View();
		}

		// POST: CinemaRoom/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaPC,TenPC,SoHangGhe,SoGheMoiHang,MaRC")] PhongChieu phongChieu)
		{
			if (ModelState.IsValid)
			{
				_context.PhongChieux.Add(phongChieu);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(phongChieu);
		}

		public ActionResult Delete(int id)
		{
			var pc = _context.PhongChieux.Find(id);

			if (pc == null)
			{
				return HttpNotFound();
			}

			_context.PhongChieux.Remove(pc);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
