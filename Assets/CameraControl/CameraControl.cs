using System;
using UnityEngine;
using UnityEngine.Serialization;

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
        private Camera _camera;
        private Material _material;
        private Shader _curShader;
        private static readonly int LuminosityAmount = Shader.PropertyToID("_LuminosityAmount");

        public CameraMode cameraMode;
        public bool enableLinearInterpolation;
        [Range(0, 1)] public float linearInterpolation;
        [Range(0, 1)] public float grayScaleAmount = 1f;

        public Transform player;

        private Vector3 _prePosition;
        private float _preAngle;
        private Vector3 _preNormal;

        public float moveSpeed;
        public Vector2 mouseRect;
        public float zoomInSpeed;
        public bool useClock;
        public Vector3 clockPosition2, clockPosition1;
        public int freeDimension;
        public bool enableFixedDistance;
        public float cameraDistance;
        public float cameraIgnoreDistance;
        public float cameraMaxDistance;
        public LayerMask cameraOnMask;

        private float _curMoveSpeed;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            var thisTransform = transform;
            _prePosition = player.position - thisTransform.position;
            RotationAngle(player, thisTransform, out _preAngle, out _preNormal);

            _curShader = Shader.Find("CameraGrey/CameraGreyShader");
            _material = new Material(_curShader) {hideFlags = HideFlags.HideAndDontSave};
        }

        private void LateUpdate()
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
            if (!player) return;
            Transform thisTransform;
            (thisTransform = transform).position = NextPosition(transform.position, player.position - _prePosition);
            RotationAngle(thisTransform, player, out var angle, out var normal);
            transform.Rotate(normal * angle);
            transform.Rotate(_preNormal * _preAngle);
        }

        private void Fol()
        {
            if (!player) return;
            transform.position = NextPosition(transform.position, player.position - _prePosition);
        }

        private void Mou()
        {
            var mouseAllowRect = new Rect(mouseRect.x, mouseRect.y, Screen.width - 2 * mouseRect.x,
                Screen.height - 2 * mouseRect.y);
            var moveVector3 =
                new Vector3(
                    Input.mousePosition.x < mouseAllowRect.xMin ? -1 :
                    Input.mousePosition.x > mouseAllowRect.xMax ? 1 : 0,
                    0,
                    Input.mousePosition.y < mouseAllowRect.yMin ? -1 :
                    Input.mousePosition.y > mouseAllowRect.yMax ? 1 : 0);
            moveVector3.Normalize();
            _curMoveSpeed = Math.Abs(moveVector3.sqrMagnitude) > Mathf.Epsilon
                ? NextValue(_curMoveSpeed, moveSpeed)
                : NextValue(_curMoveSpeed, 0);
            transform.Translate(Time.deltaTime * _curMoveSpeed * moveVector3, Space.World);
            if (enableFixedDistance)
            {
                var selfTransform = transform;
                var ray = new Ray(selfTransform.position, selfTransform.forward);
                if (Physics.Raycast(ray, out var hitInfo, cameraMaxDistance, cameraOnMask))
                {
                    if (Mathf.Abs(cameraDistance - hitInfo.distance) > cameraIgnoreDistance)
                    {
                        selfTransform.Translate(NextPosition(Vector3.zero,
                            (hitInfo.distance - cameraDistance) * selfTransform.forward), Space.World);
                    }
                }
            }

            if (useClock)
            {
                PositionClock();
            }

            _camera.orthographicSize -= zoomInSpeed * Input.mouseScrollDelta.y;
            _camera.fieldOfView -= zoomInSpeed * Input.mouseScrollDelta.y;
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

        private void PositionClock()
        {
            var position = transform.position;
            var newPosition = new Vector3
            {
                x = freeDimension == 1
                    ? position.x
                    : Mathf.Clamp(position.x, Mathf.Min(clockPosition1.x, clockPosition2.x),
                        Mathf.Max(clockPosition1.x, clockPosition2.x)),
                y = freeDimension == 2
                    ? position.y
                    : Mathf.Clamp(position.y, Mathf.Min(clockPosition1.y, clockPosition2.y),
                        Mathf.Max(clockPosition1.y, clockPosition2.y)),
                z = freeDimension == 3
                    ? position.z
                    : Mathf.Clamp(position.z, Mathf.Min(clockPosition1.z, clockPosition2.z),
                        Mathf.Max(clockPosition1.z, clockPosition2.z))
            };
            transform.position = newPosition;
        }

        private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
            if (_curShader != null)
            {
                _material.SetFloat(LuminosityAmount, grayScaleAmount);

                Graphics.Blit(sourceTexture, destTexture, _material);
            }
            else
            {
                Graphics.Blit(sourceTexture, destTexture);
            }
        }
    }
}