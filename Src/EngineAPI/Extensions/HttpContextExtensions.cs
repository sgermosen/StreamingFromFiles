using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EngineAPI.Extensions
{
    public static class HttpContextExtensions
    {
        public async static Task InsertPaginationInHeader<T>(this HttpContext httpContext,
            IQueryable<T> queryable)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }
            double quantity = await queryable.CountAsync();
            httpContext.Response.Headers.Add("totalRecordsQuantity", quantity.ToString());
        }
    }
}
