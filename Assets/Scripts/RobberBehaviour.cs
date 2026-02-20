using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class RobberBehaviour : BehaviourTreeAgent
{
    public GameObject diamond;
    public GameObject painting;
    public GameObject van;
    public GameObject backDoor;
    public GameObject frontDoor;
    GameObject pickup;
    
    [Range(0,1000)]
    public int moneyStolen = 800;
    
    Node.Status _treeStatus = Node.Status.Running;
    
    new void Start()
    {        
        base.Start();
        Sequence steal = new Sequence("Steal");
        Leaf goToBank = new Leaf("Go To Bank", GoToBank, 1);
        Leaf goToPainting = new Leaf("Go To Bank", GoToPainting, 2);
        Leaf hasMoney = new Leaf("Has Money", HasMoney);
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Selector openDoor = new Selector("Open Door");
        PrioritySelector selectObjectToSteal = new PrioritySelector("Select Object To Steal");

        
        Invertor invertMoney = new Invertor("Invert Money");
        invertMoney.AddChild(hasMoney);
        
        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);
        
        steal.AddChild(invertMoney);
        steal.AddChild(openDoor);
        
        selectObjectToSteal.AddChild(goToBank);
        selectObjectToSteal.AddChild(goToPainting);
        
        steal.AddChild(selectObjectToSteal);
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
        if (!diamond.activeSelf)
        {
            return Node.Status.Failure;
        }
        Node.Status status = GoToLocation(diamond.transform.position);
        if (status == Node.Status.Success)
        {
            diamond.transform.parent = this.gameObject.transform;
            pickup = diamond;
        }
        return status;
    }
    
    private Node.Status GoToPainting()
    {
        if (!painting.activeSelf)
        {
            return Node.Status.Failure;
        }
        Node.Status status = GoToLocation(painting.transform.position);
        if (status == Node.Status.Success)
        {
            painting.transform.parent = this.gameObject.transform;
            pickup = painting;
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
            pickup.SetActive(false);
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
                door.GetComponent<NavMeshObstacle>().enabled = false;
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