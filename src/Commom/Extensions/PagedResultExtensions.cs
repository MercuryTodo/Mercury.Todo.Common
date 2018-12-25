using Common.Type;
using System;
using System.Linq;

namespace Common.Extensions
{
    public static class PagedResultExtensions
    {
        public static PagedResult<TResult> Select<TSource, TResult>(this PagedResult<TSource> result,
            Func<TSource, TResult> selector)
        {
            if (result != null)
            {
                var mappedResults = result.Items.Select(selector);

                return PagedResult<TResult>.From(result, mappedResults);
            }
            return null;
        }
    }
}