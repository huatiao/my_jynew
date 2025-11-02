using UnityEngine;
using ProjectBase.UI;
using System.Collections.Generic;
using ProjectBase.Service;
using ProjectBase.Define;
using VContainer;
using ProjectBase.Util;
using System.IO;
using Luban;

namespace ProjectBase.Model
{
    public class ConfigModel
    {
        [Inject] private IAssetService _assetService;


        /// <summary>
        /// 初始化配置
        /// </summary>
        public bool Init()
        {
            _uiConf = _assetService.Load<UIConf>(PathUtil.ASSET_MAINUICONF);

            InitLubanTables();

            return true;
        }

        private void InitLubanTables()
        {
            _tables = new LubanGen.Tables(file => 
                new ByteBuf(File.ReadAllBytes($"{PathUtil.ROOT_RESOURCES + PathUtil.DIR_CONFIGS_LUBANDATA_BIN}/{file}.bytes")));
        }

        #region UI配置

        private UIConf _uiConf;
        public List<UIConfData> UIConfDatas => _uiConf.ConfDataList;

        public UIConfData GetUIConfData(int id)
        {
            return _uiConf.ConfDataList.Find(data => data.id == id);
        }

        #endregion

        #region luban配置

        private LubanGen.Tables _tables;
        
        public LubanGen.Tables Tables => _tables;
        public LubanGen.TbCharacterData TbCharacterData => _tables.TbCharacterData;

        #endregion
    }
}