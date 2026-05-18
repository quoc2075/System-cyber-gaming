using System.Collections.Generic;
using System.Web.Mvc;
using QuanLyQuanNet.Models;
using QuanLyQuanNet.DAL;
using MySqlConnector;

namespace QuanLyQuanNet.Controllers
{
    public class MayController : Controller
    {
        public ActionResult Index()
        {
            if (Session["VaiTro"]?.ToString() != "Admin")
                return RedirectToAction("Login", "NguoiDung");

            List<MayTinh> danhSach = new List<MayTinh>();

            using (MySqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM MayTinh";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    danhSach.Add(new MayTinh
                    {
                        MaMay = (int)reader["MaMay"],
                        TenMay = reader["TenMay"].ToString(),
                        TrangThai = reader["TrangThai"].ToString(),
                        DonGia = (double)reader["DonGia"]
                    });
                }
            }

            return View(danhSach);
        }

        public ActionResult Them()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Them(MayTinh mt)
        {
            using (MySqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string insert = "INSERT INTO MayTinh (TenMay, TrangThai, DonGia) VALUES (@ten, 'Rảnh', @gia)";
                MySqlCommand cmd = new MySqlCommand(insert, conn);
                cmd.Parameters.AddWithValue("@ten", mt.TenMay);
                cmd.Parameters.AddWithValue("@gia", mt.DonGia);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Sua(int id)
        {
            MayTinh mt = new MayTinh();

            using (MySqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM MayTinh WHERE MaMay = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    mt.MaMay = id;
                    mt.TenMay = reader["TenMay"].ToString();
                    mt.TrangThai = reader["TrangThai"].ToString();
                    mt.DonGia = (double)reader["DonGia"];
                }
            }

            return View(mt);
        }

        [HttpPost]
        public ActionResult Sua(MayTinh mt)
        {
            using (MySqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();
                string update = "UPDATE MayTinh SET TenMay = @ten, TrangThai = @tt, DonGia = @gia WHERE MaMay = @id";
                MySqlCommand cmd = new MySqlCommand(update, conn);
                cmd.Parameters.AddWithValue("@ten", mt.TenMay);
                cmd.Parameters.AddWithValue("@tt", mt.TrangThai);
                cmd.Parameters.AddWithValue("@gia", mt.DonGia);
                cmd.Parameters.AddWithValue("@id", mt.MaMay);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Xoa(int id)
        {
            using (MySqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                // Xóa dữ liệu liên quan trong bảng LichSuSuDung
                string deleteLichSu = "DELETE FROM LichSuSuDung WHERE MaMay = @id";
                MySqlCommand cmdLichSu = new MySqlCommand(deleteLichSu, conn);
                cmdLichSu.Parameters.AddWithValue("@id", id);
                cmdLichSu.ExecuteNonQuery();

                // Xóa máy tính trong bảng MayTinh
                string deleteMayTinh = "DELETE FROM MayTinh WHERE MaMay = @id";
                MySqlCommand cmdMayTinh = new MySqlCommand(deleteMayTinh, conn);
                cmdMayTinh.Parameters.AddWithValue("@id", id);
                cmdMayTinh.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

    }
}
