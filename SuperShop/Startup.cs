using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;

namespace SuperShop
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
            services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false; // obrigatoriedade para as passwords terem varios caracters, numeros etc... COLOCAR A TRUE
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<DataContext>(); // separa o datacontext do "dele" do "meu"
                            //depois da pessoa fazer o login usa o DataContext "simples", o dele tem mais seguran�a

            services.AddDbContext<DataContext>(cfg => // cria um servi�o do DataContext, injeto o meu
            {
                cfg.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"));   // usar o sqlsever com esta connection string
            });

            services.AddTransient<SeedDb>(); // quando alguem chamar pelo SeedDb cria-o na factory do Program.cs
                                             //AddTransient -> Usa e deita fora (deixa de ficar em mem�ria) e n�o pode ser mais usado 
                                             //services.AddScoped<IRepository, Repository>(); // dependecy injection, assim que for preciso um repositorio ele cria sem ser preciso instanciar
            services.AddScoped<IUserHelper, UserHelper>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddControllersWithViews();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // Middleware (Sistema operativo que mant�m a aplica��o)
                                     // Aten��o � sequ�ncia 
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
