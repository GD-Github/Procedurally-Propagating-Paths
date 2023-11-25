using UnityEngine;

namespace PPP
{
    public class AnchorPoint
    {
        private const float DEFAULT_NORMAL_OFFSET = 0.03f;
        private Vector3 _position;
        private Transform _parent;
        private Vector3 _normal;
        private Vector3 _direction;

        public Transform Parent
        {
            get { return _parent; }
        }

        private Vector3 _parentInitialScale = Vector3.one;
        private Vector3 _parentInitialPos = Vector3.zero;
        private Quaternion _parentInitialRot = Quaternion.identity;

        public AnchorPoint(Vector3 position, Vector3 direction, Vector3 normal, Transform parent)
        {
            _position = position;
            _direction = direction;
            _normal = normal;
            _parent = parent;
            if (parent)
            {
                _parentInitialPos = parent.position;
                _parentInitialRot = parent.rotation;
                _parentInitialScale = parent.lossyScale;
            }

        }

        public Vector3 GetPosition()
        {
            if (!_parent)
            {
                return _position + DEFAULT_NORMAL_OFFSET * GetNormal();
            }
            Vector3 unscaledPos = RemoveTransformOffset(_position);
            return RemoveScaleOffset(unscaledPos) + DEFAULT_NORMAL_OFFSET * GetNormal();
        }

        public Vector3 GetNormal()
        {
            if (!_parent)
            {
                return _normal;
            }
            return RemoveTransformOffset(_position + _normal) - RemoveTransformOffset(_position);
        }

        public Vector3 GetDirection()
        {
            if (!_parent)
            {
                return _direction;
            }
            return RemoveTransformOffset(_position + _direction) - RemoveTransformOffset(_position);
        }

        private Vector3 RemoveTransformOffset(Vector3 point)
        {
            //Updates the position of the anchor point regardless of the parent's position and rotation
            Quaternion rotationOffset = _parent.rotation * Quaternion.Inverse(_parentInitialRot);
            Vector3 positionOffset = _parent.position - _parentInitialPos;
            return Geometry_Utility.RotateAroundPivot(point + positionOffset, _parent.position, rotationOffset);
        }

        private Vector3 RemoveScaleOffset(Vector3 point)
        {
            //Updates the position of the anchor point regardless of the parent's scales modification
            Vector3 posOffset = point - _parent.position;
            Vector3 scaleFactors = new(_parent.lossyScale.x / _parentInitialScale.x, _parent.lossyScale.y / _parentInitialScale.y, _parent.lossyScale.z / _parentInitialScale.z);
            _parentInitialScale = _parent.lossyScale;
            return new(_parent.position.x + scaleFactors.x * posOffset.x, _parent.position.y + scaleFactors.y * posOffset.y, _parent.position.z + scaleFactors.z * posOffset.z);
        }
    }
}