using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
    public class LoginController : Controller
    {
		// GET: Login

		private readonly BookingFilmTicketsEntities1 _context;

		public LoginController()
		{
			_context = new BookingFilmTicketsEntities1();
		}

		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Index(string Email, string MatKhauKH)
		{
			var user = _context.KhachHangs.FirstOrDefault(u => u.Email == Email && u.MatKhauKH == MatKhauKH);

			if (user != null)
			{
				Session["User"] = user; // Lưu user vào Session
				TempData["Message"] = "Logged in successfully!";
				return RedirectToAction("Index", "Home");
			}
			else
			{
				TempData["Message"] = "User account or password incorrect!";
				return RedirectToAction("Index");
			}
		}

		[HttpPost]
		public ActionResult CreateAccount(KhachHang user)
		{
			if (ModelState.IsValid)
			{
				_context.KhachHangs.Add(user);
				_context.SaveChanges();
				TempData["SuccessMessage"] = "Sign Up Success!";
				return RedirectToAction("Index");
			}

			return View(user);
		}

		[HttpGet]
		public JsonResult CheckExists(string cccd, string email)
		{
			bool cccdExists = _context.KhachHangs.Any(kh => kh.CCCD == cccd);
			bool emailExists = _context.KhachHangs.Any(kh => kh.Email == email);
			return Json(new { cccdExists = cccdExists, emailExists = emailExists }, JsonRequestBehavior.AllowGet);
		}
	}
}