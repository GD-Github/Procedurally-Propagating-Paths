using System.Collections.Generic;
using UnityEngine;

namespace PPP
{
    public static class Geometry_Utility
    {
        public static List<Vector3> GeneratePerpendicularDirections(Vector3 direction, Vector3 wantedDirection, uint numberOfPerpendiculars, float divergence = 0, float angleRange = 360)
        {
            //Returns a list of "numberOfPerpendiculars" Vector3 all perpendicular to "direction", with the first one being the projection of "wantedDirection" on the normal plan.
            List<Vector3> result = new();
            if (numberOfPerpendiculars == 0) return result;

            angleRange = Mathf.Clamp(angleRange, 1, 360);
            float alpha0 = angleRange / numberOfPerpendiculars;
            result.Add(PerpendicularNormalized(direction, wantedDirection));
            for (int k = 1; k < numberOfPerpendiculars; k++)
            {
                Vector3 tmp = RotateAround(result[0], direction, alpha0 * k);
                result.Add(tmp);
            }
            divergence = Mathf.Clamp01(divergence);
            if (divergence == 0) return result;
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = RotateAround(result[i], direction, divergence * alpha0 * Random.Range(-1f / 2, 1f / 2));
            }
            return result;
        }


        public static Vector3 RotateAround(Vector3 position, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * (position);
        }

        public static Vector3 RotateDirection(Vector3 direction, Vector3 normal, float angle)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, normal);
            return rotation * direction;
        }
        public static Vector3 Perpendicular(Vector3 value)
        {
            if (value.x == 0)
            {
                return Vector3.right;
            }
            return new Vector3(-value.y / value.x, 1, 0);
        }

        public static Vector3 PerpendicularNormalized(Vector3 value, Vector3 wantedDirection)
        {
            Vector3 potentialOutput = Vector3.ProjectOnPlane(wantedDirection, value);
            if (potentialOutput != Vector3.zero) return potentialOutput.normalized;
            return Perpendicular(value).normalized;
        }

        public static Vector3 DirectionFromHitPoint(Vector3 originNormal, Vector3 hitNormal, Vector3 initialDirection)
        {
            Vector3 vector = Quaternion.FromToRotation(originNormal, hitNormal) * initialDirection;
            return Vector3.ProjectOnPlane(vector, hitNormal).normalized;
        }

        public static Vector3 RotateAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            Vector3 originalDirection = point - pivot;
            Vector3 newDirection = rotation * originalDirection;
            return pivot + newDirection;
        }
    }
}
