using UnityEngine;
using UnityEngine.UI;

namespace StateSwitcher.Editor
{
    /// <summary>
    /// Editor工具类
    /// </summary>
    public static class EditorUtils
    {
        /// <summary>
        /// 从目标对象获取GameObject
        /// </summary>
        public static GameObject GetGameObjectFromTarget(UnityEngine.Object target)
        {
            if (target == null)
            {
                return null;
            }

            // 如果是GameObject，直接返回
            GameObject go = target as GameObject;
            if (go != null)
            {
                return go;
            }

            // 如果是Component，返回其GameObject
            Component comp = target as Component;
            if (comp != null)
            {
                return comp.gameObject;
            }

            return null;
        }

        /// <summary>
        /// 获取目标类型描述字符串
        /// </summary>
        public static string GetTargetTypeString(UnityEngine.Object target)
        {
            if (target == null)
            {
                return "None";
            }

            GameObject go = target as GameObject;
            if (go != null)
            {
                return $"GameObject ({go.name})";
            }

            Component comp = target as Component;
            if (comp != null)
            {
                return $"{comp.GetType().Name} (on {comp.gameObject.name})";
            }

            return target.GetType().Name;
        }

        /// <summary>
        /// 获取目标信息字符串
        /// </summary>
        public static string GetTargetInfoString(UnityEngine.Object targetObject, GameObject targetGo)
        {
            if (targetObject is GameObject)
            {
                var components = new System.Collections.Generic.List<string>();
                if (targetGo.GetComponent<Image>()) components.Add("Image");
                if (targetGo.GetComponent<Button>()) components.Add("Button");
                if (targetGo.GetComponent<Text>()) components.Add("Text");
                
                if (components.Count > 0)
                {
                    return $"GameObject ({string.Join(", ", components.ToArray())})";
                }
                return "GameObject";
            }
            else if (targetObject is Component comp)
            {
                return comp.GetType().Name;
            }
            
            return "Unknown";
        }

        /// <summary>
        /// 将字符串转换为有效的C#标识符
        /// </summary>
        public static string MakeValidIdentifier(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "State";
            }
            
            // 移除空格和特殊字符
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            
            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    sb.Append(c);
                }
            }
            
            string result = sb.ToString();
            
            // 确保首字符不是数字
            if (result.Length > 0 && char.IsDigit(result[0]))
            {
                result = "_" + result;
            }
            
            // 如果结果为空，返回默认值
            if (string.IsNullOrEmpty(result))
            {
                result = "State";
            }
            
            return result;
        }
    }
}

