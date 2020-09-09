using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace common
{
    [DefaultExecutionOrder(-1)]
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected static T _instance = null;

        public static T Instance
        {
            get
            {
                return GetInstance();
            }
        }

        public static T GetInstance()
        {
            if (_instance == null)
            {
                var name = typeof(T).Name;
                var obj = GameObject.Find(name);
                if (obj == null)
                {
                    obj = new GameObject(name);
                    DontDestroyOnLoad(obj);
                }
                _instance = obj.GetComponent<T>();
                if (_instance == null)
                {
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}