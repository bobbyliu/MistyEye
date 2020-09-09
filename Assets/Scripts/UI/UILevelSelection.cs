using mgr;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelSelection : MonoBehaviour
{
    // Maybe these could be adjustable? 
    private static readonly int ROW_COUNT = 6;
    private static readonly int COL_COUNT = 4;
    //    public UILevelSelectionButton levelSelectionButtonPrefab;
    public GameObject levelSelectionButtonPrefab;

    // TODO: remove all these.
    public GameObject canvas;
    private float cardSize;
    private float borderSize;
    private float cardGap;
    public GameObject menu;
    public GameObject infosheet;

    private bool needInitialize;

    public UILevelSelectionButton[] uiLevelSelectionButtons;
    // Start is called before the first frame update
    void Start()
    {
        InitializeCanvas();
        InitializeEmptyButtons();
        LoadLevelSelectionButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (needInitialize && DataLoader.Instance.IsReady())
        {
            LoadLevelSelectionButtons();
            needInitialize = false;
        }
    }

    // not sure if this should happen here.
    void InitializeCanvas()
    {
        float window_width = canvas.GetComponent<RectTransform>().rect.width;
        float window_height = canvas.GetComponent<RectTransform>().rect.height;

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
        int row_count = ROW_COUNT;
        int col_count = COL_COUNT;
        System.Array.Resize(ref uiLevelSelectionButtons, row_count * col_count);
        for (int i = 0; i < row_count * col_count; i++)
        {
            int row = i % col_count;
            int col = row_count - 1 - i / col_count;

            // TODO: change all these to using card and size. 
            var new_card = Instantiate(levelSelectionButtonPrefab);
            new_card.transform.SetParent(this.transform, false);
            new_card.GetComponent<RectTransform>().sizeDelta = new Vector2(cardSize, cardSize);
            new_card.transform.localPosition =
                new Vector3((float)((row - (col_count / 2.0 - 0.5)) * (cardSize + cardGap)), (float)((col - (row_count / 2.0 - 0.5)) * (cardSize + cardGap)), 0);

            uiLevelSelectionButtons[i] = new_card.GetComponent<UILevelSelectionButton>();

            /*            new_cardcontroller.level_id_ = i;*/
        }

        return;
    }

    void LoadLevelSelectionButtons() {
        int row_count = ROW_COUNT;
        int col_count = COL_COUNT;
        System.Array.Resize(ref uiLevelSelectionButtons, row_count * col_count);
        if (!DataLoader.Instance.IsReady())
        {
            needInitialize = true;
            return;
        }
        for (int i = 0; i < Math.Min(DataLoader.Instance.levelList.levelCount, row_count * col_count); i++)
        {
            // TODO: we only support 24 max stuff here. 
            uiLevelSelectionButtons[i].SetLevelAndEnable(i, DataLoader.Instance.levelList.levelInfo[i].imageName);
        }
    }

}