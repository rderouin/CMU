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
    public class EAFormViewModel
    {
        public int ID { get; set; }

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

        //Activity Types
        [Display(Name = "Activity Type(s)")]
        //public string ActivityType { get; set; }
        public ParentChildCategoriesViewModel ActivityTypesViewModel { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ActivityType
        {
            get
            {
                string returnString = "";
                string comma = "";
                if (this.ActivityTypesViewModel != null && this.ActivityTypesViewModel.SelectedChildrenList != null)
                {
                    foreach (var activitytypeString in this.ActivityTypesViewModel.SelectedChildrenList)
                    {
                        returnString += comma + activitytypeString;
                        comma = ";";
                    }
                }
                return returnString;
            }
        }

        [Display(Name = "Facilitator Type")]
        public string ActivityFacilitatorType { get; set; }

        public string OtherActivityFacilitatorType { get; set; }
        
        [Display(Name = "Facilitator Name(s)")]
        public string ActivityFacilitatorNames { get; set; }

        //public DepartmentList AvailableDepartments { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Abstract { get; set; }
        
        [Display(Name = "Learning Objectives")]
        public string LearningObjectives { get; set; }

        //[Display(Name = "Keyword(s)")]
        //[DisplayFormat(ConvertEmptyStringToNull = false)]
        //public string Keywords { get; set; }

        [Display(Name = "Keyword(s)")]
        public ParentChildCategoriesViewModel KeywordViewModel { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Keyword
        {
            get
            {
                string returnString = "";
                string comma = "";
                if (this.KeywordViewModel != null && this.KeywordViewModel.SelectedChildrenList != null)
                {
                    foreach (var KeywordString in this.KeywordViewModel.SelectedChildrenList)
                    {
                        returnString += comma + KeywordString;
                        comma = ";";
                    }
                }
                return returnString;
            }
        }

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
        
        //Themes
        [DisplayName("Theme(s)")]
        public ParentChildCategoriesViewModel ThemesViewModel { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Themes
        {
            get
            {
                string returnString = "";
                string comma = "";
                if (this.ThemesViewModel != null && this.ThemesViewModel.SelectedChildrenList != null)
                {
                    foreach (var themeString in this.ThemesViewModel.SelectedChildrenList)
                    {
                        returnString += comma + themeString;
                        comma = ";";
                    }
                }
                return returnString;
            }
        }

        //Formulary
        [DisplayName("Formulary")]
        public ParentChildCategoriesViewModel FormularyViewModel { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Formulary
        {
            get
            {
                string returnString = "";
                string comma = "";
                if (this.FormularyViewModel != null && this.FormularyViewModel.SelectedChildrenList != null)
                {
                    foreach (var drugString in this.FormularyViewModel.SelectedChildrenList)
                    {
                        returnString += comma + drugString;
                        comma = ";";
                    }
                }
                return returnString;
            }
        }

        public EAFormViewModel()
        {
            this.Abstract = "";
        }

        public void SetupMultiSelectViewModels(EAForm eaform)
        {
            //Activity Types -- has only 1 parent category, so we load this one without choices for category
            ActivityTypesViewModel = new ParentChildCategoriesViewModel("Activity Type(s)");
            //ActivityTypesViewModel.ParentCategoryList = ***NO NEED***
            //ActivityTypesViewModel.ChildValuesAJAXFetchURL = ***NO NEED***
            ActivityTypesViewModel.DefaultChildrenList = CurriculumManagement.Models.POCO.ActivityType.GetActivityTypes().Select(d => new { d.ChildItemName }).Distinct();

            //Keywords
            KeywordViewModel = new ParentChildCategoriesViewModel("Keyword(s)");
            //EAFormDBContext db = new EAFormDBContext();
            //KeywordViewModel.ParentCategoryList = new SelectList(db.Keywords, "ParentCategoryName", "ParentCategoryName"); 

            //Themes
            ThemesViewModel = new ParentChildCategoriesViewModel("Theme(s)");
            ThemesViewModel.ParentCategoryList = Theme.GetThemes().Select(d => new { d.ParentCategoryName }).Distinct();
            ThemesViewModel.ChildValuesAJAXFetchURL = "/EAForms/ThemesList/?selectedParent=";

            //Formulary
            FormularyViewModel = new ParentChildCategoriesViewModel("Formulary Item(s)");
            //FormularyViewModel.ParentCategoryList = Drug.GetDrugs().Select(d => new { d.ParentCategoryName }).Distinct();  ***NO NEED***
            //FormularyViewModel.ChildValuesAJAXFetchURL = "/EAForms/FormularyList/?selectedParent=";  ***NO NEED***
            FormularyViewModel.DefaultChildrenList = CurriculumManagement.Models.POCO.Drug.GetDrugs().Select(d => new { d.ChildItemName }).Distinct();

            //Map eaform data for these select lists if possible...
            //
            //Need to create collection of Activity Types based on string field from DB (delimited by comma)
            if (eaform != null && eaform.ActivityType != null && eaform.ActivityType.Trim() != "")
            {
                string[] activityTypeStrings = eaform.ActivityType.Split(',');
                List<ChildItem> activityTypesList = new List<ChildItem>();
                foreach (string activityTypeString in activityTypeStrings)
                {
                    activityTypesList.Add(new ChildItem(activityTypeString));
                }
                ActivityTypesViewModel.SelectedChildrenList = activityTypesList;
            }
            else
            {
                ActivityTypesViewModel.SelectedChildrenList = Enumerable.Empty<string>();
            }
            //New to create collection of Keywords based on string filed from DB (delimited by comma)
            if (eaform != null && eaform.Keywords != null && eaform.Keywords.Trim() != "")
            {
                string[] keywordStrings = eaform.Keywords.Split(';');
                List<ChildItem> keywordList = new List<ChildItem>();
                foreach (string keywordString in keywordStrings)
                {
                    keywordList.Add(new ChildItem(keywordString));
                }
                KeywordViewModel.SelectedChildrenList = keywordList;
            }
            else
            {
                KeywordViewModel.SelectedChildrenList = Enumerable.Empty<string>();
            }
            //Need to create collection of Themes based on string field from DB (delimited by comma)
            if (eaform != null && eaform.Themes != null && eaform.Themes.Trim() != "")
            {
                string[] themeStrings = eaform.Themes.Split(';');
                List<ChildItem> themesList = new List<ChildItem>();
                foreach (string themeString in themeStrings)
                {
                    themesList.Add(new ChildItem(themeString));
                }
                ThemesViewModel.SelectedChildrenList = themesList;
            }
            else
            {
                ThemesViewModel.SelectedChildrenList = Enumerable.Empty<string>();
            }

            //Need to create collection of Drugs based on string field from DB (delimited by comma)
            if (eaform != null && eaform.Formulary != null && eaform.Formulary.Trim() != "")
            {
                string[] drugStrings = eaform.Formulary.Split(';');
                List<ChildItem> drugsList = new List<ChildItem>();
                foreach (string drugString in drugStrings)
                {
                    drugsList.Add(new ChildItem(drugString));
                }
                FormularyViewModel.SelectedChildrenList = drugsList;
            }
            else
            {
                FormularyViewModel.SelectedChildrenList = Enumerable.Empty<string>();
            }

            

        }

    }

}