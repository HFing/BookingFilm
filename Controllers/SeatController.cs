using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
    public class SeatController : Controller
    {
		// GET: Seat
		private readonly BookingFilmTicketsEntities1 _context;


		public SeatController()
		{
			_context = new BookingFilmTicketsEntities1();
		}
		public ActionResult Index()
		{
			ViewBag.MaRC = new SelectList(_context.RapChieux, "MaRC", "TenRC");
			ViewBag.MaPC = new SelectList(_context.PhongChieux, "MaPC", "TenPC");
			return View();
		}
		public JsonResult GetCinemaRooms(int MaRC)
		{
			var cinemaRooms = _context.PhongChieux.Where(x => x.MaRC == MaRC).Select(x => new { Value = x.MaPC, Text = x.TenPC });
			return Json(cinemaRooms, JsonRequestBehavior.AllowGet);
		}
		// Trong controller Seat của bạn
		[HttpPost]
		public ActionResult Index(int MaPC)
		{
			var cinemaRoom = _context.PhongChieux.FirstOrDefault(x => x.MaPC == MaPC);
			if (cinemaRoom == null)
			{
				return HttpNotFound();
			}
			return View("CinemaRoomLayout", cinemaRoom);
		}
		public ActionResult Edit(int id)
		{
			Ghe ghe = _context.Ghes.Find(id);
			if (ghe == null)
			{
				return HttpNotFound();
			}
			return View(ghe);
		}
		[HttpPost]
		public ActionResult Edit(Ghe ghe)
		{
			if (ModelState.IsValid)
			{
				_context.Entry(ghe).State = EntityState.Modified;
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(ghe);
		}
		public ActionResult Delete(int id)
		{
			Ghe ghe = _context.Ghes.Find(id);
			if (ghe == null)
			{
				return HttpNotFound();
			}

			// First, retrieve the data from the database
			var ghes = _context.Ghes.ToList();

			// Then, perform the operations that are not supported by LINQ to Entities
			var seatsToUpdate = ghes.Where(g => g.MaPC == ghe.MaPC && g.TenGhe[0] == ghe.TenGhe[0] && g.TenGhe.CompareTo(ghe.TenGhe) > 0).OrderBy(g => g.TenGhe).ToList();

			// Remove the seat
			_context.Ghes.Remove(ghe);

			// Update the names of the remaining seats
			foreach (var seat in seatsToUpdate)
			{
				var seatNumber = int.Parse(seat.TenGhe.Substring(1));
				seat.TenGhe = seat.TenGhe[0] + (seatNumber - 1).ToString("D2"); // Use "D2" to format the number with two digits
			}

			_context.SaveChanges();
			return RedirectToAction("Index");
		}





	}
}