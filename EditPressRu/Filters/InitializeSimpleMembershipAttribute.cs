using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using EditPressRu.Repository;
using WebMatrix.WebData;
using EditPressRu.Models.DB;

namespace EditPressRu.Filters
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    //public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    //{
    //    private static SimpleMembershipInitializer _initializer;
    //    private static object _initializerLock = new object();
    //    private static bool _isInitialized;

    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        // Ensure ASP.NET Simple Membership is initialized only once per app start
    //        LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
    //    }

    //    private class SimpleMembershipInitializer
    //    {
    //        public SimpleMembershipInitializer()
    //        {
    //            Database.SetInitializer<EditPressRuEntities>(null);

    //            try
    //            {
    //                using (var context = new EditPressRuEntities())
    //                {
    //                    if (!context.Database.Exists())
    //                    {
    //                        // Create the SimpleMembership database without Entity Framework migration schema
    //                        ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
    //                    }
    //                }

    //                WebSecurity.InitializeDatabaseConnection("EditPressRuEntities", "UserProfile", "UserId", "UserName", autoCreateTables: false);
    //            }
    //            catch (Exception ex)
    //            {
    //                throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
    //            }
    //        }
    //    }
    //}

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                try
                {
                    WebSecurity.InitializeDatabaseConnection("EditPressUsersConn", "UserProfile", "UserId", "UserName", autoCreateTables: false);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Something is wrong", ex);
                }
            }
        }
    }
}