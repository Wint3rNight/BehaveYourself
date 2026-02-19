using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class RobberBehaviour : BehaviourTreeAgent
{
    public GameObject diamond;
    public GameObject van;
    public GameObject backDoor;
    public GameObject frontDoor;
    
    [Range(0,1000)]
    public int moneyStolen = 800;
    
    Node.Status _treeStatus = Node.Status.Running;
    
    new void Start()
    {        
        base.Start();
        Sequence steal = new Sequence("Steal");
        Leaf goToBank = new Leaf("Go To Bank", GoToBank);
        Leaf hasMoney = new Leaf("Has Money", HasMoney);
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Selector openDoor = new Selector("Open Door");
        
        Invertor invertMoney = new Invertor("Invert Money");
        invertMoney.AddChild(hasMoney);
        
        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);
        
        steal.AddChild(invertMoney);
        steal.AddChild(openDoor);
        steal.AddChild(goToBank);
        //steal.AddChild(goToBackDoor);
        steal.AddChild(goToVan);
        Tree.AddChild(steal);
        
        Tree.PrintTree();
    }
    
    public Node.Status HasMoney()
    {
        if (moneyStolen < 500)
        {
            return Node.Status.Failure;
        }

        return Node.Status.Success;
    }
    
    private Node.Status GoToBank()
    {
        Node.Status status = GoToLocation(diamond.transform.position);
        if (status == Node.Status.Success)
        {
            diamond.transform.parent = this.gameObject.transform;
        }
        return status;
    }

    private Node.Status GoToBackDoor()
    {
        return GoToDoor(backDoor);
    }
    
    private Node.Status GoToFrontDoor()
    {
        return GoToDoor(frontDoor);
    }
    
    private Node.Status GoToVan()
    {
        Node.Status status = GoToLocation(van.transform.position);
        if (status == Node.Status.Success)
        {
            moneyStolen += 300;
            diamond.SetActive(false);
        }
        return status;
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status status = GoToLocation(door.transform.position);
        if (status == Node.Status.Success)
        {
            if (!door.GetComponent<Lock>().IsLocked)
            {
                door.SetActive(false);
                return Node.Status.Success;
            }
            return Node.Status.Failure;
        }
        else
        {
            return status;
        }
    }
    
    Node.Status GoToLocation(Vector3 destination)
    {
        float distToTarget = Vector3.Distance(destination, this.transform.position);
        if (state==ActionState.Idle)
        {
            agent.SetDestination(destination);
            state = ActionState.Working;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2)
        {
            state = ActionState.Idle;
            return Node.Status.Failure;
        }
        else if (distToTarget < 2)
        {
            state = ActionState.Idle;
            return Node.Status.Success;
        }
        return Node.Status.Running;
    }
}  