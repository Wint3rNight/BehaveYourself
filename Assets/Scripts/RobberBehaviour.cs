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
    public GameObject cop;
    
    public GameObject[] art;
    
    GameObject _pickup;
    
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
        
        RandomSelector selectObjectToSteal = new RandomSelector("Select Object To Steal");
        for (int i = 0; i < art.Length; i++)
        {
            Leaf goToArt = new Leaf("Go To Art "+art[i].name, i,GoToArt);
            selectObjectToSteal.AddChild(goToArt);
        }
        
        _goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor,2);
        _goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor,1);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        PrioritySelector openDoor = new PrioritySelector("Open Door");
        
        Invertor invertMoney = new Invertor("Invert Money");
        invertMoney.AddChild(hasMoney);
        
        openDoor.AddChild(_goToFrontDoor);
        openDoor.AddChild(_goToBackDoor);
        
        steal.AddChild(invertMoney);
        steal.AddChild(openDoor);
        
        steal.AddChild(selectObjectToSteal);
        steal.AddChild(goToVan);
        
        Sequence runAway = new Sequence("Run Away");
        Leaf canSeeCop = new Leaf("Can See Cop?", CanSeeCop);
        Leaf fleeFromCop = new Leaf("Flee From Cop", FleeFromCop);
        
        runAway.AddChild(canSeeCop);
        runAway.AddChild(fleeFromCop);
        
        Tree.AddChild(runAway);
        
        Tree.PrintTree();
    }
    
    public Node.Status CanSeeCop()
    {
        return CanSee(cop.transform.position, "Cop", 10, 90);
    }
    
    public Node.Status FleeFromCop()
    {
        return Flee(cop.transform.position, 10);
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
            _pickup = diamond;
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
            _pickup = painting;
        }
        return status;
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
            art[i].transform.parent = this.gameObject.transform;
            _pickup = art[i];
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
            _pickup.SetActive(false);
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
}  