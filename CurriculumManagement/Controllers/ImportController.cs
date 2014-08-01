using CurriculumManagement.Helpers;
using CurriculumManagement.Models;
using CurriculumManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CurriculumManagement.Controllers
{
    public class ImportController : Controller
    {
        private EAFormDBContext db = new EAFormDBContext();

        //[CustomAuthorize(Roles = "Program Assistant,Admin")]
        public ActionResult Import()
        {
            return View("~/Views/EAForms/Import.cshtml", new ImportViewModel());
        }

        [HttpPost, ActionName("Import")]
        [ValidateAntiForgeryToken]
        public ActionResult ImportFile(ImportViewModel importViewModel)
        {
            if (ModelState.IsValid)
            {
                bool SaveImportAttempt = true;

                //Attempt to get required config items from web.config
                string importFilePath = "";
                string[] availableAcademicYears = null;
                bool thereIsHeader = true;
                DataTable importDataTable = new DataTable();
                if (!CheckRequiredConfigurationItems(new string[] { "ImportFileSaveFolder", "AcademicYearsAvailable", "HeaderRowExists" }, ref importViewModel))
                {
                    return View("~/Views/EAForms/Import.cshtml", importViewModel);
                }
                else
                {
                    availableAcademicYears = WebConfigurationManager.AppSettings["AcademicYearsAvailable"].ToString().Split(',');
                    if (availableAcademicYears.Length < 1)
                    {
                        LogError("There are no available years found in config entry: AcademicYearsAvailable.", ref importViewModel);
                        SaveImportAttempt = false;
                    }
                    importFilePath = WebConfigurationManager.AppSettings["ImportFileSaveFolder"].ToString();
                    thereIsHeader = Convert.ToBoolean(WebConfigurationManager.AppSettings["HeaderRowExists"].ToString());
                }

                //Pull data from Excel file
                string uploadedFile = "";
                //try
                //{
                    uploadedFile = Server.MapPath(importFilePath + importViewModel.File.FileName.ToString());
                    importViewModel.Log("Attempting to save file \"" + importViewModel.File.FileName.ToString() + "\" to folder \"" + Server.MapPath(importFilePath) + "\"...");
                    //create the file on the server to the specified folder in the web.config
                    importViewModel.File.SaveAs(uploadedFile);
                    importViewModel.Log("Reading file \"" + uploadedFile + "\"...");
                    importDataTable = ReturnDataTableFromExcel(uploadedFile, ref importViewModel); //Pull data from Excel file

                //}
                //catch (Exception ex)
                //{
                //    LogError(ex, new string[] { "Failure encountered!" }, ref importViewModel);
                //}
                //finally
                //{
                //    //delete the file
                    System.IO.File.Delete(uploadedFile);
                //}


                //Form object to iteratively create and add to collection to save to DB
                EAForm eaform;

                int startIndex = 0;
                //if (thereIsHeader) //skip the header row OR not? seems to already skip the header row
                //    startIndex = 1;
                for (int i = startIndex; i < importDataTable.Rows.Count; i++)
                {
                    string rowNumber = (i + 1).ToString();
                    importViewModel.Log("Extracting data for row: " + rowNumber + "...");
                    //Extract from datatable representation of EXCEL sheet for row = i
                    string academicyear = importDataTable.Rows[i][0].ToString().Trim();
                    string course = importDataTable.Rows[i][1].ToString().Trim();
                    string courseNumber = importDataTable.Rows[i][2].ToString().Trim();
                    string blockweektitle = importDataTable.Rows[i][3].ToString().Trim();
                    string activitytitle = importDataTable.Rows[i][4].ToString().Trim();
                    string courseTitle = courseNumber + "-" + course;
                    string keystring = academicyear + "/" + courseTitle + "/" + blockweektitle + "/" + activitytitle;

                    if (!availableAcademicYears.Contains(academicyear))
                    {
                        LogError("Unexpected Academic Year data: " + academicyear + ". Quitting import.", ref importViewModel);
                        SaveImportAttempt = false;
                        break;
                    }

                    //EXCLUDE ACTIVITIES: containing "– Tutorial 1" or "– Tutorial 2" or "– Tutorial 3" or "– Quiz"
                    if (activitytitle.ToUpper().Contains("TUTORIAL 1") || activitytitle.ToUpper().Contains("TUTORIAL 2") ||
                        activitytitle.ToUpper().Contains("TUTORIAL 3") || activitytitle.ToUpper().Contains("QUIZ"))
                    {
                        importViewModel.Log("Skipping 'Tutorial/Quiz': " + keystring);
                        continue;
                    }

                    //DUPLICATE KEY CHECK
                    var formsCount = (from f in db.EAForms
                                      where (f.AcademicYear + "/" + f.Course + "/" + f.BlockWeekTitle + "/" + f.ActivityTitle) == keystring
                                      select f).Count();

                    if (formsCount > 0)
                    {
                        LogError("Found duplicate at row: " + rowNumber + " = " + keystring + ". Quitting import.", ref importViewModel);
                        SaveImportAttempt = false;
                        break;
                    }
                    string description_org = importDataTable.Rows[i][5].ToString();
                    string description = importDataTable.Rows[i][5].ToString().Trim();
                    string sessionObjectives = importDataTable.Rows[i][6].ToString().Trim();
                    if (activitytitle == "Clinical Study Design")
                    {
                        importViewModel.Log(description_org);
                        importViewModel.Log(description);
                    }
                    if (activitytitle == "Clinical Diagnosis in a world of Uncertainty")
                    {
                        importViewModel.Log(description_org);
                        importViewModel.Log(description);
                    } 
                    if (activitytitle == "Sexuality and Culture")
                    {
                        importViewModel.Log(description_org);
                        importViewModel.Log(description);
                    } 
                    if (activitytitle == "Orientation to Block 3")
                    {
                        importViewModel.Log(description_org);
                        importViewModel.Log(description);
                    }


                    string activityTypesString = importDataTable.Rows[i][7].ToString().Trim();
                    string formularysString = importDataTable.Rows[i][8].ToString().Trim();
                    string keywordsString = importDataTable.Rows[i][9].ToString().Trim();
                    string themesString = importDataTable.Rows[i][10].ToString().Trim();

                    string courseDirector = importDataTable.Rows[i][11].ToString().Trim();
                    string faculty = importDataTable.Rows[i][12].ToString().Trim();
                    string instructor = importDataTable.Rows[i][13].ToString().Trim();
                    string startdate = importDataTable.Rows[i][14].ToString().Trim();

                    eaform = new EAForm();
                    eaform.AcademicYear = academicyear;
                    eaform.Course = courseTitle;
                    eaform.BlockWeekTitle = blockweektitle;
                    eaform.ActivityTitle = activitytitle;

                    //List<String> activityTypes = ExtractSpecificDataFromJumble("Educational Methods", sessionMappings);
                    //List<String> keywords = ExtractSpecificDataFromJumble("Keywords", keywordString);
                    //List<String> themes = ExtractSpecificDataFromJumble("Themes", themeString);
                    //List<String> formulary = ExtractSpecificDataFromJumble("Formulary (Drug Class ~ Prototype)", formularyString);
                    //string activityTypesString = "";
                    //string keywordsString = "";
                    //string themesString = "";
                    //string drugsString = "";
                    //string delim = "";
                    //foreach (string atString in activityTypes)
                    //{
                    //    activityTypesString += delim + atString;
                    //    delim = ",";
                    //}

                    //delim = "";
                    ////We use semi-colon as the delimiter for keywords because some keywords have a comma in them.
                    //foreach (string kwString in keywords)
                    //{
                    //    keywordsString += delim + kwString;
                    //    delim = ";";
                    //}

                    //delim = "";
                    //foreach (string themeString in themes)
                    //{
                    //    themesString += delim + themeString.Replace("::", "~");
                    //    delim = ",";
                    //}

                    //delim = "";
                    //foreach (string drugString in formulary)
                    //{
                    //    drugsString += delim + drugString;
                    //    delim = ",";
                    //}

                    eaform.ActivityType = activityTypesString;
                    eaform.Keywords = keywordsString;
                    eaform.Themes = themesString;
                    eaform.Formulary = formularysString;

                    //Activity Facilitator Fields

                    if (courseDirector != "")
                    {
                        eaform.ActivityFacilitatorType = "Course Director";
                        eaform.ActivityFacilitatorNames = courseDirector;
                    }
                    else if (faculty != "")
                    {
                        eaform.ActivityFacilitatorType = "Week Chair";
                        eaform.ActivityFacilitatorNames = faculty;
                    }
                    else if (instructor != "")
                    {
                        eaform.ActivityFacilitatorType = "Instructor";
                        eaform.ActivityFacilitatorNames = instructor;
                    }
                    else
                    {
                        eaform.ActivityFacilitatorType = "Other";
                    }

                    //eaform.ActivityFacilitatorDepartments = "";

                    eaform.Abstract = description;
                    string[] learningobjArray = sessionObjectives.Split('~');

                    if (learningobjArray.Length > 2)
                    { 
                        //Encountered more than 1 ~ character -- stop import
                        LogError("Invalid use of ~ character at row: " + rowNumber + " = " + keystring + ". Quitting import.", ref importViewModel);
                        SaveImportAttempt = false;
                        break;

                    }
                    eaform.LearningObjectives = StandardizeLineBreaks(learningobjArray[0]);
                    eaform.LastUpdated = DateTime.Now;

                    //Attempt to parse "Last Submitted" from Session Objectives
                    if (learningobjArray.Length > 1)
                    {
                        try
                        {
                            eaform.LastSubmitted = DateTime.Parse(learningobjArray[1].Trim());
                        }
                        catch
                        {
                            LogError("Invalid Last Submitted Date at row: " + rowNumber + " = " + keystring + ". Quitting import.", ref importViewModel);
                            SaveImportAttempt = false;
                            break;
                        }


                    }
                    else
                        eaform.LastSubmitted = null;

                    //Try and parse Start Date
                    try {
                        eaform.StartDate = DateTime.Parse(startdate);
                    }
                    catch
                    {
                        eaform.StartDate = null;
                    }

                    //Default status when importing is "one45 Extract"
                    eaform.Status = db.EAFormStatuses
                                        .Where(f => f.Name == "one45 Extract")
                                        .FirstOrDefault();

                    //Add extracted original to DB
                    db.EAForms.Add(eaform);

                    //Now create a copy for editing
                    EAForm eaformCopy = eaform.Copy();

                    //Link the copy to the original
                    eaformCopy.ParentForm = eaform;

                    //Change the status of the copy to "Imported";
                    eaformCopy.Status = db.EAFormStatuses
                                        .Where(f => f.Name == "Imported")
                                        .FirstOrDefault();

                    //Create Initial "Imported" SaveHistory
                    eaformCopy.CreateSaveHistoryRecord("Ken");//User.Identity.Name);

                    //Add "Imported" copy to DB
                    db.EAForms.Add(eaformCopy);


                }

                //Now attempt save to DB if no problems were encountered before
                if (SaveImportAttempt)
                {
                    try
                    {
                        importViewModel.Log("Attempting to save to database...");
                        db.SaveChanges();
                        importViewModel.Log("Save successful!");
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            LogError(String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State), ref importViewModel);
                            foreach (var ve in eve.ValidationErrors)
                                LogError(String.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage), ref importViewModel);
                        }

                        LogError(ex, new string[] { "Failure encountered!" }, ref importViewModel);
                    }

                }

                //return RedirectToAction("Index");
                return View("~/Views/EAForms/Import.cshtml", importViewModel);

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

                return View("~/Views/EAForms/Import.cshtml", importViewModel);
            }
        }

        private string StandardizeLineBreaks(string preString)
        {
            //Need to replace to keep CR's standard across application
            string postString = Server.HtmlEncode(preString).Trim();
            postString = postString.Replace("\r\n", "\n");
            postString = postString.Replace("\r", "\n");
            postString = postString.Replace("\n", "\r\n");
            return Server.HtmlDecode(postString);
        }

        private List<String> ExtractSpecificDataFromJumble(string idString, string jumble)
        {
            List<String> foundItems = new List<string>();
            idString = idString + " :: ";
            int index = 0;
            bool found = true;
            while (found && index < jumble.Length)
            {
                found = false;
                index = jumble.IndexOf(idString, index, StringComparison.CurrentCulture);
                if (index > -1)
                {
                    found = true;
                    int semicolonOrEndIndex = jumble.IndexOf(';', index); // find next following semicolon or end
                    if (semicolonOrEndIndex < 0)
                    {
                        //read to end of jumble
                        semicolonOrEndIndex = jumble.Length - 1;
                    }
                    else
                        semicolonOrEndIndex = semicolonOrEndIndex - 1; //don't include the semicolon

                    //move the index past the prefix idString (since we don't want to include it)
                    index += idString.Length;

                    string foundItem = jumble.Substring(index, semicolonOrEndIndex - index + 1);
                    foundItems.Add(foundItem);

                    //move index forward the length of the foundItem
                    index += foundItem.Length;
                }
            }

            return foundItems;
        }

        public DataTable ReturnDataTableFromExcel(string pth, ref ImportViewModel importViewModel)
        {
            string strcon = string.Empty;

            //if (Path.GetExtension(pth).ToLower().Equals(".xls") || Path.GetExtension(pth).ToLower().Equals(".xlsx"))
            //{
            //    //strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pth + ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
            //    strcon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pth + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            //}
            //else
            //{
            //    strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pth + ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
            //}
            if (Path.GetExtension(pth).ToLower().Equals(".xlsx"))
            {
                strcon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pth + ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
            }
            else
            {
                strcon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pth + ";Extended Properties=\"Excel 8.0;HDR=YES;\"";
            }
            //string strselect = "Select * from [Sheet1$]";
            string strselect = "Select * from [Worksheet$]";
            DataTable exDT = new DataTable();
            using (OleDbConnection excelCon = new OleDbConnection(strcon))
            {
                //try
                //{
                    importViewModel.Log("Attempting to open connection to file...");
                    excelCon.Open();
                    using (OleDbDataAdapter exDA = new OleDbDataAdapter(strselect, excelCon))
                    {
                        importViewModel.Log("Pulling data from file...");
                        exDA.Fill(exDT);
                    }
                //}
                //catch (OleDbException oledb)
                //{
                //    LogError(oledb, new string[] { "Failure encountered!" }, ref importViewModel);
                //}
                //finally
                //{
                    excelCon.Close();
                //}

                //for (int i = 0; i < exDT.Rows.Count; i++)
                //{
                //Make changes here if necessary
                //
                //i.e.
                //// Check if first column is empty
                //// If empty then delete such record
                //if (exDT.Rows[i]["CardNo"].ToString() == string.Empty)
                //{
                //    exDT.Rows[i].Delete();
                //}
                //}
                //exDT.AcceptChanges();  // refresh rows changes
                if (exDT.Rows.Count == 0)
                {
                    LogError("File uploaded has no records found!", ref importViewModel);
                }
                return exDT;
            }
        }

        private bool CheckRequiredConfigurationItems(string[] keys, ref ImportViewModel importViewModel)
        {
            foreach (var key in keys)
            {
                if (WebConfigurationManager.AppSettings[key] == null)
                {
                    string msg = String.Format("Could not find config entry in web.config: {0}", key);
                    LogError(msg, ref importViewModel);

                    return false;
                }
            }

            return true;

        }

        private void LogError(string msg, ref ImportViewModel importViewModel)
        {
            importViewModel.Log(msg);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(msg));
        }

        private void LogError(Exception ex, string[] args, ref ImportViewModel importViewModel)
        {
            if (args != null && args.Length > 0)
                importViewModel.Log(args[0].ToString());
            else
                importViewModel.Log(ex.ToString());
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        
        }
    }
}