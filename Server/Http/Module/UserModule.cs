using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Uniars.Server.Http.Auth;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;

namespace Uniars.Server.Http.Module
{
    public class UserModule : BaseModule
    {
        public UserModule() : base("/users")
        {
            this.RequiresAuthentication();
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            Get["/"] = Index;
            Get["/{id:int}"] = Single;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        protected object Index(dynamic parameters)
        {
            string username = this.Request.Query["username"];
            string name = this.Request.Query["name"];

            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<User> db = context.Users;

                if (username != null && username != string.Empty)
                {
                    db = context.Users.Where(User => User.Username.Contains(username));
                }

                if (name != null && name != string.Empty)
                {
                    db = context.Users.Where(User => User.Name.Contains(name));
                }

                db = db.OrderBy(User => User.Id);

                return new PaginatedResult<User>(db, this.perPage, this.GetCurrentPage());
            }
        }

        protected object Single(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                User model = context.Users.Find((int)parameters.id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "User not found");
                }

                model.Password = null;

                return model;
            }
        }

        public object CreateModel(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                User model = this.Bind<User>();

                model.Password = Hash.Make(model.Password);

                context.Users.Add(model);
                context.SaveChanges();

                return model;
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                User model = context.Users.FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "User not found");
                }

                string currentPassword = model.Password;

                this.BindTo(model);

                if (model.Password == null)
                {
                    model.Password = currentPassword;
                }
                else
                {
                    model.Password = Hash.Make(model.Password);
                }

                context.SaveChanges();

                return context.Users.Find((int)parameters.id);
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                User model = context.Users.FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "User not found");
                }

                context.Users.Remove(model);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}
