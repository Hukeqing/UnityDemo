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
        private SerializedProperty _grayScaleAmount;
        private float _linearInterpolationValue;

        private SerializedProperty _moveSpeed;
        private SerializedProperty _zoomInSpeed;

        private Object _player;

        private bool _clockSceneShow = true;
        private Color _clockColor = Color.red;

        private void OnEnable()
        {
            _this = target as CameraControl;
            _cameraMode = serializedObject.FindProperty("cameraMode");
            _grayScaleAmount = serializedObject.FindProperty("grayScaleAmount");
            _moveSpeed = serializedObject.FindProperty("moveSpeed");
            _zoomInSpeed = serializedObject.FindProperty("zoomInSpeed");
            // _this.maxMovePosition = new Vector3(10, 10, 10);
            // _this.minMovePosition = new Vector3(-10, -10, -10);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_cameraMode);
            EditorGUILayout.PropertyField(_grayScaleAmount);
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
                    EditorGUILayout.PropertyField(_zoomInSpeed);
                    serializedObject.ApplyModifiedProperties();
                    _this.useClock = EditorGUILayout.Foldout(_this.useClock,
                        _this.useClock ? "Enabled Clock" : "Disabled Clock");
                    if (_this.useClock)
                    {
                        _clockSceneShow = EditorGUILayout.Foldout(_clockSceneShow,
                            _clockSceneShow ? "Enabled Scene Rect" : "Disabled Scene Rect");
                        if (_clockSceneShow)
                        {
                            _clockColor = EditorGUILayout.ColorField("Scene Show Color: ", _clockColor);
                        }

                        _this.freeDimension = EditorGUILayout.Popup("Not Clock Dimension", _this.freeDimension,
                            new string[] {"None", "x", "y", "z"});
                        EditorGUILayout.BeginVertical();
                        if (_this.freeDimension != 1)
                        {
                            EditorGUILayout.LabelField("x: ");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Range from: ", GUILayout.Width(80));
                            _this.clockPosition1.x = EditorGUILayout.FloatField(_this.clockPosition1.x);
                            EditorGUILayout.LabelField(" to: ", GUILayout.Width(30));
                            _this.clockPosition2.x = EditorGUILayout.FloatField(_this.clockPosition2.x);
                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            _this.clockPosition1.x = 0;
                            _this.clockPosition2.x = 0;
                        }

                        if (_this.freeDimension != 2)
                        {
                            EditorGUILayout.LabelField("y: ");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Range from: ", GUILayout.Width(80));
                            _this.clockPosition1.y = EditorGUILayout.FloatField(_this.clockPosition1.y);
                            EditorGUILayout.LabelField(" to: ", GUILayout.Width(30));
                            _this.clockPosition2.y = EditorGUILayout.FloatField(_this.clockPosition2.y);
                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            _this.clockPosition1.y = 0;
                            _this.clockPosition2.y = 0;
                        }

                        if (_this.freeDimension != 3)
                        {
                            EditorGUILayout.LabelField("z: ");
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Range from: ", GUILayout.Width(80));
                            _this.clockPosition1.z = EditorGUILayout.FloatField(_this.clockPosition1.z);
                            EditorGUILayout.LabelField(" to: ", GUILayout.Width(30));
                            _this.clockPosition2.z = EditorGUILayout.FloatField(_this.clockPosition2.z);
                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            _this.clockPosition1.z = 0;
                            _this.clockPosition2.z = 0;
                        }

                        EditorGUILayout.EndVertical();
                    }
                    break;
                case 3:
                    break;
            }
        }

        private void OnSceneGUI()
        {
            if (!_this.useClock || !_clockSceneShow || _this.cameraMode != CameraMode.MouseControl) return;
            Handles.color = _clockColor;
            _this.clockPosition2 = Handles.PositionHandle(_this.clockPosition2, Quaternion.identity);
            _this.clockPosition1 = Handles.PositionHandle(_this.clockPosition1, Quaternion.identity);
            Handles.DrawWireCube((_this.clockPosition2 + _this.clockPosition1) / 2,
                _this.clockPosition2 - _this.clockPosition1);
        }
    }
}