using System;
using UnityEngine;

namespace Jump
{
    public class JumpScript : MonoBehaviour
    {
        public AnimationCurve jumpCurve;
        public float moveSpeed;

        private Rigidbody _rigidBody;
        private bool _isJump;
        private float _jumpStatus;
        private float _lastY;

        private void Start()
        {
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            var target = new Vector3(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0,
                Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);

            if (!_isJump && Input.GetKeyDown(KeyCode.Space))
            {
                _isJump = true;
                _jumpStatus = 0;
                _lastY = 0;
                _rigidBody.useGravity = false;
            }

            if (_isJump)
            {
                _jumpStatus += Time.deltaTime;
                if (_jumpStatus >= 1.0f)
                {
                    _isJump = false;
                    _rigidBody.useGravity = true;
                }

                target.y += jumpCurve.Evaluate(_jumpStatus) - _lastY;
                _lastY = jumpCurve.Evaluate(_jumpStatus);
            }

            transform.Translate(target);
        }
    }
}