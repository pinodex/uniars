using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uniars.Shared.Database.Entity;
using Uniars.Shared.Foundation;
using Nancy;
using Nancy.Security;
using Nancy.ModelBinding;
using Uniars.Server.Http.Response;

namespace Uniars.Server.Http.Modules
{
    public class FlyerModule : NancyModule
    {
        public FlyerModule()
            : base("/flyers")
        {
            this.RequiresAuthentication();

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
            List<Flyer> flyers = App.Entities.Flyers.ToList();

            return flyers;
        }

        protected object Single(dynamic parameters)
        {
            Flyer model = App.Entities.Flyers.Find((int)parameters.id);

            if (model == null)
            {
                return new JsonErrorResponse(404, 404, "Flyer not found");
            }

            return model;
        }

        protected object SinglePublic(dynamic parameters)
        {
            string id = parameters.id;

            Flyer model = App.Entities.Flyers.First(Flyer => Flyer.PublicId == id);

            if (model == null)
            {
                return new JsonErrorResponse(404, 404, "Flyer not found");
            }

            return model;
        }

        protected object Search(dynamic parameters)
        {
            string name = this.Request.Query["name"];

            List<Flyer> models = new List<Flyer>();
            models = App.Entities.Flyers.Where(Flyer => Flyer.Name.Contains(name)).ToList();

            return models;
        }

        protected object CreateModel(dynamic parameters)
        {
            Flyer model = this.Bind<Flyer>("Id", "PublicId");

            model.PublicId = Helper.GetRandomAlphaNumeric();

            App.Entities.Flyers.Add(model);
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

            this.BindTo((Flyer)model, "Id", "PublicId");

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

            App.Entities.Flyers.Remove(model);
            App.Entities.SaveChanges();

            return HttpStatusCode.OK;
        }
    }
}
