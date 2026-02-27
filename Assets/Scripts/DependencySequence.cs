using UnityEngine;
using UnityEngine.AI;

public class DependencySequence : Node
{
    private BehaviourTree _dependency;
    private NavMeshAgent _agent;
    public DependencySequence(string name, BehaviourTree d, NavMeshAgent a)
    {
        Name = name;
        _dependency = d;
        _agent = a;
    }

    public override Status Process()
    {
        if (_dependency.Process()==Status.Failure)
        {
            _agent.ResetPath();
            foreach (Node n in Children)
            {
                n.Reset();
            }
            return Status.Failure;
        }
        
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
