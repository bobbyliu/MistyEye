using mgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevelSelectionTabButton : MonoBehaviour
{
    private const string ASSET_PREFIX = "Assets/Data/Cards/";
    private Sprite lockedImage;
    private Sprite clickableImage;
    public Button tabSelector;

    // TODO: should this call be done from here or DataLoader?
    public UILevelSelection levelSelectionUI;
    // Used to pass level loading status.
    public int tabId { get; set; }

    private bool m_isSelectable;
    public bool isSelectable
    {
        private get { return m_isSelectable; }
        set
        {
            m_isSelectable = value;
            Refresh();
        }
    }

    public void SetTabNumberAndEnable(int tab_id, string tab_image)
    {
        Addressables.LoadAssetAsync<Sprite>(ASSET_PREFIX + tab_image).Completed +=
            (AsyncOperationHandle<Sprite> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    clickableImage = handle.Result;
                    Refresh();
                }
            };
        m_isSelectable = true;
        tabId = tab_id;
        Refresh();
    }

    void Awake()
    {
        isSelectable = false;
        Addressables.LoadAssetAsync<Sprite>(ASSET_PREFIX + "groundbig.png").Completed +=
            (AsyncOperationHandle<Sprite> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    lockedImage = handle.Result;
                    Refresh();
                }
            };

        tabSelector.onClick.AddListener(ChangeTab);
    }

    void Refresh()
    {
        if (isSelectable)
        {
            tabSelector.GetComponent<Image>().sprite = clickableImage;
            tabSelector.interactable = true;
        }
        else
        {
            tabSelector.GetComponent<Image>().sprite = lockedImage;
            tabSelector.interactable = false;
        }
    }

    public void ChangeTab()
    {
        DataLoader.Instance.playerData.currentTab = tabId;
        levelSelectionUI.LoadLevelSelectionButtons();
    }
}
