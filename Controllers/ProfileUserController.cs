using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
    public class ProfileUserController : Controller
    {
		private readonly BookingFilmTicketsEntities1 _context;


		public ProfileUserController()
		{
			_context = new BookingFilmTicketsEntities1();
		}
		public ActionResult Index()
		{
			KhachHang khachHang = Session["User"] as KhachHang;
			if (khachHang != null)
			{
				return View(khachHang);
			}
			else
			{
				return RedirectToAction("Index", "Login");
			}
		}

		[HttpPost]
		public ActionResult Edit(KhachHang model)
		{
			KhachHang khachHang = Session["User"] as KhachHang;
			if (khachHang == null)
			{

				return RedirectToAction("Index", "Login");
			}


			if (_context.KhachHangs.Any(kh => kh.Email == model.Email && kh.MaKH != khachHang.MaKH))
			{

				ModelState.AddModelError("Email", "Email đã tồn tại trong hệ thống.");

				return View("Index", model);
			}


			KhachHang updatedKhachHang = new KhachHang
			{
				MaKH = khachHang.MaKH,
				HoTenKH = model.HoTenKH,
				Email = model.Email,
				MatKhauKH = khachHang.MatKhauKH,
				NgaySinh = model.NgaySinh,
				GioiTinh = khachHang.GioiTinh,
				CCCD = khachHang.CCCD,
				DiaChi = model.DiaChi
			};


			_context.Entry(updatedKhachHang).State = EntityState.Modified;


			_context.SaveChanges();


			Session["User"] = updatedKhachHang;
			TempData["SuccessMessage"] = "Update successful!";

			return RedirectToAction("Index");
		}
		// Trong ProfileUserController.cs
		public ActionResult ChangePassword()
		{
			return View();
		}
		[HttpPost]
		public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmNewPassword)
		{
			KhachHang sessionKhachHang = Session["User"] as KhachHang;
			if (sessionKhachHang == null)
			{
				return RedirectToAction("Login", "Home");
			}

			// Get the KhachHang from the current DbContext
			KhachHang khachHang = _context.KhachHangs.Find(sessionKhachHang.MaKH);

			if (newPassword != confirmNewPassword)
			{
				ViewBag.ConfirmPasswordError = "New password does not match.";
				return View();
			}

			if (khachHang.MatKhauKH != currentPassword)
			{
				ViewBag.CurrentPasswordError = "Current password is incorrect.";
				return View();
			}

			khachHang.MatKhauKH = newPassword;
			_context.Entry(khachHang).State = EntityState.Modified;
			_context.SaveChanges();

			TempData["SuccessMessage"] = "Updated password successfully!";

			return RedirectToAction("ChangePassword");
		}

	}
}