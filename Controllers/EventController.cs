using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
	public class EventController : Controller
	{
		private readonly BookingFilmTicketsEntities1 _context;


		public EventController()
		{
			_context = new BookingFilmTicketsEntities1();
		}
		// GET: Event
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
			var sk = from sukien in _context.SuKiens select sukien;
            if (!String.IsNullOrEmpty(searchString))
            {
                sk = sk.Where(s => s.TenSK.Contains(searchString) || s.MaSK.ToString() == searchString);
            }
            return View(sk.ToList());
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
		public ActionResult Create([Bind(Include = "MaSK,TenSK,NgayBatDau,NgayKetThuc,MucKhuyenMai")] SuKien suKien)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				_context.SuKiens.Add(suKien);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(suKien);
		}

		public ActionResult Delete(int id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var sk = _context.SuKiens.Find(id);

			if (sk == null)
			{
				return HttpNotFound();
			}

			_context.SuKiens.Remove(sk);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult Edit(int id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			SuKien sk = _context.SuKiens.Find(id);
			if (sk == null)
			{
				return HttpNotFound();
			}
			return View(sk);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaSK,TenSK,NgayBatDau,NgayKetThuc,MucKhuyenMai")] SuKien suKien)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				_context.Entry(suKien).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(suKien);
		}


	}
}