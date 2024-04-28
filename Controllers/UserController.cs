using System;
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

		public ActionResult Index()
		{
			var khachHangs = _context.KhachHangs.ToList(); // Truy vấn dữ liệu từ database
			return View(khachHangs); // Truyền dữ liệu sang View
		}
		public ActionResult Delete(int maKH)
		{
			var khachHang = _context.KhachHangs.Find(maKH);
			if (khachHang == null)
			{
				return HttpNotFound();
			}

			_context.KhachHangs.Remove(khachHang);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult Create()
		{
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaKH,HoTenKH,Email,MatKhauKH,NgaySinh,GioiTinh,CCCD,DiaChi")] KhachHang khachHang)
		{
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