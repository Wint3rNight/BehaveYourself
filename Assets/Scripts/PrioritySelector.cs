using System.Collections.Generic;
using UnityEngine;

public class PrioritySelector : Node
{
    Node[] _nodeArray;
    bool _isOrdered = false;
    
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
        if (!_isOrdered)
        {
            OrderNodes();
            _isOrdered = true;
        }
        
        Status childStatus = Children[CurrentChild].Process();
        if (childStatus == Status.Running)
        {
            return Status.Running;
        }

        if (childStatus == Status.Success)
        {
            //Children[CurrentChild].SortOrder = 1;
            CurrentChild = 0;
            _isOrdered = false;
            return Status.Success;
        }
        /*else
        {
            Children[CurrentChild].SortOrder = 10;
        }*/

        CurrentChild++;
        if (CurrentChild >= Children.Count)
        {
            CurrentChild = 0;
            _isOrdered = false;
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
                (array[lowIndex], array[j]) = (array[j], array[lowIndex]);
            }
        }
        (array[lowIndex + 1], array[high]) = (array[high], array[lowIndex + 1]);
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
