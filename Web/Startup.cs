using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using vladandartem.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using vladandartem.Data.Models;
using AutoMapper;
using vladandartem.Mapping;

namespace vladandartem
{
    public class DateTimeModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            // Try to fetch the value of the argument by name
            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var dateStr = valueProviderResult.FirstValue;
            // Here you define your custom parsing logic, i.e. using "de-DE" culture
            if (!DateTime.TryParse(dateStr, new CultureInfo("de-DE"), DateTimeStyles.None, out DateTime date))
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "DateTime should be in format 'dd.MM.yyyy HH:mm:ss'");
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(date);
            return Task.CompletedTask;
        }
    }

    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(DateTime) ||
                context.Metadata.ModelType == typeof(DateTime?))
            {
                return new DateTimeModelBinder();
            }

            return null;
        }
    }

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".MyApp.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(10000);
                options.Cookie.IsEssential = true;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            string connection = Configuration.GetConnectionString("DefaultConnection");
            string usersConnection = Configuration.GetConnectionString("UsersConnection");

            services.AddDbContext<ProductContext>(options => options.UseSqlServer(connection, b => b.MigrationsAssembly("vladandartem")));
            //services.AddDbContext<UserContext>(options => options.UseSqlServer(usersConnection));

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<ProductContext>()
                .AddDefaultTokenProviders();

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });
            //services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<UserContext>();

            //services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc(options =>
            {
                options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Online-Store API",
                    Description = ":D",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Shayne Boyer",
                        Email = string.Empty,
                        Url = "https://twitter.com/spboyer"
                    },
                    License = new License
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
