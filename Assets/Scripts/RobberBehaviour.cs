using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree _tree;
    [FormerlySerializedAs("Diamond")] public GameObject diamond;
    public GameObject van;
    public GameObject backDoor;
    NavMeshAgent _agent;
    

    public enum ActionState
    {
        Idle,
        Working
    }

    private ActionState _state = ActionState.Idle;
    
    Node.Status _treeStatus = Node.Status.Running;
    
    public void Start()
    {
        _agent = this.GetComponent<NavMeshAgent>();
        
        _tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal");
        Leaf goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor);
        Leaf goToBank = new Leaf("Go To Bank", GoToBank);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        
        steal.AddChild(goToBackDoor);
        steal.AddChild(goToBank);
        steal.AddChild(goToBackDoor);
        steal.AddChild(goToVan);
        _tree.AddChild(steal);
        
        _tree.PrintTree();
    }

    private Node.Status GoToBackDoor()
    {
        return GoToLocation(backDoor.transform.position);
    }
    
    private Node.Status GoToBank()
    {
        return GoToLocation(diamond.transform.position);
    }
    
    private Node.Status GoToVan()
    {
        return GoToLocation(van.transform.position);
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
        if (_treeStatus == Node.Status.Running)
        {
           _treeStatus =  _tree.Process();
        }
    }
}  
