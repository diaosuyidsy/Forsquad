using UnityEngine;
using Pathfinding;

namespace BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject
{
    // Abstract class for any task that uses an IAstarAI cmponent
    public abstract class IAstarAIMovement : Movement
    {
        [Tooltip("The speed of the agent")]
        public SharedFloat speed = 10;
        [Tooltip("The agent has arrived when the destination is less than the specified amount")]
        public SharedFloat arriveDistance = 0.2f;
        [Tooltip("Should the NavMeshAgent be stopped when the task ends?")]
        public SharedBool stopOnTaskEnd = true;

        protected IAstarAI agent;

        public override void OnAwake()
        {
            agent = gameObject.GetComponent<IAstarAI>();
        }

        public override void OnStart()
        {
            agent.maxSpeed = speed.Value;
        }

        protected override bool SetDestination(Vector3 target)
        {
            agent.destination = target;
            agent.canMove = true;
            agent.isStopped = false;
            return true;
        }

        protected override Vector3 Velocity()
        {
            return agent.velocity;
        }

        protected override void UpdateRotation(bool update)
        {
            // Intentionally left blank
        }

        protected override bool HasPath()
        {
            return agent.hasPath && Vector3.Distance(transform.position, agent.destination) > arriveDistance.Value;
        }

        protected override void Stop()
        {
            agent.destination = transform.position;
            agent.canMove = false;
        }

        protected override bool HasArrived()
        {
            // The path hasn't been computed yet if the path is pending.
            float remainingDistance;
            if (agent.pathPending) {
                remainingDistance = float.PositiveInfinity;
            } else {
                remainingDistance = agent.remainingDistance;
            }

            return remainingDistance <= arriveDistance.Value;
        }

        public override void OnEnd()
        {
            if (stopOnTaskEnd.Value) {
                Stop();
            }
        }

        public override void OnReset()
        {
            speed = 10;
            arriveDistance = 1;
            stopOnTaskEnd = true;
        }
    }
}
