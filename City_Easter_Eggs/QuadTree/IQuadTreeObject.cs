#region Using

using System.Numerics;

#endregion

#nullable enable

namespace City_Easter_Eggs.QuadTree;

public interface IQuadTreeObject
{
    public Rectangle GetBounds()
    {
        Vector2 objectPos = GetPosition();
        return new Rectangle(objectPos, new Vector2(1, 1));
    }

    public Vector2 GetPosition();
}
