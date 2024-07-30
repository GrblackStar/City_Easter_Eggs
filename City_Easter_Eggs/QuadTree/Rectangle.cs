using System.Numerics;

namespace City_Easter_Eggs.QuadTree;

public struct Rectangle : IQuadTreeQueryShape
{
    public Vector2 Position;
    public Vector2 Size;

    public float X => Position.X;
    public float Y => Position.Y;
    public float Width => Size.X;
    public float Height => Size.Y;

    public Rectangle(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }

    public Rectangle(float x, float y, float w, float h)
    {
        Position = new Vector2(x, y);
        Size = new Vector2(w, h);
    }

    public bool ContainsInclusive(Vector2 value)
    {
        return X <= value.X && value.X <= X + Width && Y <= value.Y && value.Y <= Y + Height;
    }

    public bool ContainsInclusive(Rectangle value)
    {
        return X <= value.X && value.X + value.Width <= X + Width && Y <= value.Y && value.Y + value.Height <= Y + Height;
    }

    public bool IntersectsBounds(Rectangle bounds)
    {
        return bounds.X < X + Width &&
               X < bounds.X + bounds.Width &&
               bounds.Y < Y + Height &&
               bounds.Y < bounds.Y + bounds.Height;
    }
}
