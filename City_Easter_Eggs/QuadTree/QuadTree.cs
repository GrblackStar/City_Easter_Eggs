﻿#region Using

using NuGet.Packaging;
using System.Collections;

#endregion

namespace City_Easter_Eggs.QuadTree;

public class QuadTree<T> : QuadTreeNode<T> where T : IQuadTreeObject
{
    private List<T> _allObjects = new List<T>();

    public QuadTree(Rectangle bounds) : base(null, bounds)
    {
    }

    public void UpdateObject(T obj)
    {
        for (int i = 0; i < _allObjects.Count; i++)
        {
            var objInList = _allObjects[i];
            if (objInList.Equals(obj))
            {
                RemoveObject(obj);
                AddObject(obj);
                break;
            }
        }
    }

    public override QuadTreeNode<T> AddObject(T obj)
    {
        _allObjects.Add(obj);
        return base.AddObject(obj);
    }

    public override void RemoveObject(T obj)
    {
        _allObjects.Remove(obj);
        base.RemoveObject(obj);
    }

    // non recursive tree walking algorithm using stack
    public void GetObjectsIntersectingShape(IList<T> list, IQuadTreeQueryShape shape)
    {
        Stack<QuadTreeNode<T>> stack = new Stack<QuadTreeNode<T>>();
        stack.Push(this);

        while (stack.Count > 0)
        {
            var temp = stack.Pop();

            if (temp.Objects == null) continue;
            for (var i = 0; i < temp.Objects.Count; i++)
            {
                T obj = temp.Objects[i];
                Rectangle bounds = obj.GetBounds();
                if (!shape.IntersectsBounds(bounds)) continue;

                list.Add(obj);
            }

            if (temp.ChildNodes == null) continue;
            for (int i = 0; i < temp.ChildNodes.Length; i++)
            {
                QuadTreeNode<T>? node = temp.ChildNodes[i];
                if (shape.IntersectsBounds(node.Bounds)) stack.Push(node);
            }
        }
    }

    public void GetAllObjects(IList<T> list)
    {
        list.AddRange(_allObjects);
    }
}