using System.Collections.Generic;
using UnityEngine;

namespace PPP
{
    public class ObjectPath
    {
        public List<AnchorPoint> AnchorPoints { get; private set; }

        private PathManager _pathManager;
        private float _minStepDistance;
        private float _maxStepDistance;
        private float _angleDeviation;
        private List<float> _cumulatedStepDistances;
        private float _currentDistanceBetweenAnchors;
        private int _currentIndex = 0;
        public ObjectPath(Vector3 initialPoint, Vector3 initialDirection, Vector3 initialNormal, Transform initialParent, float minStepDistance, float maxStepDistance, float angleDeviation, PathManager pathManager)
        {
            AnchorPoint firstPoint = new(initialPoint, initialDirection, initialNormal, initialParent);
            AnchorPoints = new() { firstPoint };
            _minStepDistance = minStepDistance;
            _maxStepDistance = maxStepDistance;
            _pathManager = pathManager;
            _angleDeviation = angleDeviation;
            _cumulatedStepDistances = new();
        }
        public AnchorPoint GetAnchorPoint(int index)
        {
            while (index >= AnchorPoints.Count)
            {
                AddNewAnchorPoint();
            }
            return AnchorPoints[index];
        }

        private float GetLerpValue(float f)
        {
            // This method is a way to get dynamic objects that follow the path to move at a constant speed. 
            // The idea is to normalize the lerp values according to the distance between consecutive anchor points.
            if (f >= _cumulatedStepDistances[_currentIndex])
            {
                _currentIndex++;
                AddNewAnchorPoint();
                _currentDistanceBetweenAnchors = _cumulatedStepDistances[_currentIndex] - _cumulatedStepDistances[_currentIndex - 1];
                if (_currentDistanceBetweenAnchors == 0) return 0; //This shouldn't happen: It means that two anchor points are on the same spot, so most likely the anchor point wasn't found.
            }
            float distanceSinceLastAnchor = _currentIndex == 0 ? f : f - _cumulatedStepDistances[_currentIndex - 1];
            return distanceSinceLastAnchor / _currentDistanceBetweenAnchors;

        }
        public Vector3[] GetObjectPositionAndForward(float f)
        {
            // Returns the position and the forward vector of an object that followed this path for a distance of "f"

            Vector3[] positionAndForward = new Vector3[2];
            while (AnchorPoints.Count < _currentIndex + 3) //We need 2 extra points to interpolate the position and the forward direction
            {
                AddNewAnchorPoint();
            }
            float lerpValue = GetLerpValue(f);

            positionAndForward[0] = Vector3.Lerp(AnchorPoints[_currentIndex].GetPosition(), AnchorPoints[_currentIndex + 1].GetPosition(), lerpValue);

            Vector3 currentPointDirection = AnchorPoints[_currentIndex + 1].GetPosition() - AnchorPoints[_currentIndex].GetPosition();
            Vector3 nextPointDirection = AnchorPoints[_currentIndex + 2].GetPosition() - AnchorPoints[_currentIndex + 1].GetPosition();
            positionAndForward[1] = Vector3.Lerp(currentPointDirection, nextPointDirection, lerpValue);

            return positionAndForward;
        }
        private void AddNewAnchorPoint()
        {
            //Adds a new anchor point to the list and updates the cumulated distance
            AnchorPoint anchor = AnchorPoints[^1];
            float stepDistance = Random.Range(_minStepDistance, _maxStepDistance);
            Vector3 newPosition = _pathManager.ReturnNextPoint(anchor.GetPosition(), anchor.GetNormal(), anchor.GetDirection(), _angleDeviation, stepDistance, out Vector3 newNormal, out Vector3 newDirection, out Transform anchorParent);
            if (_cumulatedStepDistances.Count == 0)
            {
                _cumulatedStepDistances.Add(Vector3.Distance(anchor.GetPosition(), newPosition));
                _currentDistanceBetweenAnchors = _cumulatedStepDistances[0];
            }
            else
            {
                _cumulatedStepDistances.Add(Vector3.Distance(anchor.GetPosition(), newPosition) + _cumulatedStepDistances[^1]);
            }
            AnchorPoints.Add(new AnchorPoint(newPosition, newDirection, newNormal, anchorParent));
        }

    }
}