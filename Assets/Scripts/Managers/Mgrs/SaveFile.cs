using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mgr
{
    [Serializable]
    public class SaveData
    {
        public int[] levelStatus;
    };
    public class SaveFile : ManagerBase<SaveFile>
    {
        public bool bInit { get; set; } = false;
        public static readonly string SAVE_KEY = "SAVE_KEY";
        public SaveData saveData { get; private set; }

        protected override void _InitBeforeAwake()
        {
            base._InitBeforeAwake();
            saveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(SAVE_KEY));
        }

        // Save??
    }
}
