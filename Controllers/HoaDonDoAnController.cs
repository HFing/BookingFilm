using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
    public class HoaDonDoAnController : Controller
    {
        private readonly BookingFilmTicketsEntities1 _context;

        public HoaDonDoAnController()
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
            var hd = from hoaDon in _context.HoaDonDoAns select hoaDon;
            if (!String.IsNullOrEmpty(searchString))
            {
                hd = hd.Where(s => s.MaHD.ToString() == searchString);
            }
            return View(hd.ToList());
        }

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

            var hd = _context.HoaDonDoAns.Find(id);

            if (hd == null)
            {
                return HttpNotFound();
            }

            var associatedRecords = _context.ChiTietHoaDons.Where(chd => chd.MaHD == id);
            _context.ChiTietHoaDons.RemoveRange(associatedRecords);

            _context.HoaDonDoAns.Remove(hd);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}