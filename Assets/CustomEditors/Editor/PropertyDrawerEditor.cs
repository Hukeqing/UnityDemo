using UnityEditor;
using UnityEngine;

namespace CustomEditors.Editor
{
    [CustomPropertyDrawer(typeof(Ingredient))]
    public class PropertyDrawerEditor : PropertyDrawer
    {
        //重新绘制面板
        // ReSharper disable once InconsistentNaming
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            //本来是1层，要缩进，设置成0和他的上级size同级，使之与其对齐；
            var index = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            //给三个属性安排位置
            var amountRect = new Rect(position.x, position.y, 30, position.height);
            var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
            var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

            //绘制属性
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("unit"), GUIContent.none);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

            EditorGUI.indentLevel = 1;
            //重新设置为原来的层级
            EditorGUI.indentLevel = index;

            EditorGUI.EndProperty();
        }
    }
}