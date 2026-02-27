using UnityEngine;

public class Loop : Node
{
    BehaviourTree dependency;
    public Loop(string name, BehaviourTree d)
    {
        Name = name;
        dependency = d;
    }

    public override Status Process()
    {
        if (dependency.Process() == Status.Failure)
        {
            return Status.Success;
        }
        Debug.Log("Sequence: " + Name + " processing child " + CurrentChild);
        Status childStatus = Children[CurrentChild].Process();
        if (childStatus == Status.Running)
        {
            return Status.Running;
        }

        if (childStatus == Status.Failure)
        {
            CurrentChild = 0;
            foreach (Node n in Children)
            {
                n.Reset();
            }
            return childStatus;
        }

        CurrentChild++;
        if (CurrentChild >= Children.Count)
        {
            CurrentChild = 0;
        }

        return Status.Running;
    }
}
