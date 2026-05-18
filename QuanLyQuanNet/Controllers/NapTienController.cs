using QuanLyQuanNet.DAL;
using System;
using System.Web.Mvc;
using MySqlConnector;

namespace QuanLyQuanNet.Controllers
{
    public class NapTienController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(double soTien)
        {
            // Kiểm tra đăng nhập
            if (Session["MaNguoiDung"] == null)
            {
                // Chuyển hướng về trang đăng nhập nếu chưa đăng nhập
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();

                // Cập nhật số dư người dùng
                var update = new MySqlCommand("UPDATE NguoiDung SET SoDu = SoDu + @tien WHERE MaNguoiDung = @id", conn);
                update.Parameters.AddWithValue("@tien", soTien);
                update.Parameters.AddWithValue("@id", maNguoiDung);
                update.ExecuteNonQuery();

                // Ghi vào bảng lịch sử nạp tiền (nếu có)
                var insert = new MySqlCommand("INSERT INTO LichSuNapTien (MaNguoiDung, SoTien, ThoiGian) VALUES (@nd, @tien, NOW())", conn);
                insert.Parameters.AddWithValue("@nd", maNguoiDung);
                insert.Parameters.AddWithValue("@tien", soTien);
                insert.ExecuteNonQuery();
            }

            // Thông báo thành công
            TempData["ThongBao"] = $"✅ Nạp {soTien:N0} VND thành công!";
            return RedirectToAction("Index");
        }
    }
}
