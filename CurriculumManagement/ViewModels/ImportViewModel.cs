using CurriculumManagement.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CurriculumManagement.ViewModels
{
    public class ImportViewModel
    {
        [Required, HttpPostedFileExtensions(Extensions = "xlsx,xls", ErrorMessage = "Specify an Excel file.")]
        public HttpPostedFileBase File { get; set; }

        public List<string> ImportResults { get; set; }

        public ImportViewModel()
        {
            this.ImportResults = new List<string>();
        }

        public void Log(string line)
        {
            this.ImportResults.Add(line);
        }
    }
}