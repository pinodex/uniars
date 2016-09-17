using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;
using Newtonsoft.Json;
using Nancy;
using Nancy.ModelBinding;

namespace Uniars.Server.Http.Modules
{
    public class FlyerModule : NancyModule
    {
        public FlyerModule()
            : base("/flyers")
        {
            Get["/"] = Index;
            Get["/{id:int}"] = Single;
            Get["/{id}"] = SinglePublic;
            Get["/search"] = Search;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        protected object Index(dynamic parameters)
        {
            List<Flyer> flyers = App.Entities.Flyer.ToList();

            return flyers;
        }

        protected object Single(dynamic parameters)
        {
            Flyer flyer = App.Entities.Flyer.Find((int)parameters.id);

            if (flyer == null)
            {
                return new ErrorJsonResponse(404, 404, "Flyer not found");
            }

            return flyer;
        }

        protected object SinglePublic(dynamic parameters)
        {
            string id = parameters.id;

            Flyer flyer = App.Entities.Flyer.First(Flyer => Flyer.PublicId == id);

            if (flyer == null)
            {
                return new ErrorJsonResponse(404, 404, "Flyer not found");
            }

            return flyer;
        }

        protected object Search(dynamic parameters)
        {
            string query = this.Request.Query["query"];

            List<Flyer> flyers = new List<Flyer>();
            flyers = App.Entities.Flyer.Where(Flyer => Flyer.Name.Contains(query)).ToList();

            return flyers;
        }

        protected object CreateModel(dynamic parameters)
        {
            Flyer model = this.Bind<Flyer>("Id", "PublicId");

            model.PublicId = Helper.GetRandomAlphaNumeric();

            App.Entities.Flyer.Add(model);
            App.Entities.SaveChanges();

            return model;
        }

        protected object UpdateModel(dynamic parameters)
        {
            var model = Single(parameters);

            if (model.GetType() == typeof(ErrorJsonResponse))
            {
                return model;
            }

            this.BindTo((Flyer)model, "Id", "PublicId");

            App.Entities.SaveChanges();

            return model;
        }

        protected object DeleteModel(dynamic parameters)
        {
            var model = Single(parameters);

            if (model.GetType() == typeof(ErrorJsonResponse))
            {
                return model;
            }

            App.Entities.Flyer.Remove(model);
            App.Entities.SaveChanges();

            return HttpStatusCode.OK;
        }
    }
}
