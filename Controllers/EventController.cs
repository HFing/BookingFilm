﻿using System;
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
		public ActionResult Index()
		{
			var sk = _context.SuKiens.ToList(); // Truy vấn dữ liệu từ database
			return View(sk); // Truyền dữ liệu sang View
		}

		public ActionResult Create()
		{
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "MaSK,TenSK,NgayBatDau,NgayKetThuc,MucKhuyenMai")] SuKien suKien)
		{
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