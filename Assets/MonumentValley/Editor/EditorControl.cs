using System;
using UnityEditor;
using UnityEngine;

namespace MonumentValley.Editor
{
    [CustomEditor(typeof(Player))]
    public class EditorControl : UnityEditor.Editor
    {
        private SerializedProperty _moveSpeed;
        private SerializedProperty _route;
        private string _str = "start";

        private void OnEnable()
        {
            _moveSpeed = serializedObject.FindProperty("moveSpeed");
            _route = serializedObject.FindProperty("route");
        }

        public override void OnInspectorGUI()
        {
//            serializedObject.Update();
            DrawDefaultInspector();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(_str))
            {
                ((Player) target).start = !((Player) target).start;
                _str = ((Player) target).start ? "stop" : "start";
            }

            if (GUILayout.Button("Back"))
            {
                ((Player) target).transform.position = new Vector3(0, 14, -6);
                ((Player) target).curTarget = 0;
            }

            GUILayout.EndHorizontal();
        }
    }
}