using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace MvcTesting
{
    public static class FilmExtensions
    {
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            // Performs a Where only when the condition is met

            if (condition)
            {
                return source.Where(predicate);
            }

            return source;
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource,bool> predicate)
        {
            // Performs a Where only when the condition is met

            if (condition)
            {
                return source.Where(predicate);
            }

            return source;
        }
    }

    public static class enumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var attribute = enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>();

            if (attribute != null)
            {
                return attribute.GetName();
            }

            return enumValue.ToString();
        }
    }
}
