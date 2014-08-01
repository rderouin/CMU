using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CurriculumManagement.Models;
using CurriculumManagement.ViewModels;
using AutoMapper;
using CurriculumManagement.Helpers;
using PagedList;
using CurriculumManagement.Models.POCO;

namespace CurriculumManagement.Controllers
{
    public class EAFormsController : Controller
    {
        private EAFormDBContext db = new EAFormDBContext();

        // GET: /EAForms/
        //[CustomAuthorize(Roles = "Program Assistant,Admin,Curriculum Materials Coordinator")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string SearchDateFrom, string SearchDateTo, string SelectedStatusValue, bool? hideExtractedOriginals, SearchViewModel searchViewModel, int? page, string academicyear)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CourseSortParm = String.IsNullOrEmpty(sortOrder) ? "Course_desc" : "";
            ViewBag.BlockWeekTitleSortParm = sortOrder == "BlockWeekTitle" ? "BlockWeekTitle_desc" : "BlockWeekTitle";
            ViewBag.ActivityTitleSortParm = sortOrder == "ActivityTitle" ? "ActivityTitle_desc" : "ActivityTitle";
            ViewBag.ActivityTypeSortParm = sortOrder == "ActivityType" ? "ActivityType_desc" : "ActivityType";
            ViewBag.StartDateSortParm = sortOrder == "StartDate" ? "StartDate_desc" : "StartDate";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "Status_desc" : "Status";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentDateFrom = SearchDateFrom;
            ViewBag.CurrentDateTo = SearchDateTo;
            ViewBag.CurrentStatusValue = SelectedStatusValue; // searchViewModel.SelectedStatusValue;

            var forms = from f in db.EAForms.Include("Status")
                        select f;

            if (!String.IsNullOrEmpty(searchString))
            {
                forms = forms.Where(s => s.Course.Contains(searchString) || s.BlockWeekTitle.Contains(searchString) || s.ActivityTitle.Contains(searchString) || s.ActivityType.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(SearchDateFrom))
            {
                //DateTime dt = Convert.ToDateTime(SearchDate);
                IFormatProvider culture = new System.Globalization.CultureInfo("en", true);
                DateTime dt2 = DateTime.Parse(SearchDateFrom, culture, System.Globalization.DateTimeStyles.AssumeLocal);
                forms = forms.Where(s => s.StartDate >= dt2);
            }
            if (!String.IsNullOrEmpty(SearchDateTo))
            {
                IFormatProvider culture = new System.Globalization.CultureInfo("en", true);
                DateTime dt2 = DateTime.Parse(SearchDateTo, culture, System.Globalization.DateTimeStyles.AssumeLocal);
                //Need to add a day, less a millisecond to get to the END of the day
                dt2 = dt2.AddDays(1);
                dt2 = dt2.AddMilliseconds(-1);
                forms = forms.Where(s => s.StartDate <= dt2);
            }
            if (searchViewModel.SelectedStatusValue > 0)
            {
                forms = forms.Where(s => s.Status.ID == searchViewModel.SelectedStatusValue);
            }
            if (hideExtractedOriginals == null || hideExtractedOriginals == true) //Is null by default and checked!
            {
                forms = forms.Where(s => s.Status.Name.ToLower() != "one45 extract");
            }

            //Ordering
            switch (sortOrder)
            {
                case "Course_desc":
                    forms = forms.OrderByDescending(s => s.Course);
                    break;
                case "BlockWeekTitle":
                    forms = forms.OrderBy(s => s.BlockWeekTitle);
                    break;
                case "BlockWeekTitle_desc":
                    forms = forms.OrderByDescending(s => s.BlockWeekTitle);
                    break;
                case "ActivityTitle":
                    forms = forms.OrderBy(s => s.ActivityTitle);
                    break;
                case "ActivityTitle_desc":
                    forms = forms.OrderByDescending(s => s.ActivityTitle);
                    break;
                case "ActivityType":
                    forms = forms.OrderBy(s => s.ActivityType);
                    break;
                case "ActivityType_desc":
                    forms = forms.OrderByDescending(s => s.ActivityType);
                    break;
                case "StartDate":
                    forms = forms.OrderBy(s => s.StartDate);
                    break;
                case "StartDate_desc":
                    forms = forms.OrderByDescending(s => s.StartDate);
                    break;
                case "Status":
                    forms = forms.OrderBy(s => s.Status.Name);
                    break;
                case "Status_desc":
                    forms = forms.OrderByDescending(s => s.Status.Name);
                    break;
                default:
                    forms = forms.OrderBy(s => s.Course);
                    break;
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            SearchViewModel searchviewmodel = new SearchViewModel();
            searchviewmodel.ReturnedForms = forms.ToPagedList(pageNumber, pageSize);
            searchviewmodel.EAFormStatuses.Add(new EAFormStatus(-1, "All"));
            foreach (EAFormStatus status in db.EAFormStatuses)
                searchviewmodel.EAFormStatuses.Add(status);
            searchviewmodel.IsAdmin = ActionLinkHelper.IsInRole("Admin");
            return View(searchviewmodel);

        }
        
        [CustomAuthorize(Roles = "Program Assistant,Admin,Curriculum Materials Coordinator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DetailsPageViewModel detailsViewModel = new DetailsPageViewModel();

            //Loading related entities (Status) ** Default is not to load related data
            //
            //Method 1: Enable lazy loading
            //db.Configuration.LazyLoadingEnabled = true;
            //string statusName = eaform.Status.Name;
            //
            //Methed 2: Eager loading using Include()
            //EAForm eaform = db.EAForms
            //                    .Where(f => f.ID == id)
            //                    .Include(f => f.Status)
            //                    .FirstOrDefault();
            //Method 3: Explicit loading using LoadProperty() method ** Only available via heavier ObjectContext
            EAForm eaform = db.EAForms.Find(id);
            if (eaform == null)
            {
                return HttpNotFound();
            }
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.LoadProperty(eaform, i => i.Status);
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.LoadProperty(eaform, i => i.ParentForm);
            detailsViewModel.WorkingCopyEAForm = eaform;
            
            if (eaform.ParentForm != null)
            {
                EAForm parentForm = db.EAForms.Find(eaform.ParentForm.ID);
                if (parentForm == null)
                {
                    return HttpNotFound();
                }
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.LoadProperty(parentForm, i => i.Status);

                detailsViewModel.ParentExtractedEAForm = parentForm;
            }
            else
            { 
                //No parent form = one45 original extract form
                detailsViewModel.ParentExtractedEAForm = null;
            }

            return View(detailsViewModel);
        }

        // GET: /EAForms/Create
        [CustomAuthorize(Roles = "Admin,Program Assistant")]
        public ActionResult Create()
        {
            //
            //Load ViewModel
            //
            
            //Statuses
            var editPageViewModel = new EditPageViewModel();
            editPageViewModel.EAFormStatuses = db.EAFormStatuses;
            editPageViewModel.LastUpdated = DateTime.Today;
            editPageViewModel.StartDate = null;
            editPageViewModel.LastSubmitted = null;

            editPageViewModel.SetupMultiSelectViewModels(null);

            return View(editPageViewModel);
        }

        // POST: /EAForms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Status")] EditPageViewModel editPageViewModel)
        {
            EAForm eaform = new EAForm();
            if (ModelState.IsValid)
            {
                eaform = Mapper.Map<EAFormViewModel, EAForm>(editPageViewModel, eaform);
                eaform.Status = db.EAFormStatuses.Find(editPageViewModel.SelectedStatusValue);
                eaform.LastUpdated = DateTime.Now;
                eaform.CreateSaveHistoryRecord(User.Identity.Name);

                //Need to create a parent one45 extract copy to compare to in the future
                EAForm eaformCopy = eaform.Copy();
                eaformCopy.Status = db.EAFormStatuses
                                    .Where(f => f.Name == "one45 Extract")
                                    .FirstOrDefault();
                eaformCopy.CreateSaveHistoryRecord(User.Identity.Name);

                //Link the copy to the original
                eaform.ParentForm = eaformCopy;

                try {
                    db.EAForms.Add(eaform);
                    db.EAForms.Add(eaformCopy);
                    db.SaveChanges();
                    ViewBag.SuccessMessage = "Created successfully";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("CreateForm", "Form could not be created"); //ErrorMessages.SaveError);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }

            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    if (error.Exception != null)
                        Elmah.ErrorSignal.FromCurrentContext().Raise(error.Exception);
                    else
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(error.ErrorMessage));
                }
                
            }

            editPageViewModel = Mapper.Map<EAForm, EditPageViewModel>(eaform, editPageViewModel); 
            editPageViewModel.EAFormStatuses = db.EAFormStatuses;
            editPageViewModel.SetupMultiSelectViewModels(eaform);

            return View(editPageViewModel);
          
        }

        public JsonResult ActivityTypesList(string selectedParent)
        {
            var activitytypes = from s in ActivityType.GetActivityTypes()
                         where s.ParentCategoryName == selectedParent
                         select s;

            return Json(new SelectList(activitytypes.ToArray(), "ChildItemName", "ChildItemName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ThemesList(string selectedParent)
        {
            var themes = from s in Theme.GetThemes()
                         where s.ParentCategoryName == selectedParent
                         select s;

            return Json(new SelectList(themes.ToArray(), "ChildItemName", "ChildItemName"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FormularyList(string selectedParent)
        {
            var drugs = from s in Drug.GetDrugs()
                        where s.ParentCategoryName == selectedParent
                        select s;

            return Json(new SelectList(drugs.ToArray(), "ChildItemName", "ChildItemName"), JsonRequestBehavior.AllowGet);
        }

        //UNAUTHENTICATED to allow instructors access
        //We use the same view with some minor logic to change the look/functionality
        //This allows us to avoid duplicating the view for another user
        public ActionResult Review(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var viewmodel = new ReviewPageViewModel();

            EAForm eaform = db.EAForms.Find(id);
            if (eaform == null)
            {
                return HttpNotFound();
            }
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.LoadProperty(eaform, i => i.Status);

            //
            //Load ViewModel
            //

            //Statuses

            viewmodel = Mapper.Map<EAForm, ReviewPageViewModel>(eaform, viewmodel);
            viewmodel.SetupMultiSelectViewModels(eaform);

            return View("Review", "_UnauthLayout", viewmodel);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[MultipleButton(Name = "Review", Argument = "Submit")]
        //public ActionResult ReviewSubmit(ReviewPageViewModel viewmodel)
        //{
        //    EAForm eaform = db.EAForms.Find(viewmodel.ID);
        //    if (eaform == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        eaform = Mapper.Map<ReviewPageViewModel, EAForm>(viewmodel, eaform);
        //        //Set the status and timestamp(s)
        //        eaform.Status = db.EAFormStatuses.First(p => p.Name == "Submitted");
        //        eaform.LastUpdated = DateTime.Now; 
        //        eaform.LastSubmitted = DateTime.Now;
        //        eaform.CreateSaveHistoryRecord(viewmodel.InstructorSignature);
        //        db.Entry(eaform).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return Content("submit:Thank you for submitting your review", "text/html");
        //    }
        //    else
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors);
        //        foreach (var error in errors)
        //        {
        //            if (error.Exception != null)
        //                Elmah.ErrorSignal.FromCurrentContext().Raise(error.Exception);
        //            else
        //                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(error.ErrorMessage));
        //        }
        //        return Content("submit:Review could not be submitted", "text/html");
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[MultipleButton(Name = "Review", Argument = "Save")]
        public ActionResult Review(ReviewPageViewModel viewmodel, string Command)
        {
           if(Command == "Save")
           {
                EAForm eaform = db.EAForms.Find(viewmodel.ID);
                if (eaform == null)
                {
                    return HttpNotFound();
                }

                if (ModelState.IsValid)
                {
                    eaform = Mapper.Map<ReviewPageViewModel, EAForm>(viewmodel, eaform);
                    //Set the status and timestamp(s)
                    eaform.Status = db.EAFormStatuses.First(p => p.Name == "Draft");
                    eaform.LastUpdated = DateTime.Now;
                    eaform.CreateSaveHistoryRecord(viewmodel.InstructorSignature);
                    db.Entry(eaform).State = EntityState.Modified;
                    db.SaveChanges();
                    return Content("save:Saved", "text/html");
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        if (error.Exception != null)
                            Elmah.ErrorSignal.FromCurrentContext().Raise(error.Exception);
                        else
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(error.ErrorMessage));
                    }
                    return Content("save:Review could not be saved", "text/html");
                }
            }
           else if (Command == "Search")
           {
               return Content("search:Keywords ", "text/html");
           }
           else
           {
               EAForm eaform = db.EAForms.Find(viewmodel.ID);
               if (eaform == null)
               {
                   return HttpNotFound();
               }

               if (ModelState.IsValid)
               {
                   eaform = Mapper.Map<ReviewPageViewModel, EAForm>(viewmodel, eaform);
                   //Set the status and timestamp(s)
                   eaform.Status = db.EAFormStatuses.First(p => p.Name == "Submitted");
                   eaform.LastUpdated = DateTime.Now;
                   eaform.LastSubmitted = DateTime.Now;
                   eaform.CreateSaveHistoryRecord(viewmodel.InstructorSignature);
                   db.Entry(eaform).State = EntityState.Modified;
                   db.SaveChanges();
                   return Content("submit:Thank you for submitting your review", "text/html");
               }
               else
               {
                   var errors = ModelState.Values.SelectMany(v => v.Errors);
                   foreach (var error in errors)
                   {
                       if (error.Exception != null)
                           Elmah.ErrorSignal.FromCurrentContext().Raise(error.Exception);
                       else
                           Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(error.ErrorMessage));
                   }
                   return Content("submit:Review could not be submitted", "text/html");
               }
           }
        }
        
        // GET: /EAForms/Edit/5
        //[CustomAuthorize(Roles = "Admin,Program Assistant")]

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Title = "Edit";

            var editPageViewModel = new EditPageViewModel(); 

            EAForm eaform = db.EAForms.Find(id);
            if (eaform == null)
            {
                return HttpNotFound();
            }
            //Include Status
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.LoadProperty(eaform, i => i.Status);
            //Include Save History
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.LoadProperty(eaform, i => i.SaveHistory);

            //
            //Load ViewModel
            //

            //Statuses
            
            editPageViewModel = Mapper.Map<EAForm, EditPageViewModel>(eaform, editPageViewModel);
            editPageViewModel.EAFormStatuses = db.EAFormStatuses;
            editPageViewModel.SetupMultiSelectViewModels(eaform);

            return View("Edit", editPageViewModel);
        }

        // POST: /EAForms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditPageViewModel editPageViewModel, string Command, string SearchKeywords)
        {
            if (Command == "Search")
            {
                string action = Command;
                string result = SearchKeywords;
            }
            else
            {
                string edit = Command;
            }
            EAForm eaform = db.EAForms.Find(editPageViewModel.ID);
            if (eaform == null)
            {
                return HttpNotFound();
            }
            //Include Save History
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.LoadProperty(eaform, i => i.SaveHistory);

            eaform = Mapper.Map<EAFormViewModel, EAForm>(editPageViewModel, eaform);
            eaform.Status = db.EAFormStatuses.Find(editPageViewModel.SelectedStatusValue);
            eaform.LastUpdated = DateTime.Now;
            eaform.CreateSaveHistoryRecord(User.Identity.Name);

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(eaform).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.SuccessMessage = "Saved successfully"; // UI.SaveSuccess;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("EditForm", "Form could not be saved"); //ErrorMessages.SaveError);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    if (error.Exception != null)
                        Elmah.ErrorSignal.FromCurrentContext().Raise(error.Exception);
                    else
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(error.ErrorMessage));
                }
            }

            editPageViewModel = Mapper.Map<EAForm, EditPageViewModel>(eaform, editPageViewModel);
            editPageViewModel.EAFormStatuses = db.EAFormStatuses;
            editPageViewModel.SetupMultiSelectViewModels(eaform);
    
            return View(editPageViewModel);
        }

        // GET: /EAForms/Delete/5

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EAForm eaform = db.EAForms.Find(id);
            if (eaform == null)
            {
                return HttpNotFound();
            }
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.LoadProperty(eaform, i => i.Status);

            return View(eaform);
        }

        // POST: /EAForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EAForm eaform = db.EAForms.Find(id);
            if (eaform == null)
            {
                return HttpNotFound();
            }
            //Include Save History
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)db).ObjectContext.LoadProperty(eaform, i => i.SaveHistory);

            db.EAFormStatusSaves.RemoveRange(eaform.SaveHistory);

            db.EAForms.Remove(eaform);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //JQuery Autocomplete interface.  Calls Keyword model
        //based off the users' search term.  The function returns a JSON message
        //for the JQuery Autocomplete to consume.  Note that the 10 record limit
        //was established by user requirements.
        //http://jqueryui.com/autocomplete/#remote-jsonp
        public JsonResult GetKeywords(string term)
        {
            KeywordDBContext db = new KeywordDBContext(); 
            List<string> keywords;

            keywords = db.Keywords.Where(x => x.Name.Contains(term))
                        .Select(y => y.Name).Take(10).ToList();

            return Json(keywords, JsonRequestBehavior.AllowGet); 
        }
    }
}
