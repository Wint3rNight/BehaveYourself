using UnityEngine;

public class Selector : Node
{
    public Selector(string name)
    {
        Name = name;
    }

    public override Status Process()
    {
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
}
