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
        private float _linearInterpolationValue;

        private SerializedProperty _mouseRect;
        private SerializedProperty _moveSpeed;

        private Object _player;

        private void OnEnable()
        {
            _this = target as CameraControl;
            _cameraMode = serializedObject.FindProperty("cameraMode");
            _mouseRect = serializedObject.FindProperty("mouseRect");
            _moveSpeed = serializedObject.FindProperty("moveSpeed");
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
                _linearInterpolationValue = EditorGUILayout.Slider(
                    "Value: " + _this.linearInterpolation.ToString(CultureInfo.CurrentCulture),
                    _linearInterpolationValue, 0,
                    1);
                _this.linearInterpolation = (float) ((Math.Exp(_linearInterpolationValue) - 1) / (Math.E - 1));
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
//                    EditorGUILayout.PropertyField(_mouseRect, true);
                    _this.mouseRect.x = EditorGUILayout.IntSlider("Horizontal(px)", (int) _this.mouseRect.x, 0, 20);
                    _this.mouseRect.y = EditorGUILayout.IntSlider("Vertical(px)", (int) _this.mouseRect.y, 0, 20);
                    if (_this.mouseRect.x < 5 || _this.mouseRect.y < 5)
                        EditorGUILayout.HelpBox(
                            "This is not a good value for some PC\nPlease set the value greater than 5px",
                            MessageType.Warning);
                    EditorGUILayout.PropertyField(_moveSpeed);
                    serializedObject.ApplyModifiedProperties();
                    break;
                case 3:
                    break;
            }
        }

        private void OnSceneGUI()
        {
//            Handles.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        }
    }
}