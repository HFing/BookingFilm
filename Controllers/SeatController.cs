using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	}
}