using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace BookingFilm.Controllers
{
    public class MovieController : Controller
    {
		// GET: Movie
		private readonly BookingFilmTicketsEntities1 _context;

		public MovieController()
		{
			_context = new BookingFilmTicketsEntities1();
		}

		public ActionResult Index()
		{
			var khachHangs = _context.Phims.ToList(); // Truy vấn dữ liệu từ database
			return View(khachHangs); // Truyền dữ liệu sang View
		}

		public ActionResult Create()
		{
			return View();
		}

		[Obsolete]
		public string UrlImageAfterUpload(HttpPostedFileBase HinhAnh)
		{
			var account = new Account(
						"dzamheemx",  // Cloud name
						"279156174534789",  // API key
						"KFQQWMyliAcvwK7vYvX__qEYstM"   // API secret
					);

			var cloudinary = new Cloudinary(account);

			var uploadParams = new ImageUploadParams()
			{
				File = new FileDescription(Path.GetFileName(HinhAnh.FileName), HinhAnh.InputStream),
				UploadPreset = "ml_default"  // Upload preset name
			};

			var uploadResult = cloudinary.Upload(uploadParams);
			return uploadResult.SecureUri.AbsoluteUri;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaPhim,TenPhim,TheLoai,ThoiLuong,DaoDien,NamSanXuat,HinhPhim,MoTa")] BookingFilm.Phim phim, HttpPostedFileBase HinhPhim)
		{
			if (ModelState.IsValid)
			{
				phim.HinhPhim = UrlImageAfterUpload(HinhPhim);
				_context.Phims.Add(phim);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(phim);
		}


		public ActionResult Delete(int id)
		{
			var phim = _context.Phims.Find(id);
			if (phim == null)
			{
				return HttpNotFound();
			}

			_context.Phims.Remove(phim);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult Edit(int MaPhim)
		{
			var phim = _context.Phims.Find(MaPhim);
			if (phim == null)
			{
				return HttpNotFound();
			}

			return View(phim);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaPhim,TenPhim,TheLoai,ThoiLuong,DaoDien,NamSanXuat,HinhPhim,MoTa")] BookingFilm.Phim phim, HttpPostedFileBase HinhPhim)
		{
			if (ModelState.IsValid)
			{
				var existingPhim = _context.Phims.Find(phim.MaPhim);
				if (existingPhim == null)
				{
					return HttpNotFound();
				}

				existingPhim.TenPhim = phim.TenPhim;
				existingPhim.LichChieux = phim.LichChieux;
				existingPhim.TheLoai = phim.TheLoai;
				existingPhim.ThoiLuong = phim.ThoiLuong;
				existingPhim.NamSanXuat = phim.NamSanXuat;	
				existingPhim.MoTa = phim.MoTa;
				existingPhim.DaoDien = phim.DaoDien;
				

				if (HinhPhim != null && HinhPhim.ContentLength > 0)
				{
					existingPhim.HinhPhim = UrlImageAfterUpload(HinhPhim);
				}

				_context.Entry(existingPhim).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(phim);
		}
	}
}