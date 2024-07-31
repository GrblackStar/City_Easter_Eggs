using City_Easter_Eggs.QuadTree;

public struct Ellipse : IQuadTreeQueryShape
{
    public float X;
    public float Y;
    public float RadiusX;
    public float RadiusY;

    public Ellipse(float x, float y, float rx, float ry)
    {
        X = x; 
        Y = y; 
        RadiusX = rx; 
        RadiusY = ry;
    }

    public bool IntersectsBounds(Rectangle rect)
    {
        float rectRight = rect.X + rect.Width;
        float rectBottom = rect.Y + rect.Height;

        float closestX = Math.Clamp(X, rect.X, rectRight);
        float closestY = Math.Clamp(Y, rect.Y, rectBottom);

        float distanceX = closestX - X;
        float distanceY = closestY - Y;

        float distanceXSquared = distanceX * distanceX;
        float distanceYSquared = distanceY * distanceY;
        float radiusXSquared = RadiusX * RadiusX;
        float radiusYSquared = RadiusY * RadiusY;

        return (distanceXSquared / radiusXSquared) + (distanceYSquared / radiusYSquared) <= 1;
    }

}
