using System.Collections.Generic;
using UnityEngine;

namespace ProjectBase.UI
{
    [System.Serializable]
    public class UIConfData
    {
        public int id;  //唯一标识
        public string prefabPath;  //预制体路径
        public string description;  //描述
    }

    [CreateAssetMenu(menuName = "ProjectBase/UIConf", fileName = "NewUIConf")]
    public class UIConf : ScriptableObject
    {
        public List<UIConfData> ConfDataList = new();
    }
}