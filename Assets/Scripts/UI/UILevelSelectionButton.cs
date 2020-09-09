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
        Addressables.LoadAssetAsync<Sprite>(ASSET_PREFIX + level_image).Completed += LevelImage_Completed;
        m_isSelectable = true;
        levelId = level_id;
        Refresh();
    }
    private void LevelImage_Completed(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            clickableImage = handle.Result;
            Refresh();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        isSelectable = false;
        Addressables.LoadAssetAsync<Sprite>(ASSET_PREFIX + "groundbig.png").Completed += LockedImage_Completed;
        levelSelector.onClick.AddListener(LoadLevel);
    }
    // TODO: there must be a way to merge LevelImage_Completed with LockedImage_Completed.
    private void LockedImage_Completed(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            lockedImage = handle.Result;
            Refresh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ???
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
        // Call GameManager's load game
        // Load LevelManager somehow
        //        Sticky.CurrentLevel = level_id_;
        //        Sticky.CurrentLevelName = level_name_;
//        LevelManager.Instance.LoadLevel(levelId);
        LevelManager.Instance.SetLevel(levelId);
        SceneManager.LoadScene("Main");
    }
}
