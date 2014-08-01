using CurriculumManagement.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace CurriculumManagement.ViewModels
{
    public class SearchViewModel
    {
        public PagedList.IPagedList<CurriculumManagement.Models.EAForm> ReturnedForms { get; set; }

        public int ResultsCount
        {
            get {
                if (this.ReturnedForms != null)
                    return this.ReturnedForms.Count;
                else
                    return 0;
            }
        }

        public int SelectedStatusValue { get; set; }

        [DisplayName("Status")]
        public ICollection<EAFormStatus> EAFormStatuses { get; set; }
        public EAForm HeaderForm { get; set; }
        public bool IsAdmin { get; set; }

        public SearchViewModel()
        {
            this.EAFormStatuses = new List<EAFormStatus>();
        }
    }
}