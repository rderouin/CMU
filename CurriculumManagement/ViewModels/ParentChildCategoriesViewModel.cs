using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CurriculumManagement.ViewModels
{
    public class ParentChildCategoriesViewModel
    {
        public string Label { get; set; }
        public string ChildValuesAJAXFetchURL { get; set; }
        public IEnumerable<object> ParentCategoryList { get; set; }
        public IEnumerable<object> SelectedChildrenList { get; set; }
        
        //When there is only 1 parent/main category,e pre-populate the children
        //items available for selection in the following list
        public IEnumerable<object> DefaultChildrenList { get; set; }

        public ParentChildCategoriesViewModel()
        {
        }
        
        public ParentChildCategoriesViewModel(string label)
        {
            this.Label = label;
            this.DefaultChildrenList = new List<ChildItem>();
        }

        public SelectList ParentCategorySelectList(string dataValueField, string dataTextField, string defaultValue, string defaultText)
        {
            if (this.ParentCategoryList != null)
            {
                var list = this.ParentCategoryList.ToList();
                var firstItem = new { ParentCategoryName = defaultText };
                list.Insert(0, firstItem);
                return new SelectList(list, dataValueField, dataTextField);
            }
            else
                return new SelectList(new List<ChildItem>());
        }
    }

    public class ChildItem
    {
        public string ParentCategoryName { get; set; }
        public string ChildItemName { get; set; }

        public ChildItem()
        { 
        }

        public ChildItem(string childItemName)
        {
            this.ChildItemName = childItemName;
        }
    }


}