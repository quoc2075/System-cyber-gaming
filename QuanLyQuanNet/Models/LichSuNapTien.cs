using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyQuanNet.Models
{
    public class LichSuNapTien
    {
        public int MaNap { get; set; }
        public int MaNguoiDung { get; set; }
        public double SoTien { get; set; }
        public DateTime ThoiGian { get; set; }
    }
}