using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BS_Position : BaseModels
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public virtual ICollection<BS_UserProject> UserProjects { get; set; }
        public virtual ICollection<BS_Report> Reports { get; set; }
        public virtual ICollection<BS_Recruitment> Recruitments { get; set; }
        public virtual BS_UserInfo UserInfo { get; set; }
    }
}
