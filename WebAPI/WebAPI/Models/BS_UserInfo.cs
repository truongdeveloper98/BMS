using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BS_UserInfo: BaseModels
    {
        [Key]
        public string UserId { get; set; }

        public string Position { get; set; }
        public string Level { get; set; }
        public int Department { get; set; }
        public string Team { get; set; }
        public bool IsPending { get; set; }
        public int EffortFree { get; set; }
        public DateTime? PendingStart { get; set; }
        public DateTime? PendingEnd { get; set; }
        public double ThisYearLeaveDay { get; set; }
        public double LastYearLeaveDay { get; set; }
        public double OccupiedLeaveDay { get; set; }
        public int TypeId { get; set; }
        public string Company { get; set; }
        public int CompanyId { get; set; }
        public string CVLink { get; set; }
        public BS_UserType UserType { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
