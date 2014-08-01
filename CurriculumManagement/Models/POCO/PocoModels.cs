using CurriculumManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CurriculumManagement.Models.POCO
{
    public static class AcademicYear
    {
        private static List<SelectListItem> academicyears;
        public static IEnumerable<SelectListItem> GetAcademicYears {
            get{
                if (academicyears == null)
                {
                    academicyears = new List<SelectListItem>();
                    
                    string[] yearsAvailable;
                    if (WebConfigurationManager.AppSettings["AcademicYearsAvailable"] != null)
                        yearsAvailable = WebConfigurationManager.AppSettings["AcademicYearsAvailable"].ToString().Split(',');
                    else
                        throw new Exception("Could not find AcademicYearsAvailable in web.config");

                    foreach (string foundYear in yearsAvailable)
                    {
                        academicyears.Add(new SelectListItem() { Text = foundYear, Value = foundYear, Selected = false });
                    }

                }
                return academicyears;
            } 
        }
    }

    public static class DepartmentList
    { 
        private static List<SelectListItem> departments;
        public static IEnumerable<SelectListItem> GetDepartments {
            get
            {
                if (departments == null)
                {
                    departments = new List<SelectListItem>();
                 
                    string[] departmentsAvailable;
                    if (WebConfigurationManager.AppSettings["DepartmentList"] != null)
                        departmentsAvailable = WebConfigurationManager.AppSettings["DepartmentList"].ToString().Split(',');
                    else
                        throw new Exception("Could not find DepartmentList in web.config");

                    foreach (string foundDepartment in departmentsAvailable)
                    {
                        departments.Add(new SelectListItem() { Text = foundDepartment, Value = foundDepartment, Selected = false });
                    }
                }

                return departments;
            }
        }

    }

    public static class ActivityType
    {
        public static List<ChildItem> ActivityTypesList { get; set; }
        public static IQueryable<ChildItem> GetActivityTypes()
        {
            //Activity Types
            if (ActivityTypesList == null)
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<ChildItem>));
                System.IO.StreamReader file = new System.IO.StreamReader(HttpContext.Current.Server.MapPath(@"~\XMLDataSources\ActivityTypesList.xml"));
                ActivityTypesList = (List<ChildItem>)reader.Deserialize(file);
            }
            return ActivityTypesList.AsQueryable();
        }
    }

    public static class Theme
    {
        public static List<ChildItem> ThemesList { get; set; }
        public static IQueryable<ChildItem> GetThemes()
        {
            //Themes
            if (ThemesList == null)
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<ChildItem>));
                System.IO.StreamReader file = new System.IO.StreamReader(HttpContext.Current.Server.MapPath(@"~\XMLDataSources\ThemesList.xml"));
                ThemesList = (List<ChildItem>)reader.Deserialize(file);
            }

            return ThemesList.AsQueryable();
        }
    }

    public static class Drug
    {
        public static List<ChildItem> DrugsList { get; set; }
        public static IQueryable<ChildItem> GetDrugs()
        {
            if (DrugsList == null)
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<ChildItem>));
                System.IO.StreamReader file = new System.IO.StreamReader(HttpContext.Current.Server.MapPath(@"~\XMLDataSources\FormularyList.xml"));
                DrugsList = (List<ChildItem>)reader.Deserialize(file);

                ////DEBUG TESTING -- write to XML file
                //System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<Drug>));
                //StreamWriter fileWriter = new System.IO.StreamWriter(Server.MapPath(@"~\XMLDataSources\FormularyList.xml"));
                //writer.Serialize(fileWriter, Drug.DrugsList);
                //file.Close();

            }
            return DrugsList.AsQueryable();
        }
    }
}