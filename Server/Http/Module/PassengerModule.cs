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
            Get["/search"] = Search;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        protected object Index(dynamic parameters)
        {
            IQueryable<Passenger> db = App.Entities.Passengers
                .Include(p => p.Address)
                .Include(p => p.Address.Country)
                .OrderBy(Passenger => Passenger.Id);

            return new PaginatedResult<Passenger>(db, this.perPage, this.GetCurrentPage());
        }

        protected object Single(dynamic parameters)
        {
            int id = (int)parameters.id;

            Passenger model = App.Entities.Passengers
                .Include(m => m.Address)
                .Include(m => m.Address.Country)
                .FirstOrDefault(m => m.Id == id);

            if (model == null)
            {
                return new JsonErrorResponse(404, 404, "Flyer not found");
            }

            return model;
        }

        protected object Search(dynamic parameters)
        {
            string name = this.Request.Query["name"];

            IQueryable<Passenger> db = App.Entities.Passengers
                .Include(p => p.Address)
                .Include(p => p.Address.Country)
                .Where(Flyer => Flyer.DisplayName.Contains(name))
                .OrderBy(Passenger => Passenger.Id);

            return new PaginatedResult<Passenger>(db, this.perPage, this.GetCurrentPage());
        }

        protected object CreateModel(dynamic parameters)
        {
            Passenger model = this.Bind<Passenger>();

            App.Entities.Passengers.Add(model);
            App.Entities.SaveChanges();

            return model;
        }

        protected object UpdateModel(dynamic parameters)
        {
            var model = Single(parameters);

            if (model.GetType() == typeof(JsonErrorResponse))
            {
                return model;
            }

            this.BindTo((Passenger)model);

            App.Entities.SaveChanges();

            return model;
        }

        protected object DeleteModel(dynamic parameters)
        {
            var model = Single(parameters);

            if (model.GetType() == typeof(JsonErrorResponse))
            {
                return model;
            }

            App.Entities.Passengers.Remove(model);
            App.Entities.SaveChanges();

            return HttpStatusCode.OK;
        }
    }
}
