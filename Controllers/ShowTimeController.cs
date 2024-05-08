using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
    public class ShowTimeController : Controller
    {
		private readonly BookingFilmTicketsEntities1 _context;

		public ShowTimeController()
		{
			_context = new BookingFilmTicketsEntities1();
		}
		public ActionResult Index()
		{
			var lichChieu = _context.LichChieux.Include("Phim").ToList(); // Join với bảng Phim
			ViewBag.MaPhim = new SelectList(_context.Phims, "MaPhim", "TenPhim");
			return View(lichChieu); // Truyền dữ liệu sang View
		}

		public ActionResult Edit(int? id)
		{
			var lichChieu = _context.LichChieux.Find(id);
			if (lichChieu == null)
			{
				return HttpNotFound();
			}

			ViewData["MaPhim"] = new SelectList(_context.Phims, "MaPhim", "TenPhim", lichChieu.MaPhim);
			return View(lichChieu);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaLC,NgayChieu,SuatChieu,MaPhim")] BookingFilm.LichChieu lichChieu)
		{
			if (ModelState.IsValid)
			{
				var existingLichChieu = _context.LichChieux.Find(lichChieu.MaLC);
				if (existingLichChieu == null)
				{
					return HttpNotFound();
				}

				existingLichChieu.NgayChieu = lichChieu.NgayChieu;
				existingLichChieu.SuatChieu = lichChieu.SuatChieu;
				existingLichChieu.MaPhim = lichChieu.MaPhim;

				_context.Entry(existingLichChieu).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewData["MaPhim"] = new SelectList(_context.Phims, "MaPhim", "TenPhim", lichChieu.MaPhim);
			return View(lichChieu);
		}

		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			LichChieu lichChieu = _context.LichChieux.Find(id);
			if (lichChieu == null)
			{
				return HttpNotFound();
			}

			_context.LichChieux.Remove(lichChieu);
			_context.SaveChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Create()
		{
			ViewBag.MaPhim = new SelectList(_context.Phims, "MaPhim", "TenPhim");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaLC,NgayChieu,SuatChieu,MaPhim")] LichChieu lichChieu)
		{
			if (ModelState.IsValid)
			{
				_context.LichChieux.Add(lichChieu);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.MaPhim = new SelectList(_context.Phims, "MaPhim", "TenPhim", lichChieu.MaPhim);
			return View(lichChieu);
		}
	}

}