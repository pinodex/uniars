using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Security;
using Nancy.ModelBinding;
using Uniars.Server.Http.Auth;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;
using System.Data.Entity;
using Uniars.Server.Http.Response;
using Nancy;


namespace Uniars.Server.Http.Module
{
    public class SettingsModule : BaseModule
    {
        public SettingsModule()
            : base("/settings")
        {
            this.RequiresAuthentication();

            Put["/password"] = Password;
        }

        public object Password(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                int userId = ((UserIdentity)this.Context.CurrentUser).User.Id;
                User user = context.Users.Find(userId);

                if (user == null)
                {
                    return new JsonErrorResponse(404, 404, "User not found");
                }

                string currentPassword = user.Password;
                string currentPasswordRequest = this.Request.Form["current_password"];
                string newPassword = this.Request.Form["new_password"];

                if (currentPasswordRequest == null || !Hash.Check(currentPasswordRequest, user.Password))
                {
                    return HttpStatusCode.NotAcceptable;
                }

                user.Password = Hash.Make(newPassword);
                context.SaveChanges();

                return user;
            }
        }
    }
}
