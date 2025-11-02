using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace ProjectBase.Service
{
    /// <summary>
    /// 资产服务（Resources版本）
    /// </summary>
    public class AssetService : IAssetService
    {
        public T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }

        public T[] LoadAll<T>(string path) where T : Object
        {
            return Resources.LoadAll<T>(path);
        }
    }
}