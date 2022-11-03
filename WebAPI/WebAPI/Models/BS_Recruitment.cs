using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BS_Recruitment : BaseModels
    {
        [Key]
        public int Id { get; set; }
        public double SalaryMin { get; set; }
        public double SalaryMax { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public DateTime DatePublish { get; set; }
        public DateTime DateOnBroad { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public int Result { get; set; }
        public int PositionId { get; set; }
        public int LevelId { get; set; }
        public int LanguageId { get; set; }
        public int FrameworkId { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual BS_Position Position { get; set; }
        public virtual BS_Level Level { get; set; }
        public virtual BS_Framework Framework { get; set; }
        public virtual BS_Language Language { get; set; }
    }
}
