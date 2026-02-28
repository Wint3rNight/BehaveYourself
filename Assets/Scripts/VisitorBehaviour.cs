using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class VisitorBehaviour : BehaviourTreeAgent
{
    public GameObject[] art;
    public GameObject frontDoor;
    public GameObject homeBase;

    [Range(0, 1000)]
    public int boredomThreshold = 0;

    public bool ticket = false;
    public bool isWaiting = false;

    public override void Start()
    {
        base.Start();
        RandomSelector selectObject = new RandomSelector("Select Art To Look At");
        for (int i = 0; i < art.Length; i++)
        {
            Leaf goToArt = new Leaf(name + " Go To Art " + art[i].name, i, GoToArt);
            selectObject.AddChild(goToArt);
        }

        Leaf goToFrontDoor = new Leaf(name + " Go To Front Door", GoToFrontDoor);
        Leaf goToHomeBase = new Leaf(name + " Go To Home Base", GoToHomeBase);
        Leaf isBored = new Leaf(name + " Is Bored?", IsBored);
        Leaf isOpen = new Leaf(name + " Is Open?", IsOpen);

        Sequence viewArt = new Sequence(name + " View Art");
        viewArt.AddChild(isOpen);
        viewArt.AddChild(isBored);
        viewArt.AddChild(goToFrontDoor);

        Leaf noTicket = new Leaf(name + " No Ticket?", NoTicket);
        Leaf isWaiting = new Leaf(name + " Is Waiting?", IsWaiting);

        BehaviourTree waitingForTicket = new BehaviourTree();
        waitingForTicket.AddChild(noTicket);

        Loop getTicket = new Loop(name + " Get Ticket", waitingForTicket);
        getTicket.AddChild(isWaiting);

        viewArt.AddChild(getTicket);

        BehaviourTree whileBored = new BehaviourTree();
        whileBored.AddChild(isBored);

        Loop lookAtArt = new Loop(name + " Look At Art", whileBored);
        lookAtArt.AddChild(selectObject);

        viewArt.AddChild(lookAtArt);
        viewArt.AddChild(goToHomeBase);

        BehaviourTree gallaryOpenCondition = new BehaviourTree();
        gallaryOpenCondition.AddChild(isOpen);
        DependencySequence beVisitor = new DependencySequence(name + " Be Visitor", gallaryOpenCondition, agent);
        beVisitor.AddChild(viewArt);

        Selector viewArtWithFallback = new Selector(name + " View Art With Fallback");
        viewArtWithFallback.AddChild(beVisitor);
        viewArtWithFallback.AddChild(goToHomeBase);

        Tree.AddChild(viewArtWithFallback);

        StartCoroutine(IncreaseBoredom());
    }

    IEnumerator IncreaseBoredom()
    {
        while (true)
        {
            boredomThreshold = Mathf.Clamp(boredomThreshold + 20, 0, 1000);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));
        }
    }

    private Node.Status GoToArt(int i)
    {
        if (!art[i].activeSelf)
        {
            return Node.Status.Failure;
        }
        Node.Status status = GoToLocation(art[i].transform.position);
        if (status == Node.Status.Success)
        {
            boredomThreshold = Mathf.Clamp(boredomThreshold - 150, 0, 1000);
        }
        return status;
    }

    private Node.Status GoToFrontDoor()
    {
        Node.Status status = GoToDoor(frontDoor);
        return status;
    }

    private Node.Status GoToHomeBase()
    {
        Node.Status status = GoToLocation(homeBase.transform.position);
        isWaiting = false;
        return status;
    }

    public Node.Status IsBored()
    {
        if (boredomThreshold <= 100)
        {
            return Node.Status.Failure;
        }
        else
        {
            return Node.Status.Success;
        }
    }

    public Node.Status NoTicket()
    {
        if (ticket || IsOpen() == Node.Status.Failure)
        {
            return Node.Status.Failure;
        }
        else
        {
            return Node.Status.Success;
        }
    }

    public Node.Status IsWaiting()
    {
        if (Blackboard.Instance.RegisterVisitor(this.gameObject))
        {
            isWaiting = true;
            return Node.Status.Success;
        }
        else
        {
            return Node.Status.Failure;
        }
    }
}
