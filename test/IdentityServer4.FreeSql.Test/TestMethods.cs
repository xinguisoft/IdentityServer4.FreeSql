using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4;
using IdentityServer4.FreeSql;

namespace IdentityServer4.FreeSql.Test
{
    [TestClass]
    public class TestMethods
    {
        /// <summary>
        /// 此测试方法主要用来看看依赖注入方法能不能按照套路方式点出来...
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseFreeSql(orm: null);
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseFreeSql(orm: null);

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
                });

            //ServiceProvider serviceProvider = services.BuildServiceProvider();
            //serviceProvider.GetService<IdentityServer>()
        }
    }
}
