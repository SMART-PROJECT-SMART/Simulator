using Quartz;
using Core.Common;
using Core.Configuration;
using Simulation.Common.constants;
using Simulation.Services.Flight_Path;
using Simulation.Services.Flight_Path.Motion_Calculator;
using Simulation.Services.Flight_Path.Orientation_Calculator;
using Simulation.Services.Flight_Path.Speed_Controller;
using Simulation.Services.PortManager;
using Simulation.Services.Quartz;
using Simulation.Services.TelemetryDeviceClient;
using Simulation.Services.MissionServiceClient;
using Simulation.Services.UAVManager;
using Simulation.Services.DeviceManagerClient;
using Simulation.Services.UAVStorage;
using Simulation.Services.UAVChangeHandlers;
using Simulation.Services.UAVFactory;

namespace Simulation.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddLogging();
            return services;
        }

        public static IServiceCollection AddHttpClients(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            string telemetryDeviceBaseUrl = configuration[
                $"{SimulationConstants.Config.TELEMETRY_DEVICE_SECTION}:BaseUrl"
            ];

            services
                .AddHttpClient(
                    SimulationConstants.HttpClients.TELEMETRY_DEVICE_HTTP_CLIENT,
                    client =>
                    {
                        client.BaseAddress = new Uri(telemetryDeviceBaseUrl);
                    }
                );

            string missionServiceBaseUrl = configuration[
                $"{SimulationConstants.Config.MISSION_SERVICE_SECTION}:BaseUrl"
            ];

            services
                .AddHttpClient(
                    SimulationConstants.HttpClients.MISSION_SERVICE_HTTP_CLIENT,
                    client =>
                    {
                        client.BaseAddress = new Uri(missionServiceBaseUrl);
                    }
                );

            string deviceManagerBaseUrl = configuration[
                $"{SimulationConstants.Config.DEVICE_MANAGER_SECTION}:BaseUrl"
            ];

            services
                .AddHttpClient(
                    SimulationConstants.HttpClients.DEVICE_MANAGER_HTTP_CLIENT,
                    client =>
                    {
                        client.BaseAddress = new Uri(deviceManagerBaseUrl);
                    }
                );

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
            services.AddSingleton<ITelemetryDeviceClient, TelemetryDeviceClient.TelemetryDeviceClient>();
            services.AddSingleton<IMissionServiceClient, MissionServiceClient.MissionServiceClient>();
            return services;
        }
        public static IServiceCollection AddSharedConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ICDSettings>(config.GetSection(SimulationConstants.Config.ICD_DIRECTORY));
            return services;
        }

        public static IServiceCollection AddDeviceManagerServices(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceManagerClient, DeviceManagerClient.DeviceManagerClient>();
            services.AddSingleton<UAVStorageService>();
            services.AddSingleton<IUAVStorageService>(sp => sp.GetRequiredService<UAVStorageService>());
            services.AddHostedService(sp => sp.GetRequiredService<UAVStorageService>());
            services.AddSingleton<IUAVChangeHandlerFactory, UAVChangeHandlerFactory>();
            services.AddSingleton<IUAVFactory, UAVFactory.UAVFactory>();
            return services;
        }
    }
}
