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
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace CurriculumManagement.ViewModels
{
    public class EditPageViewModel: EAFormViewModel
    {
        [Display(Name = "Facilitator Dept(s)")]
        public string ActivityFacilitatorDepartments { get; set; }

        [Display(Name = "Submitted By")]
        public string InstructorSignature { get; set; }

        public EAFormStatus Status { get; set; }
        public int SelectedStatusValue { get; set; }

        [DisplayName("Status")]
        public IEnumerable<EAFormStatus> EAFormStatuses { get; set; }

        public List<EAFormSave> SaveHistory { get; set; }

        public EditPageViewModel()
        {
            //this.AvailableAcademicYears = new AcademicYear();
            //this.AvailableDepartments = new Department();
            this.SaveHistory = new List<EAFormSave>();
        }

    }

}