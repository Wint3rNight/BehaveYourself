using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public enum Status 
    {
        Success,
        Failure,
        Running
    }
    public Status status;
    public List<Node> Children = new List<Node>();
    
    public int CurrentChild = 0;
    public string Name;
    public int SortOrder;
    public Node(){}
    
    public Node(string n)
    {
        Name = n;
    }

    public Node(string n, int sortOrder)
    {
        Name = n;
        SortOrder = sortOrder;
    }

    public void Reset()
    {
        foreach (Node n in Children)
        {
            n.Reset();
        }
        CurrentChild = 0;
    }
    
    public virtual Status Process()
    {
        return Children[CurrentChild].Process();
    }
    
    public void AddChild(Node n)
    {
        Children.Add(n);
    }
}
