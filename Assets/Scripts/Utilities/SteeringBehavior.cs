using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Nightmare
{
    public class SteeringBehavior : MonoBehaviour
    {
        protected float maxSpeed = 1f;
        protected float maxForce = 1f;
        protected float mass = 1f;
        protected float slowingDistance = 1f;
        protected float minDistanceFlee = 1f;

        public Vector3 Seek(Vector3 targetPosition, Enemy self)
        {
            Vector3 desiredVelocity = (targetPosition - self.transform.position).normalized * this.maxSpeed;
            Vector3 steeringforce = Vector3.ClampMagnitude((desiredVelocity - self.velocity), this.maxForce);
            return steeringforce;
        }

        public Vector3 SeekAndArrive(Vector3 targetPosition, Enemy self)
        {
            Vector3 desiredVelocity = (targetPosition - self.transform.position);
            float targetDistance = desiredVelocity.magnitude;
            desiredVelocity = desiredVelocity.normalized * this.maxSpeed;
            desiredVelocity = ApproachTarget(this.slowingDistance, targetDistance, desiredVelocity);
            Vector3 steeringForce = Vector3.ClampMagnitude((desiredVelocity - self.velocity), this.maxForce);

            return steeringForce;
        }

        public Vector3 SeekWithMass(Vector3 targetPosition, Enemy self)
        {
            Vector3 steeringForce = Seek(targetPosition, self) / this.mass;
            self.velocity = Vector3.ClampMagnitude((self.velocity + steeringForce), this.maxForce);
            self.transform.position += self.velocity;

            return Vector3.zero;
        }

        public Vector3 ApproachTarget(float slowingDistance, float distanceToTarge, Vector3 velocity)
        {
            if (distanceToTarge < slowingDistance)
            {
                velocity *= (distanceToTarge / slowingDistance);
            }
            return velocity;
        }

        public Vector3 Flee(Vector3 targetPosition, Enemy self)
        {
            Vector3 distanceToTarget = targetPosition - self.transform.position;
            if (distanceToTarget.magnitude > this.minDistanceFlee)
            {
                self.velocity = Vector3.zero;
                return Vector3.zero;
            }

            return -Seek(-targetPosition, self);
        }

        public Vector3 Pursue(Enemy self)
        {
            Player target = self.target.GetComponent<Player>();
            Vector3 distance = target.transform.position - self.transform.position;
            Vector3 anticipation = distance / this.maxSpeed;
            Vector3 futureTargetPosition = target.transform.position + Vector3.Scale(target.velocity, anticipation);
            return Seek(futureTargetPosition, self);
        }

        public Vector3 Evade(Enemy self) 
        {
            Player target = self.target.GetComponent<Player>();
            Vector3 distance = target.transform.position - self.transform.position;
            Vector3 updatesAhead = distance / this.maxSpeed;
            Vector3 futureTargetPosition = target.transform.position + Vector3.Scale(self.target.GetComponent<Player>().velocity, updatesAhead);
            return Flee(futureTargetPosition, self);
        }

        public Vector3 Wander(float circleDistance, float circleRadius, Enemy self)
        {
            Vector3 circleCenter = self.velocity.normalized * circleDistance;
            Vector3 displacement = new Vector3(circleRadius, 0, 0);
            Vector3 wanderVector = Quaternion.Euler(0, Random.Range(-360f, 360f), 0) * displacement;
            Vector3 steeringForce = circleCenter + wanderVector;
            return Seek(self.transform.position + steeringForce, self);
        }
    }
}
