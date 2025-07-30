namespace Simulation.Services.helpers
{
    public static class UnitConversionHelper
    {
        public static double ToRadians(this double degrees) => degrees * (Math.PI / 180.0);

        public static double ToDegrees(this double radians) => radians * (180.0 / Math.PI);
    }
}
