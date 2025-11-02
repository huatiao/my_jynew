using UnityEngine;

namespace ProjectBase.Service
{
    public interface IAssetService
    {
        T Load<T>(string path) where T : Object;
        T[] LoadAll<T>(string path) where T : Object;
    }
}