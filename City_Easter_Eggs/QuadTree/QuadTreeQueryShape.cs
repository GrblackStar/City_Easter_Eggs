namespace City_Easter_Eggs.QuadTree;

public interface IQuadTreeQueryShape
{
    public bool IntersectsBounds(Rectangle bounds);
}
