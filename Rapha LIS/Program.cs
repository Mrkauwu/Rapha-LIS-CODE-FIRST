using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rapha_LIS.Data;
using Rapha_LIS.Models;
using Rapha_LIS.Presenters;
using Rapha_LIS.Repositories;
using Rapha_LIS.Views;

namespace Rapha_LIS
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.SetBasePath(AppContext.BaseDirectory)
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    // Register EF Core DbContext
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                    // Repositories
                    services.AddTransient<ISigninRepository, UserRepository>();
                    services.AddTransient<IPatientControlRepository, PatientRepository>();
                    services.AddTransient<IAnalyticsRepository, PatientRepository>();
                    services.AddTransient<ITestListRepository, PatientRepository>();
                    services.AddTransient<IUserControlRepository, UserRepository>();

                    // Views
                    services.AddTransient<SigninView>();
                    services.AddTransient<ISigninView, SigninView>();      // <-- Add this
                    services.AddTransient<TestListView>();
                    services.AddTransient<ITestListView, TestListView>();  // <-- And add this
                    services.AddSingleton<Rapha_LIS.Views.Rapha_LIS>();

                    // Interface bindings to main form
                    services.AddSingleton<IPatientControlView>(p => p.GetRequiredService<Rapha_LIS.Views.Rapha_LIS>());
                    services.AddSingleton<IPatientAnalyticsView>(p => p.GetRequiredService<Rapha_LIS.Views.Rapha_LIS>());
                    services.AddSingleton<IUserControlView>(p => p.GetRequiredService<Rapha_LIS.Views.Rapha_LIS>());

                    // Presenters
                    services.AddTransient<SigninPresenter>();
                    services.AddTransient<PatientPresenter>();
                    services.AddTransient<UserPresenter>();
                })
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                //DATABASE INITIALIZER(Call Data Seeding)
                var context = services.GetRequiredService<AppDbContext>();
                DbInitializer.Initialize(context); 

                // Show sign-in form
                var signinView = services.GetRequiredService<SigninView>();
                var signinRepo = services.GetRequiredService<ISigninRepository>();
                var signinPresenter = new SigninPresenter(signinView, signinRepo);

                if (signinView.ShowDialog() == DialogResult.OK)
                {
                    var mainForm = services.GetRequiredService<Rapha_LIS.Views.Rapha_LIS>();

                    // Initialize presenters
                    var patientRepo = services.GetRequiredService<IPatientControlRepository>();
                    var patientAnalyticsRepo = services.GetRequiredService<IAnalyticsRepository>();
                    var testListView = services.GetRequiredService<TestListView>();
                    var testListRepo = services.GetRequiredService<ITestListRepository>();

                    new PatientPresenter(
                        (IPatientControlView)mainForm,
                        patientRepo,
                        (IPatientAnalyticsView)mainForm,
                        patientAnalyticsRepo,
                        testListView,
                        testListRepo
                    );

                    var userRepo = services.GetRequiredService<IUserControlRepository>();
                    new UserPresenter((IUserControlView)mainForm, userRepo);

                    Application.Run(mainForm);
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }
}
