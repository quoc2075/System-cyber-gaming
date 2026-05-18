using System;
using System.Collections.Generic;
using System.Web.Mvc;
using QuanLyQuanNet.Models;
using QuanLyQuanNet.DAL;
using MySqlConnector;

namespace QuanLyQuanNet.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            if (Session["VaiTro"] == null || Session["VaiTro"].ToString() != "User")
                return RedirectToAction("Login", "NguoiDung");

            ViewBag.HoTen = Session["HoTen"];

            List<MayTinh> mayRanh = new List<MayTinh>();
            double soDu = 0;

            using (MySqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                // Lấy số dư người dùng
                int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);
                string sqlSoDu = "SELECT SoDu FROM NguoiDung WHERE MaNguoiDung = @MaNguoiDung";
                using (MySqlCommand cmd = new MySqlCommand(sqlSoDu, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNguoiDung", maNguoiDung);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        soDu = Convert.ToDouble(result);
                }

                ViewBag.SoDu = soDu;

                // Nếu số dư <= 0 thì không cho phép sử dụng
                if (soDu <= 0)
                {
                    ViewBag.ThongBao = "Tài khoản của bạn không còn tiền. Vui lòng nạp thêm để sử dụng dịch vụ.";
                    return View(new List<MayTinh>()); // Danh sách rỗng
                }

                // Lấy danh sách máy đang rảnh
                string tt = "SELECT * FROM MayTinh WHERE TrangThai = 'Rảnh'";
                using (MySqlCommand cmd = new MySqlCommand(tt, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mayRanh.Add(new MayTinh
                        {
                            MaMay = Convert.ToInt32(reader["MaMay"]),
                            TenMay = reader["TenMay"].ToString(),
                            DonGia = Convert.ToDouble(reader["DonGia"])
                        });
                    }
                }
            }

            return View(mayRanh);
        }
    }
}
