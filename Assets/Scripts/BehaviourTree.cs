using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : Node
{
    public BehaviourTree()
    {
        Name = "Tree";
    }

    public BehaviourTree(string n)
    {
        Name = n;
    }

    public override Status Process()
    {
        if (Children.Count == 0)
        {
            return Status.Success;
        }
        return Children[CurrentChild].Process();
    }

    struct NodeLevel
    {
        public int Level;
        public Node Node;
    }
    
    public void PrintTree()
    {
        string treePrintout = "";
        Stack<NodeLevel> nodeStack =new  Stack<NodeLevel>();
        Node currentNode = this;
        nodeStack.Push(new NodeLevel{Level = 0, Node = currentNode});
        
        while (nodeStack.Count != 0)
        {
            NodeLevel nextNode = nodeStack.Pop();
            treePrintout += new string('-', nextNode.Level)+nextNode.Node.Name + "\n";
            
            for(int i= nextNode.Node.Children.Count - 1; i >= 0; i--)
            {
                nodeStack.Push(new NodeLevel{Level = nextNode.Level+1,Node = nextNode.Node.Children[i]});
            }
        }
        
        Debug.Log(treePrintout);
    }
}
