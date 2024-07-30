#nullable enable

namespace City_Easter_Eggs.QuadTree;

public class QuadTreeNode<T> where T : IQuadTreeObject
{
    public Rectangle Bounds;
    public int Capacity;
    public int MaxDepth;

    public QuadTreeNode<T>? Parent;
    public QuadTreeNode<T>[]? ChildNodes;

    // Node objects if undivided, and objects which span multiple nodes if divided.
    public List<T>? Objects;

    public QuadTreeNode(QuadTreeNode<T>? parent, Rectangle bounds, int capacity = 3, int maxDepth = 5)
    {
        Parent = parent;
        Bounds = bounds;
        Capacity = capacity;
        MaxDepth = maxDepth;
    }

    private QuadTreeNode<T> GetNodeForBounds(Rectangle bounds)
    {
        if (ChildNodes == null) return this;

        for (var i = 0; i < ChildNodes.Length; i++)
        {
            QuadTreeNode<T> node = ChildNodes[i];
            if (node.Bounds.ContainsInclusive(bounds)) return node.GetNodeForBounds(bounds);
        }

        return this;
    }

    public QuadTreeNode<T> AddObject(T obj)
    {
        Rectangle bounds = obj.GetBounds();

		Objects ??= new List<T>();
        if (Objects.Count + 1 > Capacity && ChildNodes == null && MaxDepth > 0)
        {
            float halfWidth = Bounds.Size.X / 2;
            float halfHeight = Bounds.Size.Y / 2;

            ChildNodes = new QuadTreeNode<T>[4];
            ChildNodes[0] = new QuadTreeNode<T>(this, new Rectangle(Bounds.X, Bounds.Y, halfWidth, halfHeight), Capacity, MaxDepth - 1);
            ChildNodes[1] = new QuadTreeNode<T>(this, new Rectangle(Bounds.X + halfWidth, Bounds.Y, halfWidth, halfHeight), Capacity, MaxDepth - 1);
            ChildNodes[2] = new QuadTreeNode<T>(this, new Rectangle(Bounds.X, Bounds.Y + halfHeight, halfWidth, halfHeight), Capacity, MaxDepth - 1);
            ChildNodes[3] = new QuadTreeNode<T>(this, new Rectangle(Bounds.X + halfWidth, Bounds.Y + halfHeight, halfWidth, halfHeight), Capacity, MaxDepth - 1);

            QuadTreeNode<T> subNode = GetNodeForBounds(bounds);
            return subNode.AddObject(obj);
        }

        Objects.Add(obj);
        return this;
    }

    public void RemoveObject(T obj)
    {
        Objects?.Remove(obj);
    }
}