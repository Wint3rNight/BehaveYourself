using UnityEngine;

public class RandomSelector : Node
{
    bool _shuffled = false;
    public RandomSelector(string name)
    {
        Name = name;
    }

    public override Status Process()
    {
        if (!_shuffled)
        {
            Children.Shuffle();
            _shuffled = true;
        }
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
