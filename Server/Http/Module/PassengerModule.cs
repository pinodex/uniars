using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Uniars.Server.Http.Response;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;
using Uniars.Shared.Database;
using Uniars.Server.Http.Auth;

namespace Uniars.Server.Http.Module
{
    public class PassengerModule : BaseModule
    {
        public PassengerModule()
            : base("/passengers")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/{code}"] = SingleCode;
            Get["/search"] = Search;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        protected object Index(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Passenger> db = context.Passengers
                    .Include(m => m.Contacts)
                    .OrderBy(m => m.Id);

                return new PaginatedResult<Passenger>(db, this.perPage, this.GetCurrentPage());
            }
        }

        protected object Single(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger passenger = context.Passengers
                    .Include(m => m.Contacts)
                    .FirstOrDefault(m => m.Id == id);

                if (passenger == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                return passenger;
            }
        }

        protected object SingleCode(dynamic parameters)
        {
            string code = (string)parameters.code;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger passenger = context.Passengers
                    .Include(m => m.Contacts)
                    .FirstOrDefault(m => m.Code == code);

                if (passenger == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                return passenger;
            }
        }

        protected object Search(dynamic parameters)
        {
            string givenName = this.Request.Query["given_name"];
            string familyName = this.Request.Query["family_name"];
            string middleName = this.Request.Query["middle_name"];
            string displayName = this.Request.Query["display_name"];

            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Passenger> db = context.Passengers
                    .Include(m => m.Contacts);

                if (givenName != null)
                {
                    db = db.Where(Model => Model.GivenName.Contains(givenName));
                }

                if (familyName != null)
                {
                    db = db.Where(Model => Model.FamilyName.Contains(familyName));
                }

                if (middleName != null)
                {
                    db = db.Where(Model => Model.MiddleName.Contains(middleName));
                }

                if (displayName != null)
                {
                    db = db.Where(Model => Model.DisplayName.Contains(displayName));
                }

                db = db.OrderBy(Passenger => Passenger.Id);

                return new PaginatedResult<Passenger>(db, this.perPage, this.GetCurrentPage());
            }
        }

        protected object CreateModel(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                Passenger passenger = this.Bind<Passenger>(
                    m => m.Id,
                    m => m.Code
                );

                passenger.GenerateCode();

                context.Passengers.Add(passenger);
                context.SaveChanges();

                return passenger;
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger passenger = context.Passengers.FirstOrDefault(m => m.Id == id);

                if (passenger == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                this.BindTo(passenger,
                    m => m.Id,
                    m => m.Code,
                    m => m.CreatedAt,
                    m => m.UpdatedAt
                );

                context.SaveChanges();

                return passenger;
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            this.RequiresClaims(UserIdentity.ROLE_ADMIN);

            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger passenger = context.Passengers.FirstOrDefault(m => m.Id == id);

                if (passenger == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                context.Passengers.Remove(passenger);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}
