using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace CameraControl
{
    public enum CameraMode
    {
        Synchronization,
        Follow,
        MouseControl,
        OverLook
    }

    public class CameraControl : MonoBehaviour
    {
        public CameraMode cameraMode;
        public bool enableLinearInterpolation;
        [Range(0, 1)] public float linearInterpolation;

        public Transform player;
        public float moveSpeed;

        private Vector3 _prePosition;
        private float _preAngle;
        private Vector3 _preNormal;

        private void Start()
        {
            var thisTransform = transform;
            _prePosition = player.position - thisTransform.position;
            RotationAngle(player, thisTransform, out _preAngle, out _preNormal);
        }

        private void FixedUpdate()
        {
            switch (cameraMode)
            {
                case CameraMode.Synchronization:
                    Sync();
                    break;
                case CameraMode.Follow:
                    Fol();
                    break;
                case CameraMode.MouseControl:
                    Mou();
                    break;
                case CameraMode.OverLook:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Sync()
        {
            Transform thisTransform;
            (thisTransform = transform).position = NextPosition(transform.position, player.position - _prePosition);
            RotationAngle(thisTransform, player, out var angle, out var normal);
            transform.Rotate(normal * angle);
            transform.Rotate(_preNormal * _preAngle);
        }

        private void Fol()
        {
            transform.position = NextPosition(transform.position, player.position - _prePosition);
        }

        private void Mou()
        {
            
        }
        
        private float NextValue(float curValue, float targetValue)
        {
            return !enableLinearInterpolation ? targetValue : Mathf.Lerp(curValue, targetValue, linearInterpolation);
        }

        private Vector3 NextPosition(Vector3 curPosition, Vector3 targetPosition)
        {
            return !enableLinearInterpolation
                ? targetPosition
                : Vector3.Lerp(curPosition, targetPosition, linearInterpolation);
        }

        private void RotationAngle(Transform curTransform, Transform targetTransform, out float angle,
            out Vector3 normal)
        {
            var curTransformForward = curTransform.forward;
            var targetTransformForward = targetTransform.forward;
            angle = Vector3.Angle(curTransformForward, targetTransformForward);
            normal = Vector3.Cross(curTransformForward, targetTransformForward);
            angle *= Mathf.Sign(Vector3.Dot(normal, curTransform.up));
            if (enableLinearInterpolation)
            {
                angle = NextValue(0, angle);
            }
        }
    }
}