using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.FreeSql.Extensions
{
    public static class FreeSqlExtensions
    {
        /// <summary>
        /// 创建IdentityServer4.FreeSql下的实体表信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="freeSql"></param>
        /// <returns></returns>
        public static async Task CreateEntitiesTable<T>(this IFreeSql<T> freeSql)
        {
            try
            {
                await Task.Run(() =>
                {
                    freeSql.CodeFirst.SyncStructure(GetTypes());
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateTable:{ex.Message}");
            }
        }

        /// <summary>
        /// 创建IdentityServer4.FreeSql下的实体表信息
        /// </summary>
        /// <param name="freeSql"></param>
        /// <returns></returns>
        public static async Task CreateEntitiesTable(this IFreeSql freeSql)
        {
            try
            {
                await Task.Run(() =>
                {
                    freeSql.CodeFirst.SyncStructure(GetTypes());
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateTable:{ex.Message}");
            }
        }

        /// <summary>
        /// 查找实体类信息
        /// </summary>
        /// <returns></returns>
        private static Type[] GetTypes()
        {
            try
            {
                List<Type> tableAssembies = new List<Type>();
                List<string> entitiesFullName = new List<string>()
                    {
                    "IdentityServer4.FreeSql.Entities",
                    };

                var types = Assembly.GetAssembly(typeof(FreeSqlExtensions)).GetExportedTypes();
                foreach (Type type in types)
                {
                    foreach (var fullname in entitiesFullName)
                    {
                        if (type.FullName.StartsWith(fullname) && type.IsClass && !type.IsAbstract)
                        {
                            tableAssembies.Add(type);
                        }
                    }
                }
                return tableAssembies.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
