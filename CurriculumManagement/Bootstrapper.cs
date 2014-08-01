using AutoMapper;
using CurriculumManagement.Models;
using CurriculumManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CurriculumManagement
{
    public static class Bootstrapper
    {
        public static void BootStrap()
        {
            ConfigureAutoMapper();
        }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<EAForm, EditPageViewModel>()
                    .ForMember(d => d.SelectedStatusValue, s => s.MapFrom<int>(src => src.Status.ID))
                    .ForMember(d => d.EAFormStatuses, s => s.Ignore())
                    //.ForMember(d => d.AvailableAcademicYears, s => s.Ignore())
                    //.ForMember(d => d.AvailableDepartments, s => s.Ignore())
                    .ForMember(d => d.ActivityTypesViewModel, s => s.Ignore())
                    .ForMember(d => d.ThemesViewModel, s => s.Ignore())
                    .ForMember(d => d.FormularyViewModel, s => s.Ignore());
            
            Mapper.CreateMap<EAForm, EAFormViewModel>()
                    .ForMember(d => d.ActivityTypesViewModel, s => s.Ignore())
                    .ForMember(d => d.KeywordViewModel, s => s.Ignore())
                    .ForMember(d => d.ThemesViewModel, s => s.Ignore())
                    .ForMember(d => d.FormularyViewModel, s => s.Ignore());
            
            Mapper.CreateMap<EAForm, ReviewPageViewModel>()
                    .ForMember(d => d.SelectedStatusValue, s => s.MapFrom<int>(src => src.Status.ID))
                    //.ForMember(d => d.EAFormStatuses, s => s.Ignore())
                    .ForMember(d => d.ActivityTypesViewModel, s => s.Ignore())
                    .ForMember(d => d.KeywordViewModel, s => s.Ignore())
                    .ForMember(d => d.ThemesViewModel, s => s.Ignore())
                    .ForMember(d => d.FormularyViewModel, s => s.Ignore());

            Mapper.CreateMap<EditPageViewModel, EAForm>()
                    //.ForMember(d => d.AcademicYear, s => s.MapFrom<string>(src => src.AvailableAcademicYears.SelectedYear))
                    .ForMember(d => d.ParentForm, s => s.Ignore())
                    .ForMember(d => d.SaveHistory, s => s.Ignore());

            Mapper.CreateMap<EAFormViewModel, EAForm>()
                    .Include<ReviewPageViewModel, EAForm>()
                    .ForMember(d => d.AcademicYear, s => s.Ignore())
                    .ForMember(d => d.ActivityFacilitatorDepartments, s => s.Ignore())
                    .ForMember(d => d.Status, s => s.Ignore())
                    .ForMember(d => d.ParentForm, s => s.Ignore())
                    .ForMember(d => d.LastSubmitted, s => s.Ignore())
                    .ForMember(d => d.LastUpdated, s => s.Ignore())
                    .ForMember(d => d.SaveHistory, s => s.Ignore()); 

            Mapper.CreateMap<ReviewPageViewModel, EAForm>();

            //Mapper.CreateMap<EAForm, ParentChildCategoriesViewModel>()
            //        .ForMember(d => d.SelectedChildrenList, s => s.ResolveUsing<CustomResolver>())
            //        .ForMember(d => d.Label, s => s.Ignore())
            //        .ForMember(d => d.ChildValuesAJAXFetchURL, s => s.Ignore())
            //        .ForMember(d => d.ParentCategoryList, s => s.Ignore());

        }

    }

    public class FormularyCustomResolver : ValueResolver<EAForm, IEnumerable<string>>
    {
        protected override IEnumerable<string> ResolveCore(EAForm eaform)
        {
            if (eaform.Formulary != null)
            {
                return eaform.Formulary.Split(',');
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }
    }

    public class ThemesCustomResolver : ValueResolver<EAForm, IEnumerable<string>>
    {
        protected override IEnumerable<string> ResolveCore(EAForm eaform)
        {
            if (eaform.Themes != null)
            {
                return eaform.Themes.Split(',');
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }
    }

}