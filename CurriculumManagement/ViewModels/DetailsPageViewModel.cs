using CurriculumManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurriculumManagement.ViewModels
{
    public class DetailsPageViewModel
    {
        public EAForm WorkingCopyEAForm { get; set; }
        public EAForm ParentExtractedEAForm { get; set; }

        public bool CompareNullableDates(DateTime? dt1, DateTime? dt2)
        {
            if (dt1.HasValue && !dt2.HasValue)
                return false;
            else if (!dt1.HasValue && dt2.HasValue)
                return false;
            else if (!dt1.HasValue && !dt2.HasValue)
                return true;
            else if (((DateTime)dt1).Date != ((DateTime)dt2).Date)
                return false;
            else
                return true;
        }
    }
}