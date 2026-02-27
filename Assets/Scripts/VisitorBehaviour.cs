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

    [Range(0,1000)]
    public int boredomThreshold = 0;

    public override void Start()
    {
        base.Start();
        RandomSelector selectObject = new RandomSelector("Select Art To Look At");
        for (int i = 0; i < art.Length; i++)
        {
            Leaf goToArt = new Leaf("Go To Art "+art[i].name, i,GoToArt);
            selectObject.AddChild(goToArt);
        }

        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToHomeBase = new Leaf("Go To Home Base", GoToHomeBase);
        Leaf isBored = new Leaf("Is Bored?", IsBored);
        Leaf isOpen = new Leaf("Is Open?", IsOpen);

        Sequence viewArt = new Sequence("View Art");
        viewArt.AddChild(isOpen);
        viewArt.AddChild(isBored);
        viewArt.AddChild(goToFrontDoor);

        BehaviourTree whileBored = new BehaviourTree();
        whileBored.AddChild(isBored);

        Loop lookAtArt = new Loop("Look At Art", whileBored);
        lookAtArt.AddChild(selectObject);

        viewArt.AddChild(lookAtArt);
        viewArt.AddChild(goToHomeBase);

        BehaviourTree gallaryOpenCondition = new BehaviourTree();
        gallaryOpenCondition.AddChild(isOpen);
        DependencySequence beVisitor = new DependencySequence("Be Visitor", gallaryOpenCondition,agent);
        beVisitor.AddChild(viewArt);

        Selector viewArtWithFallback = new Selector("View Art With Fallback");
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
