using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace CurriculumManagement.Models
{
    public class Keywords
    {
        public int ID { get; set; }
        [Display(Name = "ParentCategoryName")]
        [Required]
        public string Name { get; set; }
        public Keywords()
        {
        }
        public Keywords(int id, string name)
        {   
            this.ID = id;
            this.Name = name;
        }
    }
}