using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class BS_PartnerInfo: BaseModels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PartnerId { get; set; }

        [Required]
        [StringLength(255)]
        public string PartnerName { get; set; }

        public string Address { get; set; }

        public string Website { get; set; }

        public bool? IsPartner { get; set; }

        public bool? IsCustomer { get; set; }
        public int Vote { get; set; }

        public string Note { get; set; }

        public virtual ICollection<BS_Project> CustomerProjects { get; set; }

        public virtual ICollection<BS_Project> PartnerProjects { get; set; }
    }
}
