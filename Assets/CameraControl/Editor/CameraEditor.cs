using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CameraControl.Editor
{
    [CustomEditor(typeof(CameraControl))]
    public class CameraEditor : UnityEditor.Editor
    {
        private CameraControl _this;

        private SerializedProperty _cameraMode;

        private Object _player;

        private float _test;

        private void OnEnable()
        {
            _this = target as CameraControl;
            _cameraMode = serializedObject.FindProperty("cameraMode");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_cameraMode);
            // _this.enableLinearInterpolation = EditorGUILayout.Toggle(
            // _this.enableLinearInterpolation ? "Enabled Linear" : "Disabled Linear", _this.enableLinearInterpolation);
            _this.enableLinearInterpolation = EditorGUILayout.Foldout(_this.enableLinearInterpolation,
                _this.enableLinearInterpolation ? "Enabled Linear Interpolation" : "Disabled Linear Interpolation");
            serializedObject.ApplyModifiedProperties();
            if (_this.enableLinearInterpolation)
            {
                _test = EditorGUILayout.Slider(
                    "Value: " + _this.linearInterpolation.ToString(CultureInfo.CurrentCulture), _test, 0,
                    1);
                _this.linearInterpolation = (float) ((Math.Exp(_test) - 1) / Math.E);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Current Camera Mode: ", _this.cameraMode.ToString());
            EditorGUILayout.Separator();
            
            switch (_cameraMode.enumValueIndex)
            {
                case 0:
                case 1:
                    _this.player =
                        EditorGUILayout.ObjectField("Target ", _this.player, typeof(Transform), true) as Transform;
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        }
    }
}