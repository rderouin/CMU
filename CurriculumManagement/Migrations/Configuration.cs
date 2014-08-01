namespace CurriculumManagement.Migrations
{
    using CurriculumManagement.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CurriculumManagement.Models.EAFormDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CurriculumManagement.Models.EAFormDBContext context)
        {
            context.EAFormStatuses.AddOrUpdate(i => i.Name,
                new EAFormStatus
                {
                    Name = "Imported",
                    Description = "Copies created from the one45 extract. Ready for distribution."
                },
                new EAFormStatus
                {
                    Name = "Distributed",
                    Description = "EA Forms that are sent to teaching staff for review and changes."
                },
                new EAFormStatus
                {
                    Name = "Draft",
                    Description = "EA Forms saved as draft by teaching staff who are not yet ready to submit."
                },
                new EAFormStatus
                {
                    Name = "Submitted",
                    Description = "EA Forms submitted by teaching staff after completing review and changes."
                },
                new EAFormStatus
                {
                    Name = "PA Checked",
                    Description = "EA Forms checked by a Program Assistant."
                },
                new EAFormStatus
                {
                    Name = "Completed",
                    Description = "CMU has gathered changes from instructor(s) and is done using the form."
                },
                new EAFormStatus
                {
                    Name = "one45 Extract",
                    Description = "EA Forms that match the one45 extract imported."
                }
            );

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //Create 3 roles
            //
            //Admin
            string roleName = "Admin";
            if (!RoleManager.RoleExists(roleName))
            {
                var roleresult = RoleManager.Create(new IdentityRole(roleName));
            }

            //PA
            roleName = "Program Assistant";
            if (!RoleManager.RoleExists(roleName))
            {
                var roleresult = RoleManager.Create(new IdentityRole(roleName));
            }

            //CMC
            roleName = "Curriculum Materials Coordinator";
            if (!RoleManager.RoleExists(roleName))
            {
                var roleresult = RoleManager.Create(new IdentityRole(roleName));
            }

            //Create test users
            var user = new ApplicationUser();
            user.UserName = "chengken";
            var result = UserManager.Create(user);
            if (result.Succeeded)
            {
                result = UserManager.AddToRole(user.Id, "Admin");
            }

            user = new ApplicationUser();
            user.UserName = "pdkeats";
            result = UserManager.Create(user);
            if (result.Succeeded)
            {
                result = UserManager.AddToRole(user.Id, "Admin");
            }

            user = new ApplicationUser();
            user.UserName = "staff01";
            result = UserManager.Create(user);
            if (result.Succeeded)
            {
                result = UserManager.AddToRole(user.Id, "Program Assistant");
            }

            user = new ApplicationUser();
            user.UserName = "cmc01";
            result = UserManager.Create(user);
            if (result.Succeeded)
            {
                result = UserManager.AddToRole(user.Id, "Curriculum Materials Coordinator");
            }

            //base.Seed(context);

            context.SaveChanges();

            //context.EAForms.AddOrUpdate(i => i.Course,
            //    new EAForm
            //    {
            //        Course = "Gastrointestinal – FMED 424",
            //        BlockWeekTitle = "Esophagus and Stomach",
            //        ActivityTitle = "Neuromuscular Control of Gut Motility",
            //        ActivityType = "Test Activity Type",
            //        LastUpdated = DateTime.Now,
            //        Status = context.EAFormStatuses.Single(i => i.Name == "Imported")
            //    },

            //    new EAForm
            //    {
            //        Course = "Cerebral Lobes – BMED 301",
            //        BlockWeekTitle = "Brain",
            //        ActivityTitle = "Neurological Control of Brain Functions",
            //        ActivityType = "Test Activity Type",
            //        LastUpdated = DateTime.Now,
            //        Status = context.EAFormStatuses.Single(i => i.Name == "Imported")
            //    }
            //);

        }
    }
}
