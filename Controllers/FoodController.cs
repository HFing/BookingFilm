﻿using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;


namespace BookingFilm.Controllers
{
	public class FoodController : Controller
	{
		private readonly BookingFilmTicketsEntities1 _context;


		public FoodController()
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
			var da = from f in _context.DoAns
                     select f;

            if (!String.IsNullOrEmpty(searchString))
            {
                da = da.Where(s => s.TenDA.Contains(searchString)
                                   || s.MaDA.Contains(searchString));
            }

            return View(da.ToList());
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
		public ActionResult Create([Bind(Include = "MaDA,TenDA,GiaDA,HinhDA")] DoAn doAn,HttpPostedFileBase HinhDA)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				// Kiểm tra xem mã đồ ăn đã tồn tại chưa
				if (_context.DoAns.Any(da => da.MaDA == doAn.MaDA))
				{
					ModelState.AddModelError("MaDA", "Mã đồ ăn đã tồn tại");
					return View(doAn);
				}
				doAn.HinhDA = UrlImageAfterUpload(HinhDA);	
				_context.DoAns.Add(doAn);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(doAn);
		}
		// GET: Food/Delete/BAPPHOMAI
		public ActionResult Delete(string id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var doAn = _context.DoAns.Find(id);
			if (doAn == null)
			{
				return HttpNotFound();
			}
			if (doAn.ChiTietHoaDons.Any())
			{
                TempData["ErrorMessage"] = "Cannot delete this food because there are food bills associated with it.";
                return RedirectToAction("Index");
            }
			_context.DoAns.Remove(doAn);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult Edit(string id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			DoAn doAn = _context.DoAns.Find(id);
			if (doAn == null)
			{
				return HttpNotFound();
			}
			return View(doAn);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaDA,TenDA,GiaDA")] DoAn doAn, HttpPostedFileBase HinhDA)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				var existingDoAn = _context.DoAns.Find(doAn.MaDA);
				if (existingDoAn == null)
				{
					return HttpNotFound();
				}

				if (_context.DoAns.Any(da => da.MaDA != doAn.MaDA && da.MaDA == doAn.MaDA))
				{
					ModelState.AddModelError("MaDA", "The food code already exists");
					return View(doAn);
				}

				existingDoAn.TenDA = doAn.TenDA;
				existingDoAn.GiaDA = doAn.GiaDA;
				if (HinhDA != null && HinhDA.ContentLength > 0)
				{
					existingDoAn.HinhDA = UrlImageAfterUpload(HinhDA);
				}

				_context.Entry(existingDoAn).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(doAn);
		}
		//// GET: Food/Delete/BAPPHOMAI
		//public ActionResult Delete(string id)
		//{
		//	var doAn = _context.DoAns.Find(id);
		//	if (doAn == null)
		//	{
		//		return HttpNotFound();
		//	}
		//	return View(doAn);
		//}

		//// POST: Food/Delete/BAPPHOMAI
		//[HttpPost]
		//public ActionResult DeleteConfirmed(string id)
		//{
		//	var doAn = _context.DoAns.Find(id);
		//	_context.DoAns.Remove(doAn);
		//	_context.SaveChanges();
		//	return RedirectToAction("Index");
		//}
	}
}