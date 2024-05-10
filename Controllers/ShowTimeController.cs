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
        public ActionResult Index(string tenPhim, DateTime? ngayChieuTu, DateTime? ngayChieuDen, int? maPC)
        {
            var lichChieuQuery = _context.LichChieux.Include("Phim").Include("PhongChieu").AsQueryable();

            if (!string.IsNullOrEmpty(tenPhim))
            {
                lichChieuQuery = lichChieuQuery.Where(lc => lc.Phim.TenPhim.Contains(tenPhim));
            }

            if (ngayChieuTu != null && ngayChieuDen != null)
            {
                lichChieuQuery = lichChieuQuery.Where(lc => lc.NgayChieu >= ngayChieuTu && lc.NgayChieu <= ngayChieuDen);
            }

            if (maPC != null)
            {
                lichChieuQuery = lichChieuQuery.Where(lc => lc.MaPC == maPC);
            }

            var lichChieu = lichChieuQuery.ToList();

            ViewBag.MaPhim = new SelectList(_context.Phims, "MaPhim", "TenPhim");
            ViewBag.maPC = new SelectList(_context.PhongChieux, "MaPC", "TenPC"); // Sửa key thành "maPC"

            ViewBag.TenPhim = tenPhim;
            ViewBag.NgayChieuTu = ngayChieuTu;
            ViewBag.NgayChieuDen = ngayChieuDen;

            return View(lichChieu);
        }


        public ActionResult Edit(int? id)
		{
			var lichChieu = _context.LichChieux.Find(id);
			if (lichChieu == null)
			{
				return HttpNotFound();
			}

			ViewData["MaPhim"] = new SelectList(_context.Phims, "MaPhim", "TenPhim", lichChieu.MaPhim);
			ViewData["MaPC"] = new SelectList(_context.PhongChieux, "MaPC", "TenPC", lichChieu.MaPC); // SelectList cho Phòng Chiếu
			return View(lichChieu);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaLC,NgayChieu,SuatChieu,MaPhim,MaPC")] BookingFilm.LichChieu lichChieu) // Thêm MaPC vào Bind
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
				existingLichChieu.MaPC = lichChieu.MaPC; // Cập nhật MaPC

				_context.Entry(existingLichChieu).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewData["MaPhim"] = new SelectList(_context.Phims, "MaPhim", "TenPhim", lichChieu.MaPhim);
			ViewData["MaPC"] = new SelectList(_context.PhongChieux, "MaPC", "TenPC", lichChieu.MaPC); // SelectList cho Phòng Chiếu
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

		public ActionResult Create()
		{
			ViewBag.MaPhim = new SelectList(_context.Phims, "MaPhim", "TenPhim");
			ViewBag.MaPC = new SelectList(_context.PhongChieux, "MaPC", "TenPC"); // SelectList cho Phòng Chiếu
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaLC,NgayChieu,SuatChieu,MaPhim,MaPC")] LichChieu lichChieu) // Thêm MaPC vào Bind
		{
			if (ModelState.IsValid)
			{
				_context.LichChieux.Add(lichChieu);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			ViewBag.MaPhim = new SelectList(_context.Phims, "MaPhim", "TenPhim", lichChieu.MaPhim);
			ViewBag.MaPC = new SelectList(_context.PhongChieux, "MaPC", "TenPC", lichChieu.MaPC); // SelectList cho Phòng Chiếu
			return View(lichChieu);
		}
	}

}