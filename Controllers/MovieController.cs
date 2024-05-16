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
			var phim = from p in _context.Phims select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                phim = phim.Where(p => p.TenPhim.Contains(searchString) || p.MaPhim.ToString() == searchString);
            }
            return View(phim.ToList());
        }

        public ActionResult Create()
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
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
			if (!IsManager())
			{
				return HttpNotFound();
			}
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
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var phim = _context.Phims.Find(id);
			if (phim == null)
			{
				return HttpNotFound();
			}
            if (phim.LichChieux.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete this movie because there are schedules associated with it.";
                return RedirectToAction("Index");
            }

            _context.Phims.Remove(phim);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult Edit(int MaPhim)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
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
			if (!IsManager())
			{
				return HttpNotFound();
			}
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