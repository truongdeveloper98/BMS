using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebAPI.Models
{
    public class BS_Project : BaseModels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }
        public int? CustomerId { get; set; }

        public int? PartnerId { get; set; }
        public string ProjectName { get; set; }
        public string Code { get; set; }

        public string BackLogLink { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Revenue { get; set; }
        public double PM_Estimate { get; set; }
        public double Brse_Estimate { get; set; }
        public double Comtor_Estimate { get; set; }
        public double Tester_Estimate { get; set; }
        public double Developer_Estimate { get; set; }
        public int Status { get; set; }

        public string Note { get; set; }

        public float ManMonth { get; set; }

        public int ProjectTypeId { get; set; }
        public virtual BS_ProjectType ProjectType { get; set; }

        public virtual ICollection<BS_UserProject> UserProjects { get; set; }
        public virtual ICollection<BS_Report> Reports { get; set; }

        public virtual BS_PartnerInfo Customer { get; set; }
        public virtual BS_PartnerInfo Partner { get; set; }

    }
}
