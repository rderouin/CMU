using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Mvc;

namespace CurriculumManagement.Models
{
    public class EAForm
    {
        public int ID { get; set; }

        [StringLength(11, MinimumLength = 4)]
        [Required]
        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Course { get; set; }

        [Display(Name = "Block/Week Title")]
        public string BlockWeekTitle { get; set; }
        
        [Display(Name = "Activity Title")]
        public string ActivityTitle { get; set; }
        
        [Display(Name = "Activity Type")]
        public string ActivityType { get; set; }

        [Display(Name = "Facilitator Type")]
        public string ActivityFacilitatorType { get; set; }

        [Display(Name = "Other:")]
        public string OtherActivityFacilitatorType { get; set; }

        [Display(Name = "Facilitator Name(s)")]
        public string ActivityFacilitatorNames { get; set; }
        [Display(Name = "Facilitator Dept(s)")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ActivityFacilitatorDepartments { get; set; }

        
        [DefaultValue("")]
        public string Abstract { get; set; }
        
        [Display(Name = "Learning Objectives")]
        public string LearningObjectives { get; set; }
        public string Keywords { get; set; }

        //UBC Formulary
        public string Formulary { get; set; }

        //Themes
        public string Themes { get; set; }

        [Display(Name = "Activity Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        //Last time an Instructor reviewed and submitted the form
        [Display(Name = "Last Submitted")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastSubmitted { get; set; }

        //Last time ANYONE saved the form -- change or no change
        [Display(Name = "Last Updated")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime LastUpdated { get; set; }

        [Display(Name = "Submitted By")]
        public string InstructorSignature { get; set; }

        public EAFormStatus Status { get; set; }

        public EAForm ParentForm { get; set; }

        public List<EAFormSave> SaveHistory  { get; set; }

        public EAForm Copy()
        {
            EAForm copiedForm = new EAForm();
            copiedForm.AcademicYear = this.AcademicYear; 
            copiedForm.Course = this.Course;
            copiedForm.BlockWeekTitle = this.BlockWeekTitle;
            copiedForm.ActivityTitle = this.ActivityTitle;
            copiedForm.ActivityType = this.ActivityType;
            copiedForm.ActivityFacilitatorType = this.ActivityFacilitatorType;
            copiedForm.ActivityFacilitatorNames = this.ActivityFacilitatorNames;
            copiedForm.ActivityFacilitatorDepartments = this.ActivityFacilitatorDepartments;
            copiedForm.Abstract = this.Abstract;
            copiedForm.LearningObjectives = this.LearningObjectives;
            copiedForm.Keywords = this.Keywords;
            copiedForm.Themes = this.Themes;
            copiedForm.Formulary = this.Formulary;
            copiedForm.LastUpdated = this.LastUpdated;
            copiedForm.LastSubmitted = this.LastSubmitted;
            copiedForm.StartDate = this.StartDate;
            copiedForm.InstructorSignature = this.InstructorSignature;
            return copiedForm;
        }

        public EAForm()
        {
            this.SaveHistory = new List<EAFormSave>();
        }

        public void CreateSaveHistoryRecord(string userName)
        {
            this.SaveHistory.Add(new EAFormSave(this.Status, userName, DateTime.Now));
        }

    }

    public class EAFormDBContext : IdentityDbContext
    {
        public EAFormDBContext(): base("EAFormDBContext")
        {
        
        }

        public DbSet<EAForm> EAForms { get; set; }
        public DbSet<EAFormStatus> EAFormStatuses { get; set; }
        public DbSet<EAFormSave> EAFormStatusSaves { get; set; }
        public DbSet<Keywords> Keywords { get; set; }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
        //    //Fluent API
        //    modelBuilder.Entity<EAForm>()
        //        .HasOptional(a => a.SaveHistory)
        //        .WithOptionalDependent()
        //        .WillCascadeOnDelete(true);
            
        //}
    }
   
}