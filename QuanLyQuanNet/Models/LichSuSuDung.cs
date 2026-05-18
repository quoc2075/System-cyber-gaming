using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyQuanNet.Models
{
    public class LichSuSuDung
    {
        public int MaLichSu { get; set; }
        public int MaNguoiDung { get; set; }
        public int MaMay { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public double? SoTien { get; set; }

    }
}