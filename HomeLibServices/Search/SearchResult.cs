using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HomeLibServices.Search
{
    internal class SearchResult<T> : List<T>
    {
        public SearchResult(IQueryable<T> query, string property, string searchTerm)
        {
            query = Search(query, property, searchTerm);
            AddRange(query);
        }

        private static IQueryable<T> Search(IQueryable<T> query, string searchProperty, string searchTerm)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var source = searchProperty.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var body = Expression.Call(source, "Contains", Type.EmptyTypes, Expression.Constant(searchTerm, typeof(string)));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }
    }
}
