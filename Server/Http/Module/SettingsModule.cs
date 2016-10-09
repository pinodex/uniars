using Nancy;
using Nancy.Security;
using Uniars.Server.Http.Auth;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;

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
