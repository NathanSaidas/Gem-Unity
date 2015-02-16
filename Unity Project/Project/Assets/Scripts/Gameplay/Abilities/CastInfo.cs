using UnityEngine;
using System.Collections;

namespace Gem
{
    /// <summary>
    /// Represents the data of a cast.
    /// </summary>
    public class CastInfo
    {
        public Vector3 targetPosition { get; set; }
        public Actor target { get; set; }
        public float radius { get; set; }


        public bool useTargetPosition { get; set; }
        public bool useTarget { get; set; }
        public bool useRadius { get; set; }

        public void Empty()
        {
            targetPosition = Vector3.zero;
            target = null;
            radius = 0.0f;

            useTargetPosition = false;
            useTarget = false;
            useRadius = false;
        }

        public CastInfo Copy()
        {
            return Copy(this);
        }

        public static CastInfo Copy(CastInfo aInfo)
        {
            CastInfo newInfo = new CastInfo();
            newInfo.targetPosition = aInfo.targetPosition;
            newInfo.target = aInfo.target;
            newInfo.radius = aInfo.radius;

            newInfo.useTargetPosition = aInfo.useTargetPosition;
            newInfo.useTarget = aInfo.useTarget;
            newInfo.useRadius = aInfo.useRadius;
            return newInfo;
        }

        public void SetTarget(Vector3 aPosition)
        {
            targetPosition = aPosition;
            target = null;
            radius = 0.0f;

            useTargetPosition = true;
            useTarget = false;
            useRadius = false;
        }

        public void SetTarget(Actor aTarget)
        {
            targetPosition = Vector3.zero;
            target = aTarget;
            radius = 0.0f;

            useTargetPosition = false;
            useTarget = true;
            useRadius = false;
        }

        public void SetTarget(Vector3 aPosition, float aRadius)
        {
            targetPosition = aPosition;
            target = null;
            radius = aRadius;

            useTargetPosition = true;
            useTarget = false;
            useRadius = true;
        }

        public void SetTarget(Actor aTarget, float aRadius)
        {
            targetPosition = Vector3.zero;
            target = aTarget;
            radius = aRadius;

            useTargetPosition = false;
            useTarget = true;
            useRadius = true;
        }

        public Vector3 GetTargetPosition()
        {
            if(useTargetPosition == true)
            {
                return targetPosition;
            }
            return target.transform.position;
        }
    }
}
