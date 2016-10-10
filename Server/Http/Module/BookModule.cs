using System.Data.Entity;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using Uniars.Server.Http.Response;
using Uniars.Server.Model;
using Uniars.Shared.Database;
using Uniars.Shared.Database.Entity;

namespace Uniars.Server.Http.Module
{
    public class BookModule : BaseModule
    {
        public BookModule()
            : base("/bookings")
        {
            this.RequiresAuthentication();

            Get["/"] = Index;
            Get["/{id:int}"] = Single;

            Post["/"] = CreateModel;
            Put["/{id:int}"] = UpdateModel;
            Delete["/{id:int}"] = DeleteModel;
        }

        public object Index(dynamic parameter)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                IQueryable<Book> db = context.Books
                    .Include(m => m.Flight)
                    .Include(m => m.Flight.Airline)
                    .Include(m => m.Flight.Source)
                    .Include(m => m.Flight.Destination)
                    .Include(m => m.Passengers)
                    .OrderBy(m => m.Id);

                return new PaginatedResult<Book>(db, this.perPage, this.GetCurrentPage());
            }
        }

        public object Single(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                int id = (int)parameters.id;

                Book model = context.Books
                    .Include(m => m.Flight)
                    .Include(m => m.Flight.Airline)
                    .Include(m => m.Flight.Source)
                    .Include(m => m.Flight.Destination)
                    .Include(m => m.Passengers)
                    .FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Booking not found");
                }

                return model;
            }
        }

        public object CreateModel(dynamic parameters)
        {
            using (Context context = new Context(App.ConnectionString))
            {
                BookModel bookModel = this.Bind<BookModel>();
                Book model = new Book();

                model.FlightId = bookModel.FlightId;
                model.Passengers = context.Passengers
                    .Where(m => bookModel.PassengerIds.Contains(m.Id))
                    .ToList();

                context.Books.Add(model);
                context.SaveChanges();

                return model;
            }
        }

        protected object UpdateModel(dynamic parameters)
        {
            int id = (int)parameters.id;

            using (Context context = new Context(App.ConnectionString))
            {
                Book model = context.Books
                    .Include(m => m.Flight)
                    .Include(m => m.Passengers)
                    .FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Booking not found");
                }

                BookModel bookModel = this.Bind<BookModel>();

                model.Passengers.Clear();

                if (bookModel.FlightId != 0)
                {
                    model.FlightId = bookModel.FlightId;
                }

                if (bookModel.PassengerIds != null)
                {
                    model.Passengers = context.Passengers
                        .Where(m => bookModel.PassengerIds.Contains(m.Id))
                        .ToList();
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
                Book model = context.Books.FirstOrDefault(m => m.Id == id);

                if (model == null)
                {
                    return new JsonErrorResponse(404, 404, "Booking not found");
                }

                context.Books.Remove(model);
                context.SaveChanges();
            }

            return HttpStatusCode.OK;
        }
    }
}