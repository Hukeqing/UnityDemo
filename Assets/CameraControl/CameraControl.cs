using System;
using System.Collections.Generic;
using System.Linq;
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

    [Serializable]
    public struct Target
    {
        public Transform pos;
        public bool important;

        public Target(Transform p, bool i = false)
        {
            pos = p;
            important = i;
        }
    }

    public class CameraControl : MonoBehaviour
    {
        private static readonly int LuminosityAmount = Shader.PropertyToID("_LuminosityAmount");
        private Camera _camera;

        private float _curMoveSpeed;
        private Shader _curShader;
        private Material _material;
        private float _preAngle;
        private Vector3 _preNormal;

        private Vector3 _prePosition;
        public float cameraDistance;
        public float cameraIgnoreDistance;
        public float cameraMaxDistance;

        public CameraMode cameraMode;
        public LayerMask cameraOnMask;
        public Vector3 clockPosition2, clockPosition1;
        public bool enableFixedDistance;
        public bool enableLinearInterpolation;
        public int freeDimension;
        [Range(0, 1)] public float grayScaleAmount = 1f;
        [Range(0, 1)] public float linearInterpolation;
        [Range(1, 89)] public float maxFieldAngle;
        public float minHeight;
        public Vector2 mouseRect;

        public float moveSpeed;

        public Transform player;
        public List<Target> targets;
        public bool useClock;
        public float zoomInSpeed;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            var thisTransform = transform;
            _prePosition = player.position - thisTransform.position;
            RotationAngle(player, thisTransform, out _preAngle, out _preNormal);

            _curShader = Shader.Find("CameraGrey/CameraGreyShader");
            _material = new Material(_curShader) {hideFlags = HideFlags.HideAndDontSave};

            if (targets == null)
            {
                targets = new List<Target>();
            }
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
                    Ove();
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

        private void Ove()
        {
            if (targets.Count == 0)
                return;
            var center = new Vector3(0, 0, 0);
            center = targets.Aggregate(center, (current, target) => current + target.pos.position);

            center /= targets.Count;
            switch (freeDimension)
            {
                case 0:
                    break;
                case 1:
                {
                    var flag = transform.position.x > center.x ? 1 : 0;
                    var y = Mathf.NegativeInfinity;
                    var tmp = center.x;
                    var imp = false;
                    foreach (var position in from target in targets where target.important select target.pos.position)
                    {
                        imp = true;
                        center.x = position.x;
                        y = Mathf.Max(y, Vector3.Distance(center, position) / Mathf.Tan(Mathf.Deg2Rad * maxFieldAngle));
                    }

                    if (imp)
                    {
                        center.x = tmp + flag * y;
                    }
                    else
                    {
                        center.x = transform.position.x;
                    }
                }
                    break;
                case 2:
                {
                    var flag = transform.position.y > center.y ? 1 : 0;
                    var y = minHeight;
                    var tmp = center.y;
                    var imp = false;
                    foreach (var position in from target in targets where target.important select target.pos.position)
                    {
                        imp = true;
                        center.y = position.y;
                        y = Mathf.Max(y, Vector3.Distance(center, position) / Mathf.Tan(Mathf.Deg2Rad * maxFieldAngle));
                    }

                    if (imp)
                    {
                        center.y = tmp + flag * y;
                    }
                    else
                    {
                        center.y = transform.position.y;
                    }
                }
                    break;
                case 3:
                {
                    var flag = transform.position.z > center.z ? 1 : 0;
                    var y = minHeight;
                    var tmp = center.z;
                    var imp = false;
                    foreach (var position in from target in targets where target.important select target.pos.position)
                    {
                        imp = true;
                        center.z = position.z;
                        y = Mathf.Max(y, Vector3.Distance(center, position) / Mathf.Tan(Mathf.Deg2Rad * maxFieldAngle));
                    }

                    if (imp)
                    {
                        center.z = tmp + flag * y;
                    }
                    else
                    {
                        center.z = transform.position.z;
                    }
                }
                    break;
            }

            transform.position = NextPosition(transform.position, center);
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