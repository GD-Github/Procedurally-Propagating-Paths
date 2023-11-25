using System.Collections.Generic;
using UnityEngine;

namespace PPP
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]

    public class OnCollisionSpawnStaticObjects : StaticObjectSpawner
    {
        // A simple class to generate static objects after a collision happens. 

        private bool _hasCollided = false;
        private void OnCollisionEnter(Collision collision)
        {
            if (_hasCollided) return;
            _hasCollided = true;

            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 hitNormal = collision.contacts[0].normal;
            Transform hitTransform = collision.transform;

            List<Vector3> initialDirections = Geometry_Utility.GeneratePerpendicularDirections(hitNormal, GetComponent<Rigidbody>().velocity, _numberOfPaths, _divergence, _angleRange);
            _pathManager = new(hitPoint, hitNormal, hitTransform, initialDirections, MinPreferedStepDistance, MaxPreferedStepDistance, _iterationAngleDivergence, _layerMask);
            DefineGizmoColors();
            InstantiateObjects();
        }

    }
}
