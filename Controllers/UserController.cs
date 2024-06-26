﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
    public class UserController : Controller
    {
		private readonly BookingFilmTicketsEntities1 _context;


		public UserController()
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
			var khachHangs = from k in _context.KhachHangs select k;
			if(!String.IsNullOrEmpty(searchString))
			{
				khachHangs = khachHangs.Where(s => s.HoTenKH.Contains(searchString) || s.MaKH.ToString() == searchString);
			}	
			return View(khachHangs.ToList()); // Truyền dữ liệu sang View
		}
		public ActionResult Delete(int maKH)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var khachHang = _context.KhachHangs.Find(maKH);
			if (khachHang == null)
			{
				return HttpNotFound();
			}
            if (khachHang.HoaDonDoAns.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete this user because there are food bills associated with it.";
                return RedirectToAction("Index");
            }
			if(khachHang.Ves.Any())
			{
                TempData["ErrorMessage"] = "Cannot delete this user because there are ticket bills associated with it.";
                return RedirectToAction("Index");
            }	
            _context.KhachHangs.Remove(khachHang);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult Create()
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaKH,HoTenKH,Email,MatKhauKH,NgaySinh,GioiTinh,CCCD,DiaChi")] KhachHang khachHang)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				// Check if the CCCD already exists in the database
				if (_context.KhachHangs.Any(kh => kh.CCCD == khachHang.CCCD))
				{
					ModelState.AddModelError("CCCD", "CCCD already exists.");
				}

				// Check if the Email already exists in the database
				if (_context.KhachHangs.Any(kh => kh.Email == khachHang.Email))
				{
					ModelState.AddModelError("Email", "Email already exists.");
				}

				if (!ModelState.IsValid)
				{
					// If there are any errors, return the view with the errors
					return View(khachHang);
				}

				_context.KhachHangs.Add(khachHang);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(khachHang);
		}


		public ActionResult Edit(int? MaKH)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (MaKH == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			KhachHang khachHang = _context.KhachHangs.Find(MaKH);
			if (khachHang == null)
			{
				return HttpNotFound();
			}
			return View(khachHang);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaKH,HoTenKH,Email,MatKhauKH,NgaySinh,GioiTinh,CCCD,DiaChi")] KhachHang khachHang)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				// Check CCCD in database
				if (_context.KhachHangs.Any(kh => kh.CCCD == khachHang.CCCD && kh.MaKH != khachHang.MaKH))
				{
					ModelState.AddModelError("CCCD", "CCCD already exists.");
				}

				// Check  Email  in  database
				if (_context.KhachHangs.Any(kh => kh.Email == khachHang.Email && kh.MaKH != khachHang.MaKH))
				{
					ModelState.AddModelError("Email", "Email already exists.");
				}

				if (!ModelState.IsValid)
				{
					// If  any errors, return the view with the errors
					return View(khachHang);
				}

				_context.Entry(khachHang).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(khachHang);
		}
	}
}