using QuanLyQuanNet.Models;
using QuanLyQuanNet.DAL;
using MySqlConnector;
using System;
using System.Web.Mvc;

namespace QuanLyQuanNet.Controllers
{
    public class SuDungMayController : Controller
    {
        public ActionResult ChonMay(int id)
        {
            // Kiểm tra phiên đăng nhập
            if (Session["MaNguoiDung"] == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);

            using (MySqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                // Cập nhật trạng thái máy
                var updateMay = "UPDATE MayTinh SET TrangThai = 'Đang sử dụng' WHERE MaMay = @ma";
                using (var cmd1 = new MySqlCommand(updateMay, conn))
                {
                    cmd1.Parameters.AddWithValue("@ma", id);
                    cmd1.ExecuteNonQuery();
                }

                // Ghi lịch sử sử dụng
                var insertLichSu = @"INSERT INTO LichSuSuDung(MaNguoiDung, MaMay, ThoiGianBatDau)
                                     VALUES(@nd, @may, NOW())";
                using (var cmd2 = new MySqlCommand(insertLichSu, conn))
                {
                    cmd2.Parameters.AddWithValue("@nd", maNguoiDung);
                    cmd2.Parameters.AddWithValue("@may", id);
                    cmd2.ExecuteNonQuery();
                }
            }

            TempData["ThongBao"] = "✅ Đã bắt đầu sử dụng máy!";
            return RedirectToAction("Index", "User");
        }

        public ActionResult KetThuc()
        {
            if (Session["MaNguoiDung"] == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            int maNguoiDung = Convert.ToInt32(Session["MaNguoiDung"]);

            using (MySqlConnection conn = DbConnection.GetConnection())
            {
                conn.Open();

                // Lấy phiên đang dùng
                var select = @"SELECT * FROM LichSuSuDung 
                               WHERE MaNguoiDung = @nd AND ThoiGianKetThuc IS NULL
                               ORDER BY ThoiGianBatDau DESC LIMIT 1";
                using (var cmd = new MySqlCommand(select, conn))
                {
                    cmd.Parameters.AddWithValue("@nd", maNguoiDung);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int ID = Convert.ToInt32(reader["ID"]);
                            int maMay = Convert.ToInt32(reader["MaMay"]);
                            DateTime start = Convert.ToDateTime(reader["ThoiGianBatDau"]);
                            reader.Close();

                            TimeSpan duration = DateTime.Now - start;
                            double soPhut = duration.TotalMinutes;
                            double donGia = 0;

                            // Lấy đơn giá máy
                            var cmdGia = new MySqlCommand("SELECT DonGia FROM MayTinh WHERE MaMay = @m", conn);
                            cmdGia.Parameters.AddWithValue("@m", maMay);
                            object result = cmdGia.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                            {
                                donGia = Convert.ToDouble(result);
                            }

                            double thanhTien = Math.Round(donGia * (soPhut / 60), 0);

                            // Trừ tiền
                            var truTien = new MySqlCommand("UPDATE NguoiDung SET SoDu = SoDu - @tien WHERE MaNguoiDung = @nd", conn);
                            truTien.Parameters.AddWithValue("@tien", thanhTien);
                            truTien.Parameters.AddWithValue("@nd", maNguoiDung);
                            truTien.ExecuteNonQuery();

                            // Cập nhật lịch sử
                            var updateLichSu = @"UPDATE LichSuSuDung 
                                                 SET ThoiGianKetThuc = NOW(), TongTien = @tien 
                                                 WHERE ID = @ma";
                            var update = new MySqlCommand(updateLichSu, conn);
                            update.Parameters.AddWithValue("@tien", thanhTien);
                            update.Parameters.AddWithValue("@ma", ID);
                            update.ExecuteNonQuery();

                            // Cập nhật trạng thái máy về rảnh
                            var updateMay = new MySqlCommand("UPDATE MayTinh SET TrangThai = 'Rảnh' WHERE MaMay = @m", conn);
                            updateMay.Parameters.AddWithValue("@m", maMay);
                            updateMay.ExecuteNonQuery();

                            TempData["ThongBao"] = $"✅ Kết thúc: {Math.Ceiling(soPhut)} phút. Trừ {thanhTien:N0} VND.";
                        }
                        else
                        {
                            TempData["ThongBao"] = "⚠️ Không tìm thấy phiên sử dụng đang hoạt động!";
                        }
                    }
                }
            }

            return RedirectToAction("Index", "User");
        }
    }
}
