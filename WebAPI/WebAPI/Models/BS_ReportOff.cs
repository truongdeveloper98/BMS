using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BS_ReportOff : BaseModels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportOffId { get; set; }

        public DateTime OffDateStart { get; set; }
        public DateTime OffDateEnd { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public byte OffTypeId { get; set; }
        public virtual BS_OffType OffType { get; set; }
        public string Note { get; set; }

        public string UserId { get; set; }

        public double OffDay { get; set; }
        public virtual ApplicationUser User { get; set; }
        
    }
}
