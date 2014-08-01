using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace CurriculumManagement.Models
{
    public class EAFormStatus
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        [Display(Name = "Status")]
        public string Name { get; set; }
        public string Description { get; set; }

        public EAFormStatus()
        { 
        }

        public EAFormStatus(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

    }
    
}