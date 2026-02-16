using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Unit))]
public class UnitMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Unit unit;
    Transform followTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        unit = GetComponent<Unit>();

        agent.speed = unit.moveSpeed;
        agent.stoppingDistance = 0.5f;
        agent.angularSpeed = 360f;
        agent.acceleration = 10f;
    }

    void Update()
    {
        if (unit.isDead)
        {
            agent.isStopped = true;
            return;
        }

        if (followTarget != null)
        {
            agent.SetDestination(followTarget.position);
        }
    }

    public void MoveTo(Vector3 position)
    {
        if (unit.isDead) return;
        followTarget = null;
        agent.isStopped = false;
        agent.stoppingDistance = 0.5f;
        agent.SetDestination(position);
    }

    public void MoveToTarget(Transform target, float stoppingDistance)
    {
        if (unit.isDead) return;
        followTarget = target;
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(target.position);
    }

    public void Stop()
    {
        followTarget = null;
        if (agent.isOnNavMesh)
            agent.isStopped = true;
    }

    public bool HasReachedDestination()
    {
        if (!agent.isOnNavMesh) return true;
        if (agent.pathPending) return false;
        return agent.remainingDistance <= agent.stoppingDistance + 0.1f;
    }

    public bool IsMoving()
    {
        if (!agent.isOnNavMesh) return false;
        return agent.velocity.sqrMagnitude > 0.1f;
    }
}
