using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookingFilm.Controllers
{
	public class AdminController : Controller
	{

		public ActionResult Index()
		{
			var quanLy = Session["User"] as QuanLy;
			if (quanLy == null)
			{
				return HttpNotFound();
			}

			return View();
		}
	}
}
