using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree _tree;
    [FormerlySerializedAs("Diamond")] public GameObject diamond;
    public GameObject van;
    public GameObject backDoor;
    public GameObject frontDoor;
    NavMeshAgent _agent;
    

    public enum ActionState
    {
        Idle,
        Working
    }

    private ActionState _state = ActionState.Idle;
    
    [FormerlySerializedAs("MoneyStolen")] [Range(0,1000)]
    public int moneyStolen = 800;
    
    Node.Status _treeStatus = Node.Status.Running;
    
    public void Start()
    {
        _agent = this.GetComponent<NavMeshAgent>();
        
        _tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal");
        Leaf goToBank = new Leaf("Go To Bank", GoToBank);
        Leaf hasMoney = new Leaf("Has Money", HasMoney);
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor);
        Leaf goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        Selector openDoor = new Selector("Open Door");
        
        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);
        
        steal.AddChild(hasMoney);
        steal.AddChild(openDoor);
        steal.AddChild(goToBank);
        //steal.AddChild(goToBackDoor);
        steal.AddChild(goToVan);
        _tree.AddChild(steal);
        
        _tree.PrintTree();
    }
    
    public Node.Status HasMoney()
    {
        if (moneyStolen >= 500)
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
        if (_state==ActionState.Idle)
        {
            _agent.SetDestination(destination);
            _state = ActionState.Working;
        }
        else if (Vector3.Distance(_agent.pathEndPosition, destination) >= 2)
        {
            _state = ActionState.Idle;
            return Node.Status.Failure;
        }
        else if (distToTarget < 2)
        {
            _state = ActionState.Idle;
            return Node.Status.Success;
        }
        return Node.Status.Running;
    }

    public void Update()
    {
        if (_treeStatus != Node.Status.Success)
        {
           _treeStatus =  _tree.Process();
        }
    }
}  
