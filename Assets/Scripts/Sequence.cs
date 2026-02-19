using UnityEngine;

public class Sequence : Node
{
    public Sequence(string name)
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

        if (childStatus == Status.Failure)
        {
            return childStatus;
        }
        
        CurrentChild++;
        if (CurrentChild >= Children.Count)
        {
            CurrentChild = 0;
            return Status.Success;
        }
        
        return Status.Running;
    }
}
