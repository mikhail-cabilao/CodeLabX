using CodeLabX.EntityFramework.Attributes;
using CodeLabX.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CodeLabX.EntityFramework.Extensions
{
    public static class QueryExtensions
    {
        public static T DeepInclude<T>(this T entity, string navigationProperty, DataContext context)
        {
            var list = new List<T>();
            list.Add(entity);

            return list.DeepInclude(navigationProperty, context).FirstOrDefault();
        }

        public static IEnumerable<T> Include<T>(this IQueryable<T> queries, string[] properties) where T : class
        {
            return properties.Aggregate(queries, (current, property) =>
            {
                return current.Include(property);
            });
        }

        public static IEnumerable<T> DeepInclude<T>(this IEnumerable<T> queries, DataContext context) where T : class
        {
            var rootProperty = typeof(T).GetProperties()
                .Where(t => t.IsDefined(typeof(AllowExpand), false));

            var list = new List<T>();
            foreach (var root in rootProperty)
            {
                var isCollection = root.PropertyType.IsAssignableFrom(typeof(IEnumerable));
                var getProperties = (isCollection ? root.PropertyType.GenericTypeArguments.FirstOrDefault() : root.PropertyType)
                    .GetProperties()
                    .Where(t => t.IsDefined(typeof(AllowExpand), false));

                var result = getProperties
                    .Aggregate(queries,
                        (current, property) =>
                        {
                            var result = new List<dynamic>();
                            if (!root.PropertyType.IsAssignableFrom(typeof(IEnumerable)))
                                result = context.Set(property.PropertyType).ToList();

                            foreach (var q in current.ToList())
                            {
                                var entity = q as dynamic;
                                var deepProp = q.GetType().GetProperty(root.Name).PropertyType.GetProperty(property.Name);
                                var navPropValue = q.GetType().GetProperty(root.Name).GetValue(q);

                                if (root.PropertyType.IsAssignableFrom(typeof(IEnumerable)))
                                {
                                    var type = root.PropertyType.GenericTypeArguments.FirstOrDefault();
                                    var innerResults = navPropValue as IEnumerable<dynamic>;
                                    var res = context.Set(property.PropertyType);
                                    foreach (var d in innerResults)
                                        d.GetType()
                                        .GetProperty(property.Name)
                                        .SetValue(d, res.FirstOrDefault(r => r.Id.ToString() == d.GetType().GetProperty($"{property.Name}Id").GetValue(d).ToString()));
                                }
                                else
                                {
                                    var comparerId = navPropValue.GetType().GetProperty($"{property.Name}Id").GetValue(navPropValue);
                                    deepProp.SetValue(navPropValue, result.FirstOrDefault(r => r.Id.ToString() == comparerId.ToString()));
                                }
                            }

                            return current;
                        });

                list.AddRange(result);
            }

            return list.Distinct().AsEnumerable();
        }

        public static IEnumerable<dynamic> Set(this DataContext context, Type type)
        {
            return context.GetType()
                .GetMethods()
                .Where(m => m.IsGenericMethod && m.Name == "Set")
                .FirstOrDefault()
                .MakeGenericMethod(type)
                .Invoke(context, new object[] { }) as IEnumerable<dynamic>;
        }
    }
}