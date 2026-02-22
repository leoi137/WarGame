using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Unit))]
public class UnitMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Unit unit;
    Transform followTarget;
    Vector3 flockDestination;

    public bool UseFlocking;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        unit = GetComponent<Unit>();

        float baseSpeed = unit.moveSpeed;
        if (unit.typeDefinition != null)
            baseSpeed *= TerrainEffects.GetMovementMultiplier(transform.position, unit.typeDefinition.category);
        agent.speed = baseSpeed;
        agent.stoppingDistance = 0.5f;
        agent.angularSpeed = 360f;
        agent.acceleration = 10f;
    }

    void Update()
    {
        if (unit.isDead)
        {
            if (!UseFlocking && agent != null)
                agent.isStopped = true;
            return;
        }

        if (UseFlocking)
        {
            if (agent != null && agent.enabled)
                agent.enabled = false;
            UpdateFlocking();
            return;
        }

        if (agent != null && !agent.enabled)
            agent.enabled = true;
        if (unit.typeDefinition != null && agent != null && agent.isOnNavMesh)
        {
            float mult = TerrainEffects.GetMovementMultiplier(transform.position, unit.typeDefinition.category);
            agent.speed = unit.moveSpeed * mult;
        }

        if (followTarget != null)
        {
            agent.SetDestination(followTarget.position);
        }
    }

    void UpdateFlocking()
    {
        if (agent != null && agent.enabled)
            agent.enabled = false;

        Vector3 targetPos = followTarget != null ? followTarget.position : flockDestination;
        float speed = unit.moveSpeed;
        if (unit.typeDefinition != null)
            speed *= TerrainEffects.GetMovementMultiplier(transform.position, unit.typeDefinition.category);

        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0;
        float dist = toTarget.magnitude;
        Vector3 seek = dist > 0.1f ? toTarget.normalized * Mathf.Min(speed, dist / Time.deltaTime) : Vector3.zero;

        Vector3 separation = Vector3.zero;
        float separationRadius = 1.5f;
        float desiredSpacing = 1f;
        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        foreach (Unit other in allUnits)
        {
            if (other == unit || other.isDead || other.faction != unit.faction) continue;
            Vector3 toOther = other.transform.position - transform.position;
            toOther.y = 0;
            float d = toOther.magnitude;
            if (d < separationRadius && d > 0.01f)
            {
                float strength = 1f - (d / separationRadius);
                separation -= toOther.normalized * strength * speed * 0.5f;
            }
        }

        Vector3 velocity = seek + separation;
        if (velocity.sqrMagnitude > speed * speed)
            velocity = velocity.normalized * speed;
        transform.position += velocity * Time.deltaTime;
        if (velocity.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(velocity);
    }

    public void MoveTo(Vector3 position)
    {
        if (unit.isDead) return;
        followTarget = null;
        flockDestination = position;
        if (!UseFlocking && agent != null)
        {
            agent.isStopped = false;
            agent.stoppingDistance = 0.5f;
            agent.SetDestination(position);
        }
    }

    public void MoveToTarget(Transform target, float stoppingDistance)
    {
        if (unit.isDead) return;
        followTarget = target;
        flockDestination = target != null ? target.position : transform.position;
        if (!UseFlocking && agent != null)
        {
            agent.isStopped = false;
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(target.position);
        }
    }

    public void Stop()
    {
        followTarget = null;
        if (!UseFlocking && agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
    }

    public bool HasReachedDestination()
    {
        if (UseFlocking)
            return Vector3.Distance(transform.position, flockDestination) <= 1f;
        if (!agent.isOnNavMesh) return true;
        if (agent.pathPending) return false;
        return agent.remainingDistance <= agent.stoppingDistance + 0.1f;
    }

    public bool IsMoving()
    {
        if (UseFlocking) return false;
        if (!agent.isOnNavMesh) return false;
        return agent.velocity.sqrMagnitude > 0.1f;
    }
}
