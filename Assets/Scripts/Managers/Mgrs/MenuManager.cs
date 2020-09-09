using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace mgr
{
    public class MenuManager : ManagerBase<MenuManager>
    {
        private const string MENU_ASSET_PREFIX = "Assets/Prefabs/Menu/";

        public GameObject currentMenu;

        // TODO: make a map?
        public GameObject infoMenuPrefab;
        public GameObject pauseMenuPrefab;

//        public event Action<GameObject/*background*/> buildInfoMenu;
//        public event Action<GameObject/*background*/> buildPauseMenu;

        // Start is called before the first frame update
        private void Awake()
        {
            Addressables.LoadAssetAsync<GameObject>(MENU_ASSET_PREFIX + "UIInfo.prefab").Completed +=
                (AsyncOperationHandle<GameObject> handle) => {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        infoMenuPrefab = handle.Result;
                    }
                };

            Addressables.LoadAssetAsync<GameObject>(MENU_ASSET_PREFIX + "UIPauseMenu.prefab").Completed +=
                (AsyncOperationHandle<GameObject> handle) => {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        pauseMenuPrefab = handle.Result;
                    }
                };            
        }
        
        public void CreateMenu(GameObject background, String name)
        {
            if (currentMenu != null)
            {
                Debug.Log("Creating menu when another menu exists. This should not happen.");
                DestroyMenu();
            }
            Debug.Log("CreateMenu: " + name);
            GameObject prefab = null;
            if (name == "info_menu")
            {
                prefab = infoMenuPrefab;
            } else if (name == "pause_menu")
            {
                prefab = pauseMenuPrefab;
            } else
            {
                Debug.Log("Wrong parameter at CreateMenu: " + name);
            }
            currentMenu = Instantiate(prefab);
            currentMenu.transform.SetParent(background.transform, false);
            // TODO: add logic regarding menu conflict, etc.
        }

        public void DestroyMenu()
        {
            Destroy(currentMenu);
        }
    }
}
