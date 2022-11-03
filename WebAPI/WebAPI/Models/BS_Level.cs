using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BS_Level : BaseModels
    {
        [Key]
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public virtual ICollection<BS_Recruitment> Recruitments { get; set; }
    }
}
