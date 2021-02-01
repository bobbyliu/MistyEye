using mgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevelSelectionButton : MonoBehaviour
{
    private const string ASSET_PREFIX = "Assets/Data/Cards/";
    private Sprite lockedImage;
    private Sprite clickableImage;
    public Button levelSelector;
    // Used to pass level loading status.
    public int levelId { get; set; }

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

    public void SetLevelAndEnable(int level_id, string level_image) {
        Addressables.LoadAssetAsync<Sprite>(ASSET_PREFIX + level_image).Completed +=
            (AsyncOperationHandle<Sprite> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    clickableImage = handle.Result;
                    Refresh();
                }
            };
        m_isSelectable = true;
        levelId = level_id;
        Refresh();
    }
    public void DisableLevel()
    {
        m_isSelectable = false;
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

        levelSelector.onClick.AddListener(LoadLevel);
    }

    void Refresh()
    {
        if (isSelectable)
        {
            levelSelector.GetComponent<Image>().sprite = clickableImage;
            levelSelector.interactable = true;
        }
        else
        {
            levelSelector.GetComponent<Image>().sprite = lockedImage;
            levelSelector.interactable = false;
        }
    }

    public void LoadLevel()
    {
        LevelManager.Instance.mainLevelId = levelId;
        if (DataLoader.Instance.levelList.levelInfo[levelId].levelType == LevelInfo.LevelType.GAUNTLET)
        {
            LevelManager.Instance.SetInfinityLevel(
                DataLoader.Instance.levelList.levelInfo[levelId].gauntletRange);
        }
        else
        {
            LevelManager.Instance.SetLevel(levelId);
        }
        SceneManager.LoadScene("Main");
    }
}
