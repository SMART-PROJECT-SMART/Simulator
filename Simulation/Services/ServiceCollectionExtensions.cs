using Quartz;
using Simulation.Common.constants;
using Simulation.Configuration;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.ICDDirectory;
using Simulation.Services.PortManager;
using Simulation.Services.Quartz;
using Simulation.Services.UAVManager;

namespace Simulation.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApi();
            return services;
        }

        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ICDSettings>(
                config.GetSection(SimulationConstants.Config.ICD_DIRECTORY)
            );
            return services;
        }

        public static IServiceCollection AddIcdDirectoryServices(this IServiceCollection services)
        {
            services.AddSingleton<IICDDirectory, ICDDirectory.ICDDirectory>();
            return services;
        }

        public static IServiceCollection AddFlightPathCalculators(this IServiceCollection services)
        {
            services.AddSingleton<IMotionCalculator, MotionCalculator>();
            services.AddSingleton<ISpeedController, SpeedCalculator>();
            services.AddSingleton<IOrientationCalculator, OrientationCalculator>();
            return services;
        }

        public static IServiceCollection AddFlightPathServices(this IServiceCollection services)
        {
            services.AddTransient<FlightPathService>();
            return services;
        }

        public static IServiceCollection AddQuartzServices(this IServiceCollection services)
        {
            services.AddQuartz();
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.AddSingleton(provider =>
                provider.GetRequiredService<ISchedulerFactory>()
                    .GetScheduler().GetAwaiter().GetResult()
            );
            services.AddSingleton<IQuartzFlightJobManager, QuartzFlightJobManager>();
            return services;
        }

        public static IServiceCollection AddManagementServices(this IServiceCollection services)
        {
            services.AddSingleton<IUAVManager, UAVManager.UAVManager>();
            services.AddSingleton<IPortManager, PortManager.PortManager>();
            return services;
        }
    }
}
