using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class BehaviourTreeAgent : MonoBehaviour
{
    public BehaviourTree Tree;
    [FormerlySerializedAs("_agent")] public NavMeshAgent agent;
    

    public enum ActionState
    {
        Idle,
        Working
    }
    
    [FormerlySerializedAs("_state")] public ActionState state = ActionState.Idle;
    
    [FormerlySerializedAs("_treeStatus")] public Node.Status treeStatus = Node.Status.Running;
    
    public void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        Tree = new BehaviourTree();
    }
    
    public Node.Status GoToLocation(Vector3 destination)
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

    public void Update()
    {
        if (treeStatus != Node.Status.Success)
        {
           treeStatus =  Tree.Process();
        }
    }
}  
