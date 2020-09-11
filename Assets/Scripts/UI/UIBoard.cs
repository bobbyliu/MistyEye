using mgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIBoard : MonoBehaviour
{
    private const string LEVEL_RULE_ASSET_PREFIX = "Assets/Data/Levels/";

    public GameObject cardButtonPrefab;
    public UICardButton[] uiCardButtons;
    // TODO: remove all these?
    public GameObject background;
    private float cardSize;
    private float borderSize;
    private float cardGap;
    public UIBoardMenu boardMenu;

    private bool needInitialize;

    // Start is called before the first frame update
    void Start()
    {
        InitializeCanvas();

        LevelManager.Instance.onClearLevel += ClearBoard;
        LevelManager.Instance.onLoadLevelData += InitializeBoard;
        LevelManager.Instance.LoadLevel();
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.onClearLevel -= ClearBoard;
            LevelManager.Instance.onLoadLevelData -= InitializeBoard;
        }
    }

    void ClearBoard()
    {
        Debug.Log("Clear board");
        foreach (var uiCardButton in uiCardButtons)
        {
            Destroy(uiCardButton.gameObject);
        }
        LevelManager.Instance.onForceFlipBack -= ForceFlipBack;
        LevelManager.Instance.onRemove -= RemoveCard;
        LevelManager.Instance.onFinish -= LoadFinishMenu;
    }

    void InitializeBoard()
    {
        uiCardButtons = new UICardButton[0];
        InitializeEmptyButtons();
        LoadCardButtons();
        LevelManager.Instance.onForceFlipBack += ForceFlipBack;
        LevelManager.Instance.onRemove += RemoveCard;
        LevelManager.Instance.onFinish += LoadFinishMenu;

        Debug.Log("Loading rule image:" + LEVEL_RULE_ASSET_PREFIX + LevelManager.Instance.levelData.ruleImageName);
        Addressables.LoadAssetAsync<Sprite>(LEVEL_RULE_ASSET_PREFIX + LevelManager.Instance.levelData.ruleImageName).Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    boardMenu.SetFormulaImage(handle.Result);
                }
            };
//        boardMenu.SetFormulaImage();
    }

    void ForceFlipBack(int card_id)
    {
        uiCardButtons[card_id].cardStatus = UICardButton.CardStatus.NORMAL;
    }

    void RemoveCard(int card_id)
    {
        uiCardButtons[card_id].cardStatus = UICardButton.CardStatus.REMOVED;
    }

    // Update is called once per frame
    void Update()
    {
    }

    // not sure if this should happen here.
    void InitializeCanvas()
    {
        float window_width = background.GetComponent<RectTransform>().rect.width;
        float window_height = background.GetComponent<RectTransform>().rect.height;

        if (window_width > window_height)
        {
            // Landscape, ignore
        }
        else
        {
            // Portrait
            //            float target_height = 820.0f * window_height / 1136.0f;
            //            float target_width = 550.0f * window_height / 1136.0f;
            cardSize = 120.0f * window_height / 1136.0f; // change it with respect to card number
            cardGap = 10.0f * window_height / 1136.0f;
            //            this.GetComponent<RectTransform>().sizeDelta = new Vector2(target_width, target_height);

            //            menu.GetComponent<RectTransform>().sizeDelta = new Vector2(window_height * 640.0f / 1136.0f, 0.0f);
        }
    }
    
    void InitializeEmptyButtons()
    {
        int row_count = LevelManager.Instance.boardRuleLogic.rowCount;
        int col_count = LevelManager.Instance.boardRuleLogic.columnCount;
        System.Array.Resize(ref uiCardButtons, row_count * col_count);
        Debug.Log("InitializeEmptyButtons." + "row_count:" + row_count + "col_count:" + col_count);
        for (int i = 0; i < row_count * col_count; i++)
        {
            int row = i % col_count;
            int col = row_count - 1 - i / col_count;

            // TODO: change all these to using card and size. 
            var new_card = Instantiate(cardButtonPrefab);
            // TODO: ???
            new_card.transform.SetParent(gameObject.transform, false);
            new_card.GetComponent<RectTransform>().sizeDelta = new Vector2(cardSize, cardSize);
            new_card.transform.localPosition =
                new Vector3((float)((row - (col_count / 2.0 - 0.5)) * (cardSize + cardGap)), (float)((col - (row_count / 2.0 - 0.5)) * (cardSize + cardGap)), 0);

            uiCardButtons[i] = new_card.GetComponent<UICardButton>();

            /*            new_cardcontroller.level_id_ = i;*/
        }
        return;
    }

    void LoadCardButtons()
    {
        int row_count = LevelManager.Instance.boardRuleLogic.rowCount;
        int col_count = LevelManager.Instance.boardRuleLogic.columnCount;
        Debug.Log("LoadCardButtons." + "row_count:" + row_count + "col_count:" + col_count);
        for (int i = 0; i < row_count * col_count; i++)
        {
            // TODO: we only support 24 max stuff here. 
            uiCardButtons[i].SetCardAndEnable(i);
        }
    }

    void LoadFinishMenu()
    {
        MenuManager.Instance.CreateMenu(background, "pause_menu");
        MenuManager.Instance.currentMenu.GetComponent<UIPauseMenu>().SetState(UIPauseMenu.State.SUCCESS);
    }
}
