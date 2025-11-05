using UnityEditor;
using UnityEngine;

namespace ProjectBase.Editor
{
    public class SceneStructureTools : MonoBehaviour
    {

        [MenuItem("GameObject/创建场景结构", false, 1000)]
        public static void CreateFullSceneStructure()
        {
            Undo.SetCurrentGroupName($"Create Full Scene Structure");
            int group = Undo.GetCurrentGroup();

            // 创建根节点
            GameObject root = CreateObject($"Level");
            
            // 创建所有子结构
            CreateEnvironmentStructure(root);
            CreateDynamicStructure(root);
            CreateFXStructure(root);

            // 选中根节点
            Selection.activeGameObject = root;
            Undo.CollapseUndoOperations(group);
        }

        // 创建环境结构
        private static void CreateEnvironmentStructure(GameObject parent)
        {
            GameObject environment = CreateChildObject(parent, "Environment");
            CreateChildObject(environment, "Static");
            
            // GameObject lighting = CreateChildObject(environment, "BakeLighting");
            
            // // 如果场景中没有主光源，创建一个
            // if (GameObject.FindObjectOfType<Light>() == null)
            // {
            //     GameObject mainLight = CreateChildObject(lighting, "Light_Directional_Sun");
            //     Light lightComp = mainLight.AddComponent<Light>();
            //     lightComp.type = LightType.Directional;
            //     lightComp.color = Color.white;
            //     lightComp.intensity = 1f;
            // }
        }

        // 创建动态对象结构
        private static void CreateDynamicStructure(GameObject parent)
        {
            GameObject dynamic = CreateChildObject(parent, "Dynamic");
        }

        // 创建特效结构
        private static void CreateFXStructure(GameObject parent)
        {
            GameObject fx = CreateChildObject(parent, "FX");
        }

        // 工具方法：创建子对象
        private static GameObject CreateChildObject(GameObject parent, string name)
        {
            GameObject obj = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");
            obj.transform.SetParent(parent.transform);
            return obj;
        }

        // 工具方法：创建独立对象
        private static GameObject CreateObject(string name)
        {
            GameObject obj = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");
            return obj;
        }
    }
}