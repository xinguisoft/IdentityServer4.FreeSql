using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using IdentityServer4.FreeSql.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using IdentityServer4.FreeSql;

namespace IdentityServer4.FreeSql.IntegrationTest
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var freeSqlC = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, @"Data Source=|DataDirectory|\idsr_freesql_config.db;Pooling=true;Max Pool Size=10")
                .UseAutoSyncStructure(true)
                .UseNoneCommandParameter(true)
                .UseMonitorCommand(cmd => Trace.WriteLine(cmd.CommandText))
                .Build<ConfigurationDbContext>();

            var freeSqlO = new FreeSqlBuilder()
                .UseConnectionString(DataType.Sqlite, @"Data Source=|DataDirectory|\idsr_freesql_op.db;Pooling=true;Max Pool Size=10")
                .UseAutoSyncStructure(true)
                .UseNoneCommandParameter(true)
                .UseMonitorCommand(cmd => Trace.WriteLine(cmd.CommandText))
                .Build<PersistedGrantDbContext>();

            services.AddSingleton<IFreeSql<ConfigurationDbContext>>(freeSqlC);
            services.AddSingleton<IFreeSql<PersistedGrantDbContext>>(freeSqlO);
            services.AddIdentityServer()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseFreeSql(orm: freeSqlC);
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseFreeSql(orm: freeSqlO);

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
