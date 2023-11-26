using System.Collections.Generic;
using UnityEngine;

namespace PPP
{
    public class OnClickSpawner : DynamicObjectSpawner
    {

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                SpawnDynamicObjects();
            }
        }
        public void SpawnDynamicObjects()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPoint = hit.point;
                Vector3 hitNormal = hit.normal;
                Transform hitTransform = hit.transform;
                List<Vector3> initialDirections = Geometry_Utility.GeneratePerpendicularDirections(hitNormal, Vector3.forward, _numberOfPaths, _divergence, _angleRange);
                _pathManager = new(hitPoint, hitNormal, hitTransform, initialDirections, preferedStepDistance, preferedStepDistance, _iterationAngleDivergence, _layerMask);
                DefineGizmoColors();
                InstantiateObjects();
            }
        }
    }
}
