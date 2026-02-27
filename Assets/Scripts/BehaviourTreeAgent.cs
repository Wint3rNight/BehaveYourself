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
    public NavMeshAgent agent;
    

    public enum ActionState
    {
        Idle,
        Working
    }
    
    public ActionState state = ActionState.Idle;
    public Node.Status treeStatus = Node.Status.Running;

    public WaitForSeconds _waitForSeconds;
    Vector3 _rememberedLocation;
    
    public virtual void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        Tree = new BehaviourTree();
        _waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1f));
        StartCoroutine("Behave");
    }

    public Node.Status CanSee(Vector3 target, string tag, float viewDistance, float fieldOfView)
    {
        Vector3 directionToTarget = target - this.transform.position;
        float angleToTarget = Vector3.Angle(directionToTarget, this.transform.forward);

        if (angleToTarget < fieldOfView && directionToTarget.magnitude <= viewDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, directionToTarget, out hit))
            {
                if (hit.collider.gameObject.CompareTag(tag))
                {
                    return Node.Status.Success;
                }
            }
        }
        return Node.Status.Failure;
    }

    public Node.Status Flee(Vector3 location, float fleeDistance)
    {
        if (state == ActionState.Idle)
        {
            _rememberedLocation = this.transform.position+(transform.position-location).normalized*fleeDistance;
        }

        return GoToLocation(_rememberedLocation);
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

    IEnumerator Behave()
    {
        while (true)
        {
            treeStatus = Tree.Process();
            yield return _waitForSeconds;
        }
    }
}  
