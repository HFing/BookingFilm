using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
	public class CinemaController : Controller
	{
		private readonly BookingFilmTicketsEntities1 _context;


		public CinemaController()
		{
			_context = new BookingFilmTicketsEntities1();
		}
		// GET: Cinema
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
			var ra = from r in _context.RapChieux
                     select r;

            if (!String.IsNullOrEmpty(searchString))
            {
                ra = ra.Where(s => s.TenRC.Contains(searchString) || s.MaRC.ToString() == searchString);
            }

            return View(ra.ToList());
        }


        [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaRC,TenRC,DiaChi")] RapChieu rapChieu)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				if (_context.RapChieux.Any(ra => ra.MaRC == rapChieu.MaRC))
				{
					ModelState.AddModelError("MaRC", "Mã rạp chiếu đã tồn tại");
					return View(rapChieu);
				}
				_context.RapChieux.Add(rapChieu);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(rapChieu);
		}

		public ActionResult Delete(int id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			var ra = _context.RapChieux.Find(id);

			if (ra == null)
			{
				return HttpNotFound();
			}

			_context.RapChieux.Remove(ra);
			_context.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult Edit(int id)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			RapChieu ra = _context.RapChieux.Find(id);
			if (ra == null)
			{
				return HttpNotFound();
			}
			return View(ra);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "MaRC,TenRC,DiaChi")] RapChieu rapChieu)
		{
			if (!IsManager())
			{
				return HttpNotFound();
			}
			if (ModelState.IsValid)
			{
				//if (_context.RapChieux.Any(ra => ra.MaRC == rapChieu.MaRC && ra.MaRC != rapChieu.MaRC))
				//{
				//    ModelState.AddModelError("MaRC", "The food code already exists");
				//    return View(rapChieu);
				//}
				_context.Entry(rapChieu).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(rapChieu);


		}
	}
}