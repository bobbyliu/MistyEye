using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mgr
{
    public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
    {
        protected static T _instance = null;
        private static bool applicationIsQuitting = false;
        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                if (_instance == null)
                {
                    GameGlobal.GetInstance();
                }
                Debug.Assert(_instance != null);
                return _instance;
            }
        }

        public void InitBeforeAwake()
        {
            _instance = (T)this;
            _InitBeforeAwake();
        }

        protected virtual void _InitBeforeAwake()
        {
            //Debug.Log($"{_instance.name}._InitBeforeAwake!!!");
        }

        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }
    }
}