using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
	public class AdminController : Controller
	{
		private readonly BookingFilmTicketsEntities1 _context;


		public AdminController()
		{
			_context = new BookingFilmTicketsEntities1();
		}
		private bool IsManager()
		{
			var quanLy = Session["User"] as QuanLy;
			return quanLy != null;
		}
		public ActionResult Index()
		{
			var quanLy = Session["User"] as QuanLy;
			if (quanLy == null)
			{
				return HttpNotFound();
			}

			return View();
		}

		public ActionResult ThongKeVe(DateTime startDate, DateTime endDate)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var ve = _context.Ves
				.Where(v => DbFunctions.TruncateTime(v.NgayDat) >= startDate.Date && DbFunctions.TruncateTime(v.NgayDat) <= endDate.Date)
				.ToList();

			if (ve == null)
			{
				return HttpNotFound();
			}

			return View(ve);
		}

		public ActionResult ThongKePhim(DateTime startDate, DateTime endDate)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var phim = _context.Phims
				.Where(p => p.LichChieux.Any(lc => DbFunctions.TruncateTime(lc.NgayChieu) >= startDate.Date && DbFunctions.TruncateTime(lc.NgayChieu) <= endDate.Date))
				.ToList();

			if (phim == null)
			{
				return HttpNotFound();
			}

			return View(phim);
		}

		public ActionResult ThongKeDoanhThu(DateTime startDate, DateTime endDate)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			ViewBag.startDate = startDate.ToString("dd-MM-yyyy");
			ViewBag.endDate = endDate.ToString("dd-MM-yyyy");

			ViewBag.doanhThuVe = _context.Ves
				.Where(v => DbFunctions.TruncateTime(v.NgayDat) >= startDate.Date && DbFunctions.TruncateTime(v.NgayDat) <= endDate.Date)
				.Select(v => v.ThanhTien)
				.DefaultIfEmpty(0)
				.Sum();

			ViewBag.doanhThuDoAn = _context.HoaDonDoAns
				.Where(hd => DbFunctions.TruncateTime(hd.NgayDat) >= startDate.Date && DbFunctions.TruncateTime(hd.NgayDat) <= endDate.Date)
				.Select(hd => hd.TongTien)
				.DefaultIfEmpty(0)
				.Sum();

			ViewBag.tongDoanhThu = ViewBag.doanhThuVe + ViewBag.doanhThuDoAn;

			return View();
		}

		public ActionResult ListAdmin()
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var admins = _context.QuanLies.ToList();

			if (admins == null)
			{
				return HttpNotFound();
			}

			return View(admins);
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
		public ActionResult Create([Bind(Include = "ID,UserName,MatKhau")] QuanLy admin)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				// Kiểm tra xem username đã tồn tại trong database chưa
				var existingUser = _context.QuanLies.Any(u => u.UserName == admin.UserName);
				if (existingUser)
				{
					// Nếu username đã tồn tại, thêm một lỗi vào ModelState
					ModelState.AddModelError("UserName", "Username already exists.");
				}
				else
				{
					// Nếu username chưa tồn tại, tiếp tục với logic tạo mới
					_context.QuanLies.Add(admin);
					_context.SaveChanges();
					return RedirectToAction("ListAdmin");
				}
			}
			return View(admin);
		}

		// GET: Admin/Edit/5
		public ActionResult Edit(int? id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			QuanLy admin = _context.QuanLies.Find(id);
			if (admin == null)
			{
				return HttpNotFound();
			}
			return View(admin);
		}

		// POST: Admin/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(QuanLy model)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				// Kiểm tra xem username đã tồn tại trong database chưa
				var existingUser = _context.QuanLies.Any(u => u.UserName == model.UserName);
				if (existingUser)
				{
					// Nếu username đã tồn tại, thêm một lỗi vào ModelState
					ModelState.AddModelError("UserName", "Username already exists.");
				}
				else
				{
					// Nếu username chưa tồn tại, tiếp tục với logic cập nhật
					_context.Entry(model).State = EntityState.Modified;
					_context.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			return View(model);
		}

		// GET: Admin/Delete/5
		public ActionResult Delete(int? id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			QuanLy admin = _context.QuanLies.Find(id);
			if (admin == null)
			{
				return HttpNotFound();
			}
			return View(admin);
		}

		// POST: Admin/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			QuanLy currentAdmin = Session["User"] as QuanLy;
			if (currentAdmin == null || currentAdmin.ID == id)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Cannot delete self");
			}
			QuanLy admin = _context.QuanLies.Find(id);
			_context.QuanLies.Remove(admin);
			_context.SaveChanges();
			return RedirectToAction("ListAdmin");
		}

	}
}
