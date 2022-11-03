using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.ViewModels
{
    public class LeaveDayVM
    {
        public string UserId { get; set; }
        public double ThisYearTotal { get; set; }
        public double LastYearRemain { get; set; }
        public double Occupied { get; set; }
        public double Avaiable { get;  set; } // = thisMonth + LastYearRemain - Occupied
        //số ngày được nghỉ = phép còn năm ngoái + Phép chưa nghỉ năm nay
        //phép chưa nghỉ năm nay = Số ngày được nghỉ trong tháng - số ngày đã nghỉ
        //số ngày được nghỉ trong tháng =  Số tháng - (12 - ThisYearTotal)

    }
}
