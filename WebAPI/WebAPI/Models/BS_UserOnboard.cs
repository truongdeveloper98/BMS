using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class BS_UserOnboard : BaseModels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public int Position { get; set; }

        [StringLength(100)]
        public string Language { get; set; }

        [Required]
        public int Level { get; set; }

        [Required]
        public DateTime OnboardDate { get; set; }

        public string Note { get; set; }
    }
}
