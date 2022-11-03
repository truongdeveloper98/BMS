using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BS_Report : BaseModels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportId { get; set; }

        public DateTime WorkingDay { get; set; }
        public float WorkingHour { get; set; }
        public float RateValue { get; set; }
        public byte WorkingType { get; set; }

        /// <summary>
        /// =0: Normal; =1: OT
        /// </summary>
        public byte ReportType { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int ProjectId { get; set; }
        public virtual BS_Project Project { get; set; }

        public int PositionId { get; set; }
        public virtual BS_Position Position { get; set; }
    }
}