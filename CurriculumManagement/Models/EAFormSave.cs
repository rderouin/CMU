using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CurriculumManagement.Models
{
    public class EAFormSave
    {
        public int ID { get; set; }
        public EAFormStatus Status { get; set; }
        
        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        public EAFormSave()
        { }
        
        public EAFormSave(EAFormStatus status, string userId, DateTime timeStamp)
        {
            this.Status = status;
            this.UserId = userId;
            this.TimeStamp = timeStamp;
        }

    }
}