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
                    .Include(p => p.Address)
                    .Include(p => p.Address.Country)
                    .OrderBy(Passenger => Passenger.Id);

                return new PaginatedResult<Passenger>(db, this.perPage, this.GetCurrentPage());
            }
        }

        protected object Single(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger model = context.Passengers
                    .Include(m => m.Address)
                    .Include(m => m.Address.Country)
                    .FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                return model;
            }
        }

        protected object SingleCode(dynamic parameters)
        {
            string code = (string)parameters.code;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger model = context.Passengers
                    .Include(m => m.Address)
                    .Include(m => m.Address.Country)
                    .FirstOrDefault(m => m.Code == code);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                return model;
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
                IQueryable<Passenger> db = App.Entities.Passengers
                    .Include(p => p.Address)
                    .Include(p => p.Address.Country);

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
                    db = db.Where(Model => Model.MiddleName.Contains(familyName));
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
                Passenger model = this.Bind<Passenger>(
                    m => m.Id,
                    m => m.Code,
                    m => m.Address.PassengerId,
                    m => m.Address.Country,
                    m => m.CreatedAt
                );

                if (model.Address != null && model.Address.Country != null)
                {
                    model.Address.Country = context.Countries.Find(model.Address.Country.Id);
                }

                model.GenerateCode();
                model.CreatedAt = DateTime.Now;

                context.Passengers.Add(model);
                context.SaveChanges();

                return model;
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger model = context.Passengers
                    .Include(m => m.Address)
                    .Include(m => m.Address.Country)
                    .FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                this.BindTo((Passenger)model,
                    m => m.Id,
                    m => m.Code,
                    m => m.Address,
                    m => m.Address.PassengerId,
                    m => m.Address.Country,
                    m => m.CreatedAt
                );

                if (model.Address != null && model.Address.Country != null)
                {
                    model.Address.Country = context.Countries.Find(model.Address.Country.Id);
                }

                context.SaveChanges();

                return model;
            }
        }

        protected object DeleteModel(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Passenger model = context.Passengers.FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Passenger not found");
                }

                context.Passengers.Remove(model);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}
