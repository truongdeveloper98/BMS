using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UsageHelper;

namespace WebAPI.Models.ViewModels
{
    public class PartnerViewModels
    {
        public int PartnerId { get; set; }

        [Required]
        [StringLength(255)]
        public string PartnerName { get; set; }

        public string Address { get; set; }

        public string Website { get; set; }

        public bool IsDelete { get; set; }
        public int Vote { get; set; }

        public string Note { get; set; }

    }

    public class CreatePartnerViewModels
    {
        public int PartnerId { get; set; }

        [Required]
        [StringLength(255)]
        public string PartnerName { get; set; }

        public string Address { get; set; }

        public string Website { get; set; }

        public int Vote { get; set; }

        public string Note { get; set; }

        public bool? IsPartner { get; set; }

    }
    public class PartnerQueryParameters : QueryParametersBS {
        public bool? IsPartner { get; set; }
    }
}
