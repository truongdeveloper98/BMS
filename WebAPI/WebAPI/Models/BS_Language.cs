using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BS_Language : BaseModels
    {
        [Key]
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public virtual ICollection<BS_Recruitment> Recruitments { get; set; }
    }
}
