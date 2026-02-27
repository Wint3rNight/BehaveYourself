using System;
using UnityEngine;
using UnityEngine.AI;

public class VisitorBehaviour : BehaviourTreeAgent
{
    public GameObject[] art;
    public GameObject frontDoor;
    public GameObject homeBase;

    [Range(0,1000)]
    public int boredomThreshold = 0;

    public override void Start()
    {
        base.Start();
        RandomSelector selectObjectToSteal = new RandomSelector("Select Art To Look At");
        for (int i = 0; i < art.Length; i++)
        {
            Leaf goToArt = new Leaf("Go To Art "+art[i].name, i,GoToArt);
            selectObjectToSteal.AddChild(goToArt);
        }

        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToHomeBase = new Leaf("Go To Home Base", GoToHomeBase);
        Leaf isBored = new Leaf("Is Bored?", IsBored);

        Sequence viewArt = new Sequence("View Art");
        viewArt.AddChild(isBored);
        viewArt.AddChild(goToFrontDoor);
        viewArt.AddChild(selectObjectToSteal);
        viewArt.AddChild(goToHomeBase);

        Selector beVisitor = new Selector("Be Visitor");
        beVisitor.AddChild(viewArt);

        Tree.AddChild(beVisitor);
    }

    private Node.Status GoToArt(int i)
    {
        if (!art[i].activeSelf)
        {
            return Node.Status.Failure;
        }
        Node.Status status = GoToLocation(art[i].transform.position);
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
        return status;
    }

    public Node.Status IsBored()
    {
        if(boredomThreshold <= 100)
        {
            return Node.Status.Failure;
        }
        else
        {
            return Node.Status.Success;
        }
    }
}
