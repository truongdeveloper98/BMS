using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class BS_UserSalaries: BaseModels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public double HourlySalary { get; set; }

        public DateTime EffectiveDate { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
