using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeLabX.EntityFramework.Utilities
{
    public static class DbContextHelper
    {
        public static void CreateTable(ModelBuilder modelBuilder)
        {
            foreach (Type type in GetAllTypesImplementingBaseType(typeof(IEntityContext)))
            {
                if (type == typeof(EntityContext)) continue;
                var method = modelBuilder.GetType().GetMethod("Entity", new Type[] { });
                var methodGen = method.MakeGenericMethod(type);
                var invoke = methodGen.Invoke(modelBuilder, null) as EntityTypeBuilder;
                
                var ex = typeof(RelationalEntityTypeBuilderExtensions);
                var entity = ex.GetMethod("ToTable", new Type[] { typeof(EntityTypeBuilder), typeof(string) });
                entity.Invoke(ex, new object[] { invoke, type.Name });
            }
        }

        public static IEnumerable<Type> GetAllTypesImplementingBaseType(Type baseType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypesOrDefault()
               .Where(m => {
                   try
                   {
                       return m.GetTypeInfo().ImplementedInterfaces.Any(t => t == baseType);
                   }
                   catch (Exception)
                   {
                       return false;
                   }

               }).ToList()).ToList();

            return types;
        }

        public static IEnumerable<Type> GetAllTypesImplementingAttribute(Type attributeType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypesOrDefault()
               .Where(m => {
                   try
                   {
                       return m.GetCustomAttributes(attributeType, false).Length > 0;
                   }
                   catch (Exception)
                   {
                       return false;
                   }

               }).ToList()).ToList();

            return types;
        }

        public static IEnumerable<Type> GetTypesOrDefault(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return new List<Type>();                
            }
        }
    }
}
