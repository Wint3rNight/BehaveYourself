using UnityEngine;

public class Worker : BehaviourTreeAgent
{
    public GameObject office;
    public GameObject visitor;
    public override void Start()
    {
        base.Start();
        Leaf goToVisitor = new Leaf("Go To Visitor", GoToVisitor);
        Leaf goToOffice = new Leaf("Go To Office", GoToOffice);
        Selector beWorker = new Selector("Be Worker");
        beWorker.AddChild(goToVisitor);
        beWorker.AddChild(goToOffice);

        Tree.AddChild(beWorker);
    }

    public Node.Status GoToVisitor()
    {
        if(Blackboard.Instance.visitors.Count == 0)
        {
            return Node.Status.Failure;
        }

        visitor = Blackboard.Instance.visitors.Pop();
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
        return status;
    }
}
