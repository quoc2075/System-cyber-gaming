using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyQuanNet.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
       
            public ActionResult Index()
            {
                if (Session["VaiTro"] == null || Session["VaiTro"].ToString() != "Admin")
                    return RedirectToAction("Login", "NguoiDung");

                ViewBag.HoTen = Session["HoTen"];
                return View();
            }
    }

    
}