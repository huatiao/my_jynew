using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ProjectBase.UI;


namespace ProjectBase.Editor.UI
{
    /// <summary>
    /// UIID代码生成器
    /// </summary>
    public class UICodeGenerator
    {
        #region 常量定义
        private const string PREF_EXPORT_NAMESPACE = "UICodeGenerator.ExportNamespace";
        private const string PREF_EXPORT_CLASSNAME = "UICodeGenerator.ExportClassName";
        private const string PREF_EXPORT_PATH = "UICodeGenerator.ExportPath";
        
        private const string DEFAULT_NAMESPACE = "ProjectBase.Define";
        private const string DEFAULT_CLASSNAME = "UIID";
        private const string DEFAULT_EXPORT_PATH = "Assets/Scripts/Define/UIID.cs";
        
        // 模板文件路径
        private const string TEMPLATE_FILE = "Assets/Editor/UI/Templates/UIIDTempate.txt";
        private const string TEMPLATE_VIEW = "Assets/Editor/UI/Templates/WindowView.cs.txt";
        private const string TEMPLATE_VIEWMODEL = "Assets/Editor/UI/Templates/ViewModel.cs.txt";
        
        // 代码生成设置
        private const string PREF_VIEW_PATH = "UICodeGenerator.ViewPath";
        private const string PREF_VIEWMODEL_PATH = "UICodeGenerator.ViewModelPath";
        private const string PREF_CODE_NAMESPACE = "UICodeGenerator.CodeNamespace";
        
        private const string DEFAULT_VIEW_PATH = "Assets/Scripts/UI";
        private const string DEFAULT_VIEWMODEL_PATH = "Assets/Scripts/UI";
        private const string DEFAULT_CODE_NAMESPACE = "ProjectBase.UI";
        #endregion

        #region 公共属性
        public string ExportNamespace { get; set; }
        public string ExportClassName { get; set; }
        public string ExportPath { get; set; }
        
        public string ViewPath { get; set; }
        public string ViewModelPath { get; set; }
        public string CodeNamespace { get; set; }
        #endregion

        #region 初始化
        public UICodeGenerator()
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            ExportNamespace = EditorPrefs.GetString(PREF_EXPORT_NAMESPACE, DEFAULT_NAMESPACE);
            ExportClassName = EditorPrefs.GetString(PREF_EXPORT_CLASSNAME, DEFAULT_CLASSNAME);
            ExportPath = EditorPrefs.GetString(PREF_EXPORT_PATH, DEFAULT_EXPORT_PATH);
            
            ViewPath = EditorPrefs.GetString(PREF_VIEW_PATH, DEFAULT_VIEW_PATH);
            ViewModelPath = EditorPrefs.GetString(PREF_VIEWMODEL_PATH, DEFAULT_VIEWMODEL_PATH);
            CodeNamespace = EditorPrefs.GetString(PREF_CODE_NAMESPACE, DEFAULT_CODE_NAMESPACE);
        }

        public void SaveSettings()
        {
            EditorPrefs.SetString(PREF_EXPORT_NAMESPACE, ExportNamespace);
            EditorPrefs.SetString(PREF_EXPORT_CLASSNAME, ExportClassName);
            EditorPrefs.SetString(PREF_EXPORT_PATH, ExportPath);
            
            EditorPrefs.SetString(PREF_VIEW_PATH, ViewPath);
            EditorPrefs.SetString(PREF_VIEWMODEL_PATH, ViewModelPath);
            EditorPrefs.SetString(PREF_CODE_NAMESPACE, CodeNamespace);
        }
        #endregion

        #region 生成方法
        /// <summary>
        /// 生成UIID文件
        /// </summary>
        /// <param name="dataList">UI配置数据列表</param>
        /// <returns>是否生成成功</returns>
        public bool Generate(List<UIConfData> dataList)
        {
            // 验证数据
            if (!ValidateData(dataList))
            {
                return false;
            }

            try
            {
                // 读取模板文件
                if (!File.Exists(TEMPLATE_FILE))
                {
                    EditorUtility.DisplayDialog("导出失败", $"模板文件不存在:\n{TEMPLATE_FILE}", "确定");
                    Debug.LogError($"模板文件不存在: {TEMPLATE_FILE}");
                    return false;
                }

                string template = File.ReadAllText(TEMPLATE_FILE);

                // 生成内容
                string content = GenerateContent(dataList);

                // 替换占位符
                string result = template
                    .Replace("#DATETIME#", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    .Replace("#NAMESPACE#", ExportNamespace)
                    .Replace("#CLASSNAME#", ExportClassName)
                    .Replace("#CONTENT#", content);

                // 确保目录存在
                string directory = Path.GetDirectoryName(ExportPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 写入文件
                File.WriteAllText(ExportPath, result);

                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("导出成功", 
                    $"UIID文件已成功导出到:\n{ExportPath}\n\n共导出 {dataList.Count} 个UI ID", 
                    "确定");
                
                return true;
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("导出失败", $"导出时发生错误:\n{ex.Message}", "确定");
                Debug.LogError($"导出UIID文件失败: {ex}");
                return false;
            }
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        public bool ValidateData(List<UIConfData> dataList)
        {
            // 检查是否有配置
            if (dataList == null || dataList.Count == 0)
            {
                EditorUtility.DisplayDialog("导出失败", "没有UI配置可导出！", "确定");
                return false;
            }

            // 检查是否有重复ID
            var duplicates = CheckDuplicateIDs(dataList);
            if (duplicates.Count > 0)
            {
                if (!EditorUtility.DisplayDialog("警告", 
                    $"检测到 {duplicates.Count} 个重复的ID: {string.Join(", ", duplicates)}\n\n是否继续导出？", 
                    "继续", "取消"))
                {
                    return false;
                }
            }

            // 检查是否有空路径
            int emptyPathCount = dataList.Count(data => string.IsNullOrEmpty(data.prefabPath));
            if (emptyPathCount > 0)
            {
                if (!EditorUtility.DisplayDialog("警告", 
                    $"有 {emptyPathCount} 个配置的预制体路径为空\n\n是否继续导出？", 
                    "继续", "取消"))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 生成View和ViewModel文件
        /// </summary>
        public bool GenerateViewAndViewModel(UIConfData data)
        {
            try
            {
                // 验证数据
                if (string.IsNullOrEmpty(data.prefabPath))
                {
                    EditorUtility.DisplayDialog("生成失败", "预制体路径为空！", "确定");
                    return false;
                }

                // 从预制体名称提取类名
                string prefabName = Path.GetFileNameWithoutExtension(data.prefabPath);
                if (string.IsNullOrEmpty(prefabName))
                {
                    EditorUtility.DisplayDialog("生成失败", "无法从预制体路径提取文件名！", "确定");
                    return false;
                }

                // 提取类名前缀（如 LobbyWindow -> Lobby）
                string classPrefix = ExtractClassPrefix(prefabName);

                // 类名和UIID
                string viewClassName = prefabName;
                string viewModelClassName = classPrefix + "ViewModel";
                string uiidName = ConvertCamelCaseToSnakeCase(prefabName).ToUpper();

                // 生成文件路径
                string viewFilePath = Path.Combine(ViewPath, classPrefix, viewClassName + ".cs");
                string viewModelFilePath = Path.Combine(ViewModelPath, classPrefix, viewModelClassName + ".cs");

                // 检查文件是否存在
                bool viewExists = File.Exists(viewFilePath);
                bool viewModelExists = File.Exists(viewModelFilePath);

                if (viewExists || viewModelExists)
                {
                    string message = "以下文件已存在:\n";
                    if (viewExists) message += $"- {viewFilePath}\n";
                    if (viewModelExists) message += $"- {viewModelFilePath}\n";
                    message += "\n您要如何处理？";

                    int choice = EditorUtility.DisplayDialogComplex("文件已存在", message, "覆盖", "取消", "跳过");
                    
                    if (choice == 1) // 取消
                    {
                        return false;
                    }
                    else if (choice == 2) // 跳过
                    {
                        EditorUtility.DisplayDialog("已跳过", "跳过生成已存在的文件", "确定");
                        return false;
                    }
                    // choice == 0 继续覆盖
                }

                // 读取模板
                if (!File.Exists(TEMPLATE_VIEW))
                {
                    EditorUtility.DisplayDialog("生成失败", $"View模板文件不存在:\n{TEMPLATE_VIEW}", "确定");
                    return false;
                }

                if (!File.Exists(TEMPLATE_VIEWMODEL))
                {
                    EditorUtility.DisplayDialog("生成失败", $"ViewModel模板文件不存在:\n{TEMPLATE_VIEWMODEL}", "确定");
                    return false;
                }

                string viewTemplate = File.ReadAllText(TEMPLATE_VIEW);
                string viewModelTemplate = File.ReadAllText(TEMPLATE_VIEWMODEL);

                // 替换占位符 - View
                string viewCode = viewTemplate
                    .Replace("#NAMESPACE#", CodeNamespace)
                    .Replace("#CLASSNAME#", viewClassName)
                    .Replace("#VIEWMODEL#", viewModelClassName)
                    .Replace("#UIID#", uiidName);

                // 替换占位符 - ViewModel
                string viewModelCode = viewModelTemplate
                    .Replace("#NAMESPACE#", CodeNamespace)
                    .Replace("#CLASSNAME#", viewModelClassName);

                // 确保目录存在
                string viewDir = Path.GetDirectoryName(viewFilePath);
                string viewModelDir = Path.GetDirectoryName(viewModelFilePath);

                if (!Directory.Exists(viewDir))
                {
                    Directory.CreateDirectory(viewDir);
                }

                if (!Directory.Exists(viewModelDir))
                {
                    Directory.CreateDirectory(viewModelDir);
                }

                // 写入文件
                File.WriteAllText(viewFilePath, viewCode);
                File.WriteAllText(viewModelFilePath, viewModelCode);

                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("生成成功", 
                    $"成功生成代码文件:\n\nView:\n{viewFilePath}\n\nViewModel:\n{viewModelFilePath}", 
                    "确定");

                return true;
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("生成失败", $"生成时发生错误:\n{ex.Message}", "确定");
                Debug.LogError($"生成View和ViewModel失败: {ex}");
                return false;
            }
        }
        #endregion

        #region 私有辅助方法
        /// <summary>
        /// 生成内容部分
        /// </summary>
        private string GenerateContent(List<UIConfData> dataList)
        {
            // 按ID排序导出
            var sortedData = dataList.OrderBy(data => data.id).ToList();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < sortedData.Count; i++)
            {
                var data = sortedData[i];
                string variableName = GenerateVariableName(data.prefabPath, data.id);

                // 添加注释
                if (!string.IsNullOrEmpty(data.description))
                {
                    sb.AppendLine($"        /// <summary>{data.description}</summary>");
                }

                sb.Append($"        public const int {variableName} = {data.id};");
                
                // 项之间添加空行（除了最后一项）
                if (i < sortedData.Count - 1)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private string GenerateVariableName(string prefabPath, int id)
        {
            // 使用预制体名称作为变量名
            string variableName = Path.GetFileNameWithoutExtension(prefabPath);

            // 确保变量名是有效的C#标识符
            variableName = System.Text.RegularExpressions.Regex.Replace(variableName, @"[^a-zA-Z0-9_]", "");
            
            if (string.IsNullOrEmpty(variableName) || char.IsDigit(variableName[0]))
            {
                variableName = "UI_" + id;
            }
            else
            {
                // 驼峰转下划线
                variableName = ConvertCamelCaseToSnakeCase(variableName);
            }

            // 转换为大写
            return variableName.ToUpper();
        }

        private string ConvertCamelCaseToSnakeCase(string input)
        {
            // 在小写字母/数字后跟大写字母的位置添加下划线
            string result = System.Text.RegularExpressions.Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2");
            
            // 在连续大写字母中，最后一个大写字母前添加下划线
            result = System.Text.RegularExpressions.Regex.Replace(result, @"([A-Z])([A-Z][a-z])", "$1_$2");
            
            return result;
        }

        /// <summary>
        /// 提取类名前缀（如 LobbyWindow -> Lobby, MainPanel -> Main）
        /// </summary>
        private string ExtractClassPrefix(string className)
        {
            // 移除常见后缀
            string[] suffixes = { "Window", "Panel", "View", "Dialog", "Popup" };
            
            foreach (var suffix in suffixes)
            {
                if (className.EndsWith(suffix) && className.Length > suffix.Length)
                {
                    return className.Substring(0, className.Length - suffix.Length);
                }
            }
            
            // 如果没有匹配的后缀，返回原始类名
            return className;
        }

        private HashSet<int> CheckDuplicateIDs(List<UIConfData> dataList)
        {
            HashSet<int> duplicates = new HashSet<int>();
            Dictionary<int, int> idCount = new Dictionary<int, int>();

            foreach (var data in dataList)
            {
                if (idCount.ContainsKey(data.id))
                {
                    idCount[data.id]++;
                    duplicates.Add(data.id);
                }
                else
                {
                    idCount[data.id] = 1;
                }
            }

            return duplicates;
        }
        #endregion
    }
}

