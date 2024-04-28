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
			return View();
		}

	}
}
