using System.Numerics;
using UnityEngine;


public class Cop : BehaviourTreeAgent
{
    public GameObject[] patrolPoints;
    public GameObject robber;

    public override void Start()
    {
        base.Start();

        Sequence selectPatrolPoint = new Sequence("Select Patrol Point");
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Leaf patrolToPoint = new Leaf("Patrol To Point " + patrolPoints[i].name, i, GoToPoint);
            selectPatrolPoint.AddChild(patrolToPoint);
        }

        Sequence chaseRobber = new Sequence("Chase Robber");
        Leaf canSeeRobber = new Leaf("Can See Robber?", CanSeeRobber);
        Leaf chase = new Leaf("Chase Robber", ChaseRobber);

        chaseRobber.AddChild(canSeeRobber);
        chaseRobber.AddChild(chase);

        Invertor cantSeeRobber = new Invertor("Can't See Robber");
        cantSeeRobber.AddChild(canSeeRobber);

        BehaviourTree patrolCondition = new BehaviourTree();
        Sequence condition = new Sequence("Condition");
        condition.AddChild(cantSeeRobber);
        patrolCondition.AddChild(condition);
        DependencySequence patrol = new DependencySequence("Patrol Until See Robber", patrolCondition, agent);
        patrol.AddChild(selectPatrolPoint);

        Selector beCop = new Selector("Be Cop");
        beCop.AddChild(patrol);
        beCop.AddChild(chaseRobber);

        Tree.AddChild(beCop);
    }

    public Node.Status GoToPoint(int i)
    {
        Node.Status status = GoToLocation(patrolPoints[i].transform.position);
        return status;
    }

    public Node.Status CanSeeRobber()
    {
        return CanSee(robber.transform.position, "Robber", 5, 60);
    }

    UnityEngine.Vector3 lastLocation;
    public Node.Status ChaseRobber()
    {
        float chaseDistace = 10;
        if (state == ActionState.Idle)
        {
            lastLocation = this.transform.position - (transform.position - robber.transform.position).normalized * chaseDistace;
        }
        return GoToLocation(lastLocation);
    }

}
