using System.ComponentModel.DataAnnotations;

namespace DailyDo.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Modification Date")]
        public DateTime ModificationDate { get; set; }

        [Required]
        public bool IsDone { get; set; }

        public Category Category { get; set; }
    }
}
