using System.Drawing;

namespace City_Easter_Eggs.QuadTree;

public static class MathHelper
{
    public static float MetersToEarthRadiusRadian(float meters)
    {
        var r2d = 180f / MathF.PI; // radians to degrees 
        var earthsradius = 6371f * 1000f;

        // find the raidus in lat/lon 
        return (meters / earthsradius) * r2d;
    }
}
