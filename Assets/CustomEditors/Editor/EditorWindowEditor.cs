using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace CustomEditors.Editor
{
    public enum Enums
    {
        Enum0, Enum1, Enum2
    }
    public class EditorWindowEditor : EditorWindow
    {
        [MenuItem("Unity编辑器/OneEditorWindow")]
        public static void OpenWindow()
        {
            //打开窗口的方法 注释掉的方法你可以设置打开的窗口的位置和大小
            //可以直接用没参数的重载方法,参数用来设置窗口名称，是否用标签窗口形式之类的
            EditorWindow.GetWindow<EditorWindowEditor>(true, "Windows");
            //EditorWindow.GetWindowWithRect<MyFirstWindow>(new Rect(0f, 0f, 500, 500));
        }

        private bool _foldout = true;
        private float _knob;
        private int _popup;
        private float _slider;
        private bool _toggle;
        private Bounds _boundsField;
        private Color _colorField;
        private AnimationCurve _curveField = new AnimationCurve();
        private double _doubleField;
        private Enums _enumPopup;
        private float _floatField;
        private Gradient _gradientField = new Gradient();

        //OnGUI里写你想绘制在窗口里的内容
        private void OnGUI()
        {
            _foldout = EditorGUILayout.Foldout(_foldout, "foldout");
            if (!_foldout) return;
            _knob = EditorGUILayout.Knob(new Vector2(50, 50), _knob, -10, 10, "knob", Color.black, Color.blue, true);
            _popup = EditorGUILayout.Popup("popup", _popup, new[] {"0", "1", "2"});
            EditorGUILayout.Separator();
            _slider = EditorGUILayout.Slider("slider", _slider, -1, 1);
            EditorGUILayout.Space();
            _toggle = EditorGUILayout.Toggle("toggle", _toggle);
            _boundsField = EditorGUILayout.BoundsField("boundsField", _boundsField);
            _colorField = EditorGUILayout.ColorField("colorField", _colorField);
            _curveField = EditorGUILayout.CurveField("curveField", _curveField);
            _doubleField = EditorGUILayout.DoubleField("doubleField", _doubleField);
            EditorGUILayout.DropdownButton(new GUIContent("GUIContent", "tip"), FocusType.Keyboard);
            _enumPopup = (Enums)EditorGUILayout.EnumPopup("enumPopup", _enumPopup);
            _floatField = EditorGUILayout.FloatField("floatField", _floatField);
            _gradientField = EditorGUILayout.GradientField("gradientField", _gradientField);
            EditorGUILayout.HelpBox("helpBox", MessageType.None);
            EditorGUILayout.InspectorTitlebar(true, focusedWindow);
        }
    }
}