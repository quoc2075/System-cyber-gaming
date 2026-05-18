using QuanLyQuanNet.DAL;
using System.Collections.Generic;
using System.Web.Mvc;
using QuanLyQuanNet.Models;
using System;
using MySqlConnector;

namespace QuanLyQuanNet.Controllers
{
    public class LichSuSuDungController : Controller
    {
        public ActionResult Index()
        {
            // Kiểm tra Session["MaNguoiDung"]
            if (Session["MaNguoiDung"] == null || !int.TryParse(Session["MaNguoiDung"].ToString(), out int maNguoiDung))
            {
                return RedirectToAction("Login", "Account");
            }

            List<LichSuSuDung> lichSu = new List<LichSuSuDung>();

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();

                // Cập nhật câu lệnh SQL để bao gồm cột 'TongTien'
                var cmd = new MySqlCommand(@"
                    SELECT ls.MaMay, ls.ThoiGianBatDau, ls.ThoiGianKetThuc, ls.TongTien
                    FROM LichSuSuDung ls
                    WHERE ls.MaNguoiDung = @nd
                    ORDER BY ls.ThoiGianBatDau DESC", conn);

                cmd.Parameters.AddWithValue("@nd", maNguoiDung);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lichSu.Add(new LichSuSuDung
                    {
                        MaMay = Convert.ToInt32(reader["MaMay"]),
                        ThoiGianBatDau = Convert.ToDateTime(reader["ThoiGianBatDau"]),
                        ThoiGianKetThuc = reader["ThoiGianKetThuc"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["ThoiGianKetThuc"]),
                        SoTien = reader["TongTien"] == DBNull.Value ? null : (double?)Convert.ToDouble(reader["TongTien"]) // Sử dụng cột 'TongTien'
                    });
                }
            }

            return View(lichSu);
        }
    }
}
