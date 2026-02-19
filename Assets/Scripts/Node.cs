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
    public Node(){}
    
    public Node(string n)
    {
        Name = n;
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
