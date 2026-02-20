using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


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

    private WaitForSeconds _waitForSeconds;
    
    public void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        Tree = new BehaviourTree();
        _waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1f));
        StartCoroutine("Behave");
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

    IEnumerator Behave()
    {
        while (true)
        {
            treeStatus = Tree.Process();
            yield return _waitForSeconds;
        }
    }
}  
