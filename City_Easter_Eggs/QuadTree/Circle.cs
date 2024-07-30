using System.Numerics;
using System.Text.RegularExpressions;

namespace City_Easter_Eggs.QuadTree;

public struct Circle : IQuadTreeQueryShape
{
    public float X;
    public float Y;
    public float Radius;

    public Circle(float x, float y, float radius)
    {
        X = x;
        Y = y;
        Radius = radius;
    }

    public bool IntersectsBounds(Rectangle bounds)
    {
        return Vector2.Distance(new Vector2(X, Y), bounds.Position) < Radius;
    }
}