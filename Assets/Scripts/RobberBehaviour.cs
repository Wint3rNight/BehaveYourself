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

    public GameObject[] art;
    
    [Range(0,1000)]
    public int moneyStolen = 800;
    
    Node.Status _treeStatus = Node.Status.Running;
    private Leaf _goToBackDoor;
    private Leaf _goToFrontDoor;
    
    new void Start()
    {        
        base.Start();
        Sequence steal = new Sequence("Steal");
        Leaf goToBank = new Leaf("Go To Bank", GoToBank, 1);
        Leaf goToPainting = new Leaf("Go To Bank", GoToPainting, 2);
        Leaf hasMoney = new Leaf("Has Money", HasMoney);
        
        Leaf goToArt1 = new Leaf("Go To Art 1", GoToArt1);
        Leaf goToArt2 = new Leaf("Go To Art 2", GoToArt2);
        Leaf goToArt3 = new Leaf("Go To Art 3", GoToArt3);
        
        _goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor,2);
        _goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor,1);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        
        PrioritySelector openDoor = new PrioritySelector("Open Door");
        RandomSelector selectObjectToSteal = new RandomSelector("Select Object To Steal");

        
        Invertor invertMoney = new Invertor("Invert Money");
        invertMoney.AddChild(hasMoney);
        
        openDoor.AddChild(_goToFrontDoor);
        openDoor.AddChild(_goToBackDoor);
        
        steal.AddChild(invertMoney);
        steal.AddChild(openDoor);
        
        selectObjectToSteal.AddChild(goToArt1);
        selectObjectToSteal.AddChild(goToArt2);
        selectObjectToSteal.AddChild(goToArt3);
        
        steal.AddChild(selectObjectToSteal);
        //steal.AddChild(goToBackDoor);
        steal.AddChild(goToVan);
        Tree.AddChild(steal);
        
        Tree.PrintTree();
    }

    private Node.Status GoToArt1()
    {
        if (!art[0].activeSelf)
        {
            return Node.Status.Failure;
        }
        Node.Status status = GoToLocation(art[0].transform.position);
        if (status == Node.Status.Success)
        {
            art[0].transform.parent = this.gameObject.transform;
            pickup = art[0];
        }
        return status;
    }

    private Node.Status GoToArt2()
    {
        if (!art[1].activeSelf)
        {
            return Node.Status.Failure;
        }
        Node.Status status = GoToLocation(art[1].transform.position);
        if (status == Node.Status.Success)
        {
            art[1].transform.parent = this.gameObject.transform;
            pickup = art[1];
        }
        return status;  
    }

    private Node.Status GoToArt3()
    {
        if (!art[2].activeSelf)
        {
            return Node.Status.Failure;
        }
        Node.Status status = GoToLocation(art[2].transform.position);
        if (status == Node.Status.Success)
        {
            art[2].transform.parent = this.gameObject.transform;
            pickup = art[2];
        }
        return status;   
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
        Node.Status status = GoToDoor(backDoor);
        if(status==Node.Status.Failure)
        {
            _goToBackDoor.SortOrder=10;
        }
        else
        {
            _goToBackDoor.SortOrder=1;
        }
        return status;
    }
    
    private Node.Status GoToFrontDoor()
    {
        Node.Status status = GoToDoor(frontDoor);
        if(status==Node.Status.Failure)
        {
            _goToFrontDoor.SortOrder=10;
        }
        else
        {
            _goToFrontDoor.SortOrder=1;
        }
        return status;
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