using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class BS_Framework : BaseModels
    {
        [Key]
        public int FrameworkId { get; set; }
        public string FrameworkName { get; set; }
        public virtual ICollection<BS_Recruitment> Recruitments { get; set; }

    }
}
