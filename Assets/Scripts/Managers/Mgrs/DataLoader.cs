﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
    // Currently we only need to save the progress of the player.
    [Serializable]
    public class PlayerData
    {
        public int levelProgress;
    };
    public class DataLoader : ManagerBase<DataLoader>
    {
        public bool bInit { get; set; } = false;
        public LevelList levelList { get; private set; }
        public PlayerData playerData;
        private bool isReady;

        public event Action onDataLoad;

        public bool IsReady()
        {
            return isReady;
        }

        // TODO: Should this be here or another PlayerManager?
        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PlayerInfo.dat", FileMode.OpenOrCreate);

            bf.Serialize(file, playerData);
            file.Close();
        }
        public void Load()
        {
            Debug.Log("Load");
            if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
            {
                Debug.Log("Load from persistent file.");
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);

                playerData = bf.Deserialize(file) as PlayerData;
                if (playerData == null)
                {
                    Debug.Log("Load from persistent file failed.");
                    playerData = new PlayerData();
                    playerData.levelProgress = 0;
                }
                file.Close();
            }
            else
            {
                Debug.Log("File not found. Starting new save.");
                playerData = new PlayerData();
                playerData.levelProgress = 0;
            }
        }
        public void UpdateLevelProgress(int levelId)
        {
            if (playerData.levelProgress < levelId + 1)
            {
                playerData.levelProgress = levelId + 1;
                Save();
            }
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
                    }
                };
            Load();
        }
    }
}
