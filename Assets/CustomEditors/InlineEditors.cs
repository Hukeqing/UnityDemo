using UnityEditor;
using UnityEngine;

namespace CustomEditors
{
    // 此 Component 将被放置到上面的 Component 菜单下
    [AddComponentMenu("TestMenu/Test")]
    // 不允许单个物体拥有多个此类
    [DisallowMultipleComponent]
    // 在编辑模式下运行此类
    [ExecuteInEditMode]
    // 添加类之间的依赖
    [RequireComponent(typeof(Rigidbody))]
    public class InlineEditors : MonoBehaviour
    {
        // 在变量上方做注释
        [Header("这里会显示在上面")] public int a;

        // 变量将被隐藏
        [HideInInspector] public int b;

        // 变量限定范围
        [Range(-1, 10)] public int c;

        // 强制序列化，使得任何变量可以在 Inspector 中编辑
        [SerializeField] private int d;

        // 添加在 Inspector 中的空间
        [Space(40)] public int e;

        // 添加 Component 右侧的设置里的菜单
        [ContextMenu("Do Something")]
        private void DoSomething()
        {
            Debug.Log("Perform operation");
        }

        // 为变量添加右键方法
        [ContextMenuItem("Reset", "ResetName")]
        // 允许多行文本
        [MultilineAttribute]
        public new string name = "Default";

        private void ResetName()
        {
            name = "Default";
        }

        // 出现整块的编辑区
        [TextArea] public string str;

        // 悬停显示
        [Tooltip("这里会显示在悬停的时候")] public int f;

        // 这里会在上方菜单里创建一个按钮
        [MenuItem("MyMenu/Create GameObject")]
        private static void Test()
        {
            Debug.Log("Hit it!");
        }
    }
}