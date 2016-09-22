﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;
using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using Uniars.Server.Http.Response;

namespace Uniars.Server.Http.Module
{
    public class AirlineModule : BaseModule
    {
        public AirlineModule()
            : base("/airlines")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/search"] = Search;
        }

        public object Index(dynamic parameter)
        {
            IQueryable<Airline> db = App.Entities.Airlines.OrderBy(Airline => Airline.Id);

            return new PaginatedResult<Airline>(db, this.perPage, this.GetCurrentPage());
        }

        public object Single(dynamic parameters)
        {
            Airline model = App.Entities.Airlines.Find((int)parameters.id);

            if (model == null)
            {
                return new JsonErrorResponse(404, 404, "Airline not found");
            }

            return model;
        }

        protected object Search(dynamic parameters)
        {
            string name = this.Request.Query["name"];
            string callsign = this.Request.Query["callsign"];
            string country = this.Request.Query["country"];

            IQueryable<Airline> db = App.Entities.Airlines;
            
            if (name != null)
            {
                db = db.Where(Flyer => Flyer.Name.Contains(name));
            }

            if (callsign != null)
            {
                db = db.Where(Flyer => Flyer.Callsign.Contains(callsign));
            }

            if (country != null)
            {
                db = db.Where(Flyer => Flyer.Country.Contains(country));
            }

            db = db.OrderBy(Airline => Airline.Id);

            return new PaginatedResult<Airline>(db, this.perPage, this.GetCurrentPage());
        }
    }
}