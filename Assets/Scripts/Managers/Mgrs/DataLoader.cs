using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace mgr
{
    [Serializable]
    public class LevelInfo
    {
        public string levelName;
        public string imageName;
    }
    [Serializable]
    public class LevelList
    {
        public int levelCount;
        public LevelInfo[] levelInfo;
    };
    public class DataLoader : ManagerBase<DataLoader>
    {
        public bool bInit { get; set; } = false;
        public LevelList levelList { get; private set; }
        private bool isReady;

        public event Action onDataLoad;

        public bool IsReady()
        {
            return isReady;
        }

        // TODO: Where do we do this? 
        protected override void _InitBeforeAwake()
        {
            base._InitBeforeAwake();
            isReady = false;

            Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Levels/LevelList.json").Completed +=
                (AsyncOperationHandle<TextAsset> handle) => {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        levelList = JsonUtility.FromJson<LevelList>(handle.Result.text);

                        isReady = true;
                        onDataLoad?.Invoke();
                        // The texture is ready for use.
                    }
                };
        }
    }
}
