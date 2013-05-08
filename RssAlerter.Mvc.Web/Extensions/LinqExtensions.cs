using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RssAlerter.Mvc.Web.Extensions
{
    public static class LinqExtensions
    {
        public static bool ContainsAny<T>(this IEnumerable<T> list, IEnumerable<T> otherList)
        {
            foreach (var item in otherList)
            {
                if (list.Contains(item))
                    return true;
            }

            return false;
        }
    }
}