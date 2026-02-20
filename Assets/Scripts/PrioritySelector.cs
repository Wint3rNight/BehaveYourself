using System.Collections.Generic;
using UnityEngine;

public class PrioritySelector : Node
{
    Node[] _nodeArray;
    
    public PrioritySelector(string name)
    {
        Name = name;
    }
    
    public void OrderNodes()
    {
        _nodeArray = Children.ToArray();
        Sort(_nodeArray, 0, Children.Count - 1);
        Children = new List<Node>(_nodeArray);
    }

    public override Status Process()
    {
        OrderNodes();
        
        Status childStatus = Children[CurrentChild].Process();
        if (childStatus == Status.Running)
        {
            return Status.Running;
        }

        if (childStatus == Status.Success)
        {
            CurrentChild = 0;
            return Status.Success;
        }

        CurrentChild++;
        if (CurrentChild >= Children.Count)
        {
            CurrentChild = 0;
            return Status.Failure;
        }
        
        return Status.Running;
    }

    int Partition(Node[] array, int low, int high)
    {
        Node pivot = array[high];
        int lowIndex = low - 1;

        for (int j = low; j < high; j++)
        {
            if(array[j].SortOrder < pivot.SortOrder)
            {
                lowIndex++;
                Node temp = array[lowIndex];
                array[lowIndex] = array[j];
                array[j] = temp;
            }
        }
        Node temp1 = array[lowIndex + 1];
        array[lowIndex + 1] = array[high];
        array[high] = temp1;
        return lowIndex + 1;
    }
    
    void Sort(Node[] array, int low, int high){
        if (low < high)
        {
            int partitionIndex = Partition(array, low, high);
            Sort(array, low, partitionIndex - 1);
            Sort(array, partitionIndex + 1, high);
        }
    }
}
