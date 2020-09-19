using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using sample.Abstract.IREPO;
using sample.Implementation;
using sample.Abstract;

namespace sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(p =>
            {
                p.AddPolicy("cors",
                c =>
                {
                    c
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();

                });
            });
            services.AddControllers();

            services.AddScoped<IDbConnection>(db => new SqlConnection(Configuration.GetConnectionString("dbCon")));

            services.AddTransient(typeof(IRepo<>), typeof(Repository<>));
            services.AddTransient<IEmployee, EmployeeRepo>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors("cors");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
