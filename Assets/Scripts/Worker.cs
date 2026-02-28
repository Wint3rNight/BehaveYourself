using UnityEngine;

public class Worker : BehaviourTreeAgent
{
    public GameObject office;
    public GameObject visitor;
    public override void Start()
    {
        base.Start();
        Leaf visitorStillWaiting = new Leaf(name + " Visitor Still Waiting?", VisitorWaiting);
        Leaf allocateVisitor = new Leaf(name + " Allocate Visitor", AllocateVisitor);
        Leaf goToVisitor = new Leaf(name + " Go To Visitor", GoToVisitor);
        Leaf goToOffice = new Leaf(name + " Go To Office", GoToOffice);

        Sequence getVisitor = new Sequence(name + " Get Visitor");
        getVisitor.AddChild(allocateVisitor);

        BehaviourTree waiting = new BehaviourTree();
        waiting.AddChild(visitorStillWaiting);

        DependencySequence moveToVisitor = new DependencySequence("Move To Visitor", waiting, agent);
        moveToVisitor.AddChild(goToVisitor);

        getVisitor.AddChild(moveToVisitor);

        Selector beWorker = new Selector("Be Worker");
        beWorker.AddChild(getVisitor);
        beWorker.AddChild(goToOffice);

        Tree.AddChild(beWorker);
    }

    public Node.Status VisitorWaiting()
    {
        if (visitor != null && visitor.GetComponent<VisitorBehaviour>().isWaiting)
        {
            return Node.Status.Success;
        }
        return Node.Status.Failure;
    }

    public Node.Status AllocateVisitor()
    {
        if (Blackboard.Instance.visitors.Count == 0)
        {
            return Node.Status.Failure;
        }

        visitor = Blackboard.Instance.visitors.Pop();
        if (visitor == null)
        {
            return Node.Status.Failure;
        }

        return Node.Status.Success;
    }

    public Node.Status GoToVisitor()
    {
        if (visitor == null)
        {
            return Node.Status.Failure;
        }

        Node.Status status = GoToLocation(visitor.transform.position);

        if (status == Node.Status.Success)
        {
            visitor.GetComponent<VisitorBehaviour>().ticket = true;
            visitor = null;
        }
        return status;
    }

    public Node.Status GoToOffice()
    {
        Node.Status status = GoToLocation(office.transform.position);
        visitor = null;
        return status;
    }
}
