using System;
using UnityEngine;

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
        public Vector2 mouseRect;
        public Vector2 screenVector2;
        
        private float _curMoveSpeed;

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
            var mouseAllowRect = new Rect(mouseRect.x, mouseRect.y, Screen.width - 2 * mouseRect.x,
                Screen.height - 2 * mouseRect.y);
            screenVector2 = new Vector2(Screen.width, Screen.height);
            var moveVector3 =
                new Vector3(
                    Input.mousePosition.x < mouseAllowRect.xMin ? -1 :
                    Input.mousePosition.x > mouseAllowRect.xMax ? 1 : 0,
                    Input.mousePosition.y < mouseAllowRect.yMin ? -1 :
                    Input.mousePosition.y > mouseAllowRect.yMax ? 1 : 0,
                    0);
//            moveVector3.Normalize();
//            Debug.Log(moveVector3);
//            Debug.Log(Input.mousePosition);
            _curMoveSpeed = Math.Abs(moveVector3.sqrMagnitude) > Mathf.Epsilon
                ? NextValue(_curMoveSpeed, moveSpeed)
                : NextValue(_curMoveSpeed, 0);
            transform.Translate(Time.deltaTime * _curMoveSpeed * moveVector3);
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