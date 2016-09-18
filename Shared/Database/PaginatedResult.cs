using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniars.Shared.Database
{
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; }

        public InfoModel Info { get; set; }

        public PaginatedResult(IQueryable<T> db, int perPage, int currentPage)
        {
            int total = db.Count();

            if (currentPage < 1)
            {
                currentPage = 1;
            }

            currentPage -= 1;

            this.Data = db.Skip(perPage * currentPage).Take(perPage).ToList();

            this.Info = new InfoModel
            {
                Total = total,
                PerPage = perPage,
                CurrentPage = currentPage + 1,
                DataCount = this.Data.Count()
            };
        }

        public class InfoModel
        {
            public int Total { get; set; }

            public int PerPage { get; set; }

            public int CurrentPage { get; set; }

            public int PageCount
            {
                get
                {
                    return Convert.ToInt32(Math.Ceiling((double)Total / (double)PerPage));
                }
            }

            public int DataCount { get; set; }
        }
    }
}
