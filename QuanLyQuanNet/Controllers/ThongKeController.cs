using QuanLyQuanNet.DAL;
using System.Web.Mvc;
using MySqlConnector;
using System;

namespace QuanLyQuanNet.Controllers
{
    public class ThongKeController : Controller
    {
        public ActionResult Index(int? ngay, int? thang, int? nam)
        {
            // Gán ngày/tháng/năm hiện tại nếu không có
            DateTime now = DateTime.Now;
            ngay = ngay ?? now.Day;
            thang = thang ?? now.Month;
            nam = nam ?? now.Year;

            double tongNap = 0;
            double tongChi = 0;

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();

                // Tổng tiền đã nạp trong ngày
                var cmd1 = new MySqlCommand(@"
                    SELECT IFNULL(SUM(SoTien), 0)
                    FROM LichSuNapTien
                    WHERE DAY(ThoiGian) = @ngay AND MONTH(ThoiGian) = @thang AND YEAR(ThoiGian) = @nam", conn);
                cmd1.Parameters.AddWithValue("@ngay", ngay);
                cmd1.Parameters.AddWithValue("@thang", thang);
                cmd1.Parameters.AddWithValue("@nam", nam);
                tongNap = Convert.ToDouble(cmd1.ExecuteScalar());

                // Tổng tiền sử dụng máy trong ngày
                var cmd2 = new MySqlCommand(@"
                    SELECT IFNULL(SUM(TongTien), 0)
                    FROM LichSuSuDung
                    WHERE DAY(ThoiGianKetThuc) = @ngay AND MONTH(ThoiGianKetThuc) = @thang AND YEAR(ThoiGianKetThuc) = @nam", conn);
                cmd2.Parameters.AddWithValue("@ngay", ngay);
                cmd2.Parameters.AddWithValue("@thang", thang);
                cmd2.Parameters.AddWithValue("@nam", nam);
                tongChi = Convert.ToDouble(cmd2.ExecuteScalar());
            }

            // Truyền dữ liệu qua View
            ViewBag.Ngay = ngay;
            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            ViewBag.TongNap = tongNap;
            ViewBag.TongChi = tongChi;
            ViewBag.LoiNhuan = tongNap - tongChi;

            return View();
        }
    }
}
