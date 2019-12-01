using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomEditors.Editor
{
    [CustomEditor(typeof(EditorScene))]
    public class EditorEditor : UnityEditor.Editor
    {
        private SerializedProperty _target;

        private void OnEnable()
        {
            _target = serializedObject.FindProperty("target");
        }

        public override void OnInspectorGUI()
        {
//            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(_target);
            var t = target as EditorScene;
            Debug.Assert(t != null, nameof(t) + " != null");
            if (GUILayout.Button("change"))
            {
                t.isLook = !t.isLook;
                t.Update();
            }

            if (GUILayout.Button("random"))
            {
                t.target = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
                t.Update();
            }
        }

        public void OnSceneGUI()
        {
            var t = target as EditorScene;
            EditorGUI.BeginChangeCheck();
            //绘制一个图形（移动物体的那个三箭头），位置对应lookPos
            Debug.Assert(t != null, nameof(t) + " != null");
            var pos = Handles.PositionHandle(t.target, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move point");
                t.target = pos;
                t.Update();
            }

            var transform = t.transform;
            var position = transform.position;
            var up = transform.up;
            var forward = transform.forward;
            Handles.color = new Color(1, 1, 1, 0.2f);
            Handles.DrawSolidArc(position, up, forward, 20, 10);
            Handles.DrawSolidArc(position, up, forward, -20, 10);
//            Handles.DrawWireArc(position, up,forward, 360, 10);
        }
    }
}