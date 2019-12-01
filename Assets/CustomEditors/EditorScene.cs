using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CustomEditors
{
    [ExecuteInEditMode]
    public class EditorScene : MonoBehaviour
    {
        public Vector3 target;
        public bool isLook;

        public void Update()
        {
            if (isLook)
                transform.LookAt(target);
        }
    }
}