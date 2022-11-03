using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BS_ProjectType : BaseModels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectTypeId { get; set; }

        public string ProjectType_Name { get; set; }

        public virtual ICollection<BS_Project> Projects { get; set; }
    }
}
