using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MonumentValley
{
    public class Player : MonoBehaviour
    {
        public float moveSpeed;
        public List<Transform> route;
        [HideInInspector] public bool start;
        [HideInInspector] public int curTarget;

        private void Start() => curTarget = 0;

        private void Update()
        {
            if (!start) return;
            if (curTarget >= route.Count)
                return;
            Transform selfTransform;
            (selfTransform = transform).LookAt(route[curTarget]);
            transform.Translate(moveSpeed * Time.deltaTime * Vector3.forward);
            if (Vector3.Distance(selfTransform.position, route[curTarget].position) < 0.1)
                curTarget++;
            if (curTarget != 2) return;
            curTarget = 3;
            transform.position = route[2].position;
        }
    }
}