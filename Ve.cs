//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookingFilm
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ve
    {
        public int MaVe { get; set; }
        public Nullable<decimal> GiaVe { get; set; }
        public Nullable<int> MaPhim { get; set; }
        public Nullable<int> MaGhe { get; set; }
        public Nullable<int> MaKH { get; set; }
        public Nullable<System.DateTime> NgayDat { get; set; }
        public string MaDoAn { get; set; }
        public Nullable<decimal> TongTien { get; set; }
        public Nullable<int> MaSuKien { get; set; }
        public Nullable<decimal> ThanhTien { get; set; }
    
        public virtual DoAn DoAn { get; set; }
        public virtual Ghe Ghe { get; set; }
        public virtual KhachHang KhachHang { get; set; }
        public virtual Phim Phim { get; set; }
        public virtual SuKien SuKien { get; set; }
    }
}
