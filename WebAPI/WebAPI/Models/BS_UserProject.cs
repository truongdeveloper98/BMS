using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BS_UserProject : BaseModels
    {
        [Key, Column(Order = 0)]
        public string UserId { get; set; }
        [Key, Column(Order = 1)]
        public int ProjectId { get; set; }
        [Key, Column(Order = 2)]
        public int PositionId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual BS_Project Project { get; set; }
        public virtual BS_Position Position { get; set; }

    }
}
