using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;
using CentronSoftware.Centron.WebServices.Connections;
using Helpo.Services.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor;
using MudBlazor.Services;
using Raven.Client.Documents;
using System;
using System.IO;
using Helpo.Services.Questions;
using Raven.Client.Documents.Indexes;

namespace Helpo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddControllers();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/signin";
                        options.LogoutPath = "/signout";
                    });
            
            services.AddMudBlazorDialog();
            services.AddMudBlazorSnackbar();
            services.AddMudBlazorResizeListener();

            services.AddSingleton<IDocumentStore>(serviceProvider =>
            {
                var certificateData = this.Configuration.GetValue<string>("RavenDB:ClientCertificateData");
                var certificatePath = this.Configuration.GetValue<string>("RavenDB:ClientCertificatePath");
                var certificatePassword = this.Configuration.GetValue<string>("RavenDB:ClientCertificatePassword");

                var certificate = string.IsNullOrWhiteSpace(certificatePath) == false && File.Exists(certificatePath)
                    ? new X509Certificate2(certificatePath, certificatePassword)
                    : new X509Certificate2(Convert.FromBase64String(certificateData), certificatePassword, X509KeyStorageFlags.MachineKeySet);

                var store = new DocumentStore();
                store.Certificate = certificate;
                store.Urls = new[] {this.Configuration.GetValue<string>("RavenDB:Url")};
                store.Database = this.Configuration.GetValue<string>("RavenDB:DatabaseName");

                store.Initialize();
                
                IndexCreation.CreateIndexes(this.GetType().Assembly, store);

                return store;
            });

            services.AddSingleton(serviceProvider =>
            {
                var url = this.Configuration.GetValue<string>("CentronWebService:Url");
                return new CentronWebService(url);
            });
            
            services.AddScoped<AuthService>();
            services.AddScoped<QuestionsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
