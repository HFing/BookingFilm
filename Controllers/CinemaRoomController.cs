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
