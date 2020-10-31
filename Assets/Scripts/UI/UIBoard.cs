﻿using mgr;
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

    public GameObject materialBoard;
    public GridLayoutGroup materialLayoutGroup;
    public GameObject targetBoard;
    public GridLayoutGroup targetLayoutGroup;
    [SerializeField] [Tooltip("按键区域")] private GameObject textBoard;

    [SerializeField] [Tooltip("按键记录")] private TMPro.TextMeshProUGUI partialText;
    public Image greenLight;
    public Image redLight;

    [SerializeField] [Tooltip("按键记录")] private TMPro.TextMeshProUGUI revealCountdownTimer;

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
        LevelManager.Instance.onUpdatePartialText -= UpdatePartialText;
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
        LevelManager.Instance.onUpdatePartialText += UpdatePartialText;

        Debug.Log("Loading rule image:" + LEVEL_RULE_ASSET_PREFIX + LevelManager.Instance.levelData.ruleImageName);
        Addressables.LoadAssetAsync<Sprite>(LEVEL_RULE_ASSET_PREFIX + LevelManager.Instance.levelData.ruleImageName).Completed +=
            (AsyncOperationHandle<Sprite> handle) => {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    boardMenu.SetFormulaImage(handle.Result);
                }
            };

        StartCoroutine(RevealCountdown());
    }

    private IEnumerator RevealCountdown()
    {
        LevelManager.Instance.ResetTimer();
        foreach (var card in uiCardButtons)
        {
            card.cardStatus = UICardButton.CardStatus.SELECTED;
        }
        revealCountdownTimer.gameObject.SetActive(true);
        revealCountdownTimer.text = "3";
        yield return new WaitForSecondsRealtime(1.0f);
        revealCountdownTimer.text = "2";
        yield return new WaitForSecondsRealtime(1.0f);
        revealCountdownTimer.text = "1";
        yield return new WaitForSecondsRealtime(1.0f);
        revealCountdownTimer.gameObject.SetActive(false);
        foreach (var card in uiCardButtons)
        {
            card.cardStatus = UICardButton.CardStatus.NORMAL;
        }
        LevelManager.Instance.StartTimer();
        yield return 0;
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
        int material_count = LevelManager.Instance.boardRuleLogic.materialCount;
        int col_count = LevelManager.Instance.boardRuleLogic.columnCount;
        int row_count = (material_count - 1) / col_count + 1;
        int target_count = LevelManager.Instance.boardRuleLogic.targetCount;

        int target_row = (target_count - 1) / col_count + 1;

        if (max_width > max_height)
        {
            // Landscape, I don't think we'd ever support these. 
        }
        else
        {
            // Portrait
            if (target_count == 0)
            {
                targetLayoutGroup.gameObject.SetActive(false);
                cardSize = Math.Min(max_width / (col_count + 1), max_height / (row_count + 2));
                cardGap = cardSize / 6;
                float board_width = cardSize * col_count + cardGap * (col_count + 1);
                float board_height = cardSize * (row_count + 1) + cardGap * (row_count + 3);
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(board_width, board_height);
                materialLayoutGroup.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(
                    0,
                    -(cardSize * (row_count + 1) + cardGap * (row_count + 4)));
                materialLayoutGroup.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(
                    0,
                    -(cardSize * 1 + cardGap * 3));
                materialLayoutGroup.cellSize = new Vector2(cardSize, cardSize);
                materialLayoutGroup.spacing = new Vector2(cardGap, cardGap);
                materialLayoutGroup.constraintCount = col_count;
                materialLayoutGroup.padding = new RectOffset(cardGap, cardGap, cardGap, cardGap);

                textBoard.GetComponent<RectTransform>().offsetMin = new Vector2(
                    0,
                    -(cardSize * 1 + cardGap * 2));
                partialText.fontSize = cardSize;
            }
            else {
                targetLayoutGroup.gameObject.SetActive(true);

                cardSize = Math.Min(max_width / (col_count + 1), max_height / (row_count + target_row + 3));
                cardGap = cardSize / 6;
                float board_width = cardSize * col_count + cardGap * (col_count + 1);
                float board_height = cardSize * (row_count + target_row + 1) + cardGap * (row_count + target_row + 6);
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(board_width, board_height);
                materialLayoutGroup.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(
                    0,
                    -(cardSize * (row_count + 1) + cardGap * (row_count + 4)));
                materialLayoutGroup.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(
                    0,
                    -(cardSize * 1 + cardGap * 3));
                materialLayoutGroup.cellSize = new Vector2(cardSize, cardSize);
                materialLayoutGroup.spacing = new Vector2(cardGap, cardGap);
                materialLayoutGroup.constraintCount = col_count;
                materialLayoutGroup.padding = new RectOffset(cardGap, cardGap, cardGap, cardGap);

                textBoard.GetComponent<RectTransform>().offsetMin = new Vector2(
                    0,
                    -(cardSize * 1 + cardGap * 2));
                partialText.fontSize = cardSize;

                targetLayoutGroup.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(
                    0,
                    0);
                targetLayoutGroup.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(
                    0,
                    cardSize * (target_row) + cardGap * (target_row + 1));
                targetLayoutGroup.cellSize = new Vector2(cardSize, cardSize);
                targetLayoutGroup.spacing = new Vector2(cardGap, cardGap);
                targetLayoutGroup.constraintCount = col_count;
                targetLayoutGroup.padding = new RectOffset(cardGap, cardGap, cardGap, cardGap);
            }
        }
    }
    
    void InitializeEmptyButtons()
    {
        int material_count = LevelManager.Instance.boardRuleLogic.materialCount;
        int col_count = LevelManager.Instance.boardRuleLogic.columnCount;
        int target_count = LevelManager.Instance.boardRuleLogic.targetCount;
        System.Array.Resize(ref uiCardButtons, material_count + target_count);
        Debug.Log("InitializeEmptyButtons." + "material_count:" + material_count
            + "col_count:" + col_count
            + "target_count:" + target_count);
        for (int i = 0; i < material_count; i++)
        {
            // TODO: change all these to using card and size. 
            var new_card = Instantiate(cardButtonPrefab);
            // TODO: ???
            new_card.transform.SetParent(materialLayoutGroup.transform, false);
            new_card.GetComponent<RectTransform>().sizeDelta = new Vector2(cardSize, cardSize);

            uiCardButtons[i] = new_card.GetComponent<UICardButton>();
        }
        for (int i = material_count; i < material_count + target_count; i++)
        {
            // TODO: change all these to using card and size. 
            var new_card = Instantiate(cardButtonPrefab);
            // TODO: ???
            new_card.transform.SetParent(targetLayoutGroup.transform, false);
            new_card.GetComponent<RectTransform>().sizeDelta = new Vector2(cardSize, cardSize);

            uiCardButtons[i] = new_card.GetComponent<UICardButton>();
        }
        return;
    }

    void LoadCardButtons()
    {
        int material_count = LevelManager.Instance.boardRuleLogic.materialCount;
        int col_count = LevelManager.Instance.boardRuleLogic.columnCount;
        int target_count = LevelManager.Instance.boardRuleLogic.targetCount;
        Debug.Log("LoadCardButtons." + "material_count:" + material_count + "col_count:" + col_count);
        for (int i = 0; i < material_count + target_count; i++)
        {
            // TODO: we only support 24 max stuff here. 
            uiCardButtons[i].SetCardAndEnable(i);
        }
    }

    void UpdatePartialText(string text, logic.BoardRuleLogicBase.JudgeState state)
    {
        partialText.text = text;
        if (state == logic.BoardRuleLogicBase.JudgeState.INVALID)
        {
            greenLight.gameObject.SetActive(false);
            redLight.gameObject.SetActive(true);
        }
        else if (state == logic.BoardRuleLogicBase.JudgeState.VALID)
        {
            greenLight.gameObject.SetActive(true);
            redLight.gameObject.SetActive(false);
        }
        else
        {
            greenLight.gameObject.SetActive(false);
            redLight.gameObject.SetActive(false);
        }

    }

    void LoadFinishMenu()
    {
        MenuManager.Instance.CreateMenu(background, "pause_menu");
        MenuManager.Instance.currentMenu.GetComponent<UIPauseMenu>().SetState(UIPauseMenu.State.SUCCESS);
    }
}
