using mgr;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class UIBoard : MonoBehaviour
{
    private const string LEVEL_RULE_ASSET_PREFIX = "Assets/Data/Levels/";

    public GameObject cardButtonPrefab;
    // TODO: change to private serializable?
    public UICardButton[] uiCardButtons;
    public GridLayoutGroup gridLayoutGroup;
    // TODO: remove all these?
    public GameObject background;
    private int cardSize;
    private int cardGap;
    public UIBoardMenu boardMenu;

    private bool needInitialize;

    // Start is called before the first frame update
    void Start()
    {
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
        InitializeCanvas();
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
        int max_width = Mathf.RoundToInt(background.GetComponent<RectTransform>().rect.width * 550.0f / 640.0f);
        int max_height = Mathf.RoundToInt(background.GetComponent<RectTransform>().rect.height * 850.0f / 1136.0f);
        int row_count = LevelManager.Instance.boardRuleLogic.rowCount;
        int col_count = LevelManager.Instance.boardRuleLogic.columnCount;

        if (max_width > max_height)
        {
            // Landscape, ignore
        }
        else
        {
            // Portrait
            cardSize = Math.Min(max_width / (col_count + 1), max_height / (row_count + 1));
            cardGap = cardSize / 6;
            float target_width = cardSize * col_count + cardGap * (col_count + 1);
            float target_height = cardSize * row_count + cardGap * (row_count + 1);
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(target_width, target_height);
            gridLayoutGroup.cellSize = new Vector2(cardSize, cardSize);
            gridLayoutGroup.spacing = new Vector2(cardGap, cardGap);
            gridLayoutGroup.constraintCount = col_count;
            gridLayoutGroup.padding = new RectOffset(cardGap, cardGap, cardGap, cardGap);
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

            uiCardButtons[i] = new_card.GetComponent<UICardButton>();
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
