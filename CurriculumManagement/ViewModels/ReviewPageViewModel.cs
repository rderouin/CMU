using CurriculumManagement.Helpers;
using CurriculumManagement.Models;
using CurriculumManagement.Models.POCO;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CurriculumManagement.ViewModels
{
    public class ReviewPageViewModel : EAFormViewModel
    {
        [Required]
        [Display(Name = "Facilitator Dept(s)")]
        public string ActivityFacilitatorDepartments { get; set; }

        [Display(Name = "Submitted By")]
        [Required(AllowEmptyStrings = false)]
        public string InstructorSignature { get; set; }

        [Display(Name = "Learning Objectives")]
        [Required(AllowEmptyStrings = false)]
        public string LearningObjectives { get; set; }

        public EAFormStatus Status { get; set; }
        public int SelectedStatusValue { get; set; }

        public ReviewPageViewModel()
        {
            //this.AvailableDepartments = new Department();
        }

    }

}