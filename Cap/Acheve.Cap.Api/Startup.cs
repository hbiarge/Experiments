using System;
using System.Collections.Generic;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Acheve.Cap.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCap(options =>
            {
                options.UseDashboard();

                options.UseRabbitMQ(setup =>
                {
                    setup.HostName = "rabbit";
                    setup.Port = 5672;
                    setup.UserName = "guest";
                    setup.Password = "guest";
                });

                options.UseSqlServer(setup =>
                {
                    setup.ConnectionString = @"Server=database;User Id=sa;Password=P@ssw0rd;Initial Catalog=cap.sample.producer;MultipleActiveResultSets=true;";
                    setup.Schema = "producer";
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
               {
                   var bus = context.RequestServices.GetRequiredService<ICapPublisher>();
                   //var sqlServerOptions = context.RequestServices.GetRequiredService<IOptions<SqlServerOptions>>();

                   //using (var connection = new SqlConnection(sqlServerOptions.Value.ConnectionString))
                   //using (var transaction = connection.BeginTransaction(bus, autoCommit: false))
                   //{
                   //    connection.Execute("insert into test(name) values('test')", transaction);
                   //    bus.Publish("sample.rabbitmq.mysql", DateTime.Now);

                   //    transaction.Commit();
                   //}

                   await bus.PublishAsync("test.show.time", DateTime.UtcNow, new Dictionary<string, string>(), context.RequestAborted);

                   context.Response.StatusCode = 204;
               });
            });
        }
    }
}
