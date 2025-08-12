using Quartz;
using Simulation.Common.constants;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.PortManager;
using Simulation.Services.Quartz;
using Simulation.Services.UAVManager;

namespace Simulation.Services
{
    public static class ServiceCollectionExtensions
    {
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
