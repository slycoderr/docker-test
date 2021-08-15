using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.HttpOverrides;

namespace webapp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"hello world");
                });
                endpoints.MapGet("/db", async context =>
                {
                    var client = new MongoClient("mongodb://localhost:27017");
                    var database = client.GetDatabase("test");
                    var collection = database.GetCollection<BsonDocument>("123");
                    var collections = database.ListCollectionNames().ToList();
                    var sqlDbs = new List<string>();
                    var mongo = $"<p>{string.Join(',', collections)}</p>";

                    using (var conn = new MySqlConnection("server=127.0.0.1;uid=root;pwd=root"))
                    {
                        using (var command = conn.CreateCommand())
                        {
                            command.CommandText = "create database if not exists testdb;";
                            await command.ExecuteNonQueryAsync();

                            command.CommandText = "SHOW DATABASES;";
                            await command.ExecuteNonQueryAsync();

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    sqlDbs.Add(reader.GetString(0));
                                }
                            }
                        }
                    }

                    var sql = $"<p>{string.Join(',', sqlDbs)}</p>";

                    await context.Response.WriteAsync($"{sql}{mongo}");
                });
            });
        }
    }
}
