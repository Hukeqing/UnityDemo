using UnityEngine;

namespace Jump
{
    public class JumpScript : MonoBehaviour
    {
        public AnimationCurve jumpCurve;
        
        private bool _isJump;
        private float _jumpStatus;
        private Vector3 _baseVector3;

        private void Update()
        {
            if (!_isJump && Input.GetKeyDown(KeyCode.Space))
            {
                _isJump = true;
                _jumpStatus = 0;
                _baseVector3 = transform.position;
            }

            if (!_isJump) return;
            _jumpStatus += Time.deltaTime;
            if (_jumpStatus >= 1.0f)
            {
                _isJump = false;
                transform.position = _baseVector3;
            }

            transform.position = _baseVector3 + jumpCurve.Evaluate(_jumpStatus) * Vector3.up;
        }
    }
}