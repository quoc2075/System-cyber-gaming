using System.Web.Mvc;
using QuanLyQuanNet.Models;
using QuanLyQuanNet.DAL;
using System;
using MySqlConnector;
using System.Data.Common;

namespace QuanLyQuanNet.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string soDienThoai, string matKhau)
        {
            using (MySqlConnection conn = DAL.DbConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM NguoiDung WHERE SoDienThoai = @sdt AND MatKhau = @matKhau";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@sdt", soDienThoai);
                cmd.Parameters.AddWithValue("@matKhau", matKhau);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Session["MaNguoiDung"] = reader["MaNguoiDung"].ToString();
                    Session["HoTen"] = reader["HoTen"].ToString();
                    Session["VaiTro"] = reader["VaiTro"].ToString();

                    if (reader["VaiTro"].ToString() == "Admin")
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "User");
                }
                else
                {
                    ViewBag.ThongBao = "Sai số điện thoại hoặc mật khẩu!";
                    return View();
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        public ActionResult DangKy()
        {
            return View();
        }

        // POST: DangKy
        [HttpPost]
        public ActionResult DangKy(string hoTen, string matKhau, string soDienThoai)
        {
            using (MySqlConnection conn = DAL.DbConnection.GetConnection())
            {
                conn.Open();

                // Kiểm tra trùng số điện thoại
                string checkQuery = "SELECT COUNT(*) FROM NguoiDung WHERE SoDienThoai = @sdt";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@sdt", soDienThoai);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    ViewBag.ThongBao = "Số điện thoại đã tồn tại!";
                    return View();
                }

                string insert = "INSERT INTO NguoiDung(HoTen, MatKhau, SoDienThoai, SoDu, VaiTro) " +
                                "VALUES (@ht, @mk, @sdt, 0, 'User')";
                MySqlCommand cmd = new MySqlCommand(insert, conn);
                cmd.Parameters.AddWithValue("@ht", hoTen);
                cmd.Parameters.AddWithValue("@mk", matKhau);
                cmd.Parameters.AddWithValue("@sdt", soDienThoai);
                cmd.ExecuteNonQuery();
            }

            ViewBag.ThanhCong = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }
    }
}