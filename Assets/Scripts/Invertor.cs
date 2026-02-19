using UnityEngine;

public class Invertor : Node
{
    public Invertor(string name)
    {
        Name = name;
    }

    public override Status Process()
    {
        Status childStatus = Children[0].Process();
        if (childStatus == Status.Running)
        {
            return Status.Running;
        }

        if (childStatus == Status.Failure)
        {
            return Status.Success;
        }
        else
        {
            return Status.Failure;
        }
    }
}
