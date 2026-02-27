using System;
using System.Collections;
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
    
    //Node.Status _treeStatus = Node.Status.Running;
    private Leaf _goToBackDoor;
    private Leaf _goToFrontDoor;
    
    new void Start()
    {        
        base.Start();
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
        
        Sequence runAway = new Sequence("Run Away");
        Leaf canSeeCop = new Leaf("Can See Cop?", CanSeeCop);
        Leaf fleeFromCop = new Leaf("Flee From Cop", FleeFromCop);
        
        Invertor invertMoney = new Invertor("Invert Money");
        invertMoney.AddChild(hasMoney);
        
        openDoor.AddChild(_goToFrontDoor);
        openDoor.AddChild(_goToBackDoor);
        
        runAway.AddChild(canSeeCop);
        runAway.AddChild(fleeFromCop);
        
        Invertor cantSeeCop = new Invertor("Can't See Cop");
        cantSeeCop.AddChild(canSeeCop);
        
        Sequence s1 = new Sequence("s1");
        s1.AddChild(invertMoney);
        Sequence s2 = new Sequence("s2");
        s2.AddChild(cantSeeCop);
        s2.AddChild(openDoor);
        Sequence s3 = new Sequence("s3");
        s3.AddChild(cantSeeCop);
        s3.AddChild(selectObjectToSteal);
        Sequence s4 = new Sequence("s4");
        s4.AddChild(cantSeeCop);
        s4.AddChild(goToVan);
        
        /*steal.AddChild(s1);
        steal.AddChild(s2);
        steal.AddChild(s3);
        steal.AddChild(s4);*/
        BehaviourTree stealConditions = new BehaviourTree();
        Sequence conditions = new Sequence("Stealing Conditions");
        conditions.AddChild(cantSeeCop);
        conditions.AddChild(invertMoney);
        stealConditions.AddChild(conditions);
        DependencySequence steal = new DependencySequence("Steal",stealConditions,agent);
        //steal.AddChild(invertMoney);
        steal.AddChild(openDoor);
        steal.AddChild(selectObjectToSteal);
        steal.AddChild(goToVan);
        
        Selector stealWithFallback = new Selector("Steal With Fallback");
        stealWithFallback.AddChild(steal);
        stealWithFallback.AddChild(goToVan);
        
        Selector beThief = new Selector("Be Thief");
        beThief.AddChild(stealWithFallback);
        beThief.AddChild(runAway);
        
        Tree.AddChild(beThief);
        
        Tree.PrintTree();

        StartCoroutine(DecreaseMoney());
    }

        IEnumerator DecreaseMoney()
    {
        while (true)
        {
            moneyStolen = Mathf.Clamp(moneyStolen - 50, 0, 1000);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));
        }
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
}  