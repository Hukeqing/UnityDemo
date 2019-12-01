using System.Collections.Generic;
using UnityEngine;

namespace MonumentValley
{
    public class Player : MonoBehaviour
    {
        public float moveSpeed;
        public List<Transform> route;
        public bool start;

        private int _curTarget;

        private void Start() => _curTarget = 0;

        private void Update()
        {
            if (!start) return;
            if (_curTarget >= route.Count)
                return;
            Transform selfTransform;
            (selfTransform = transform).LookAt(route[_curTarget]);
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
            if (Vector3.Distance(selfTransform.position, route[_curTarget].position) < 0.1)
                _curTarget++;
            if (_curTarget != 2) return;
            _curTarget = 3;
            transform.position = route[2].position;
        }
    }
}