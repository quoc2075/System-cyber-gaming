using System.Collections.Generic;
using System.Web.Mvc;
using QuanLyQuanNet.DAL;
using QuanLyQuanNet.Models;
using MySqlConnector;

namespace QuanLyQuanNet.Controllers
{
    public class AdminNguoiDungController : Controller
    {
        // Hiển thị danh sách người dùng
        public ActionResult Index()
        {
            var ds = new List<NguoiDung>();

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM NguoiDung", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ds.Add(new NguoiDung
                    {
                        MaNguoiDung = reader.GetInt32("MaNguoiDung"),
                        HoTen = reader.GetString("HoTen"),
                        MatKhau = reader.GetString("MatKhau"),
                        SoDienThoai = reader.GetString("SoDienThoai"),
                        SoDu = reader.GetDouble("SoDu"),
                        VaiTro = reader.GetString("VaiTro")
                    });
                }
            }

            return View(ds);
        }

        // Xem form sửa người dùng
        public ActionResult Sua(int id)
        {
            NguoiDung nd = null;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM NguoiDung WHERE MaNguoiDung = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    nd = new NguoiDung
                    {
                        MaNguoiDung = reader.GetInt32("MaNguoiDung"),
                        HoTen = reader.GetString("HoTen"),
                        MatKhau = reader.GetString("MatKhau"),
                        SoDienThoai = reader.GetString("SoDienThoai"),
                        SoDu = reader.GetDouble("SoDu"),
                        VaiTro = reader.GetString("VaiTro")
                    };
                }
            }

            return View(nd);
        }

        // Xử lý cập nhật người dùng
        [HttpPost]
        public ActionResult Sua(NguoiDung nd)
        {
            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(@"
                    UPDATE NguoiDung SET
                        HoTen = @HoTen,
                        MatKhau = @MatKhau,
                        SoDienThoai = @SoDienThoai,
                        SoDu = @SoDu,
                        VaiTro = @VaiTro
                    WHERE MaNguoiDung = @MaNguoiDung", conn);

                cmd.Parameters.AddWithValue("@HoTen", nd.HoTen);
                cmd.Parameters.AddWithValue("@MatKhau", nd.MatKhau);
                cmd.Parameters.AddWithValue("@SoDienThoai", nd.SoDienThoai);
                cmd.Parameters.AddWithValue("@SoDu", nd.SoDu);
                cmd.Parameters.AddWithValue("@VaiTro", nd.VaiTro);
                cmd.Parameters.AddWithValue("@MaNguoiDung", nd.MaNguoiDung);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // Đổi vai trò
        

        // Xóa người dùng
        public ActionResult Xoa(int id)
        {
            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM NguoiDung WHERE MaNguoiDung = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}
