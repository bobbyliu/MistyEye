using mgr;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelSelection : MonoBehaviour
{
    // Maybe these could be adjustable? 
    private static readonly int ROW_COUNT = 4;
    private static readonly int COL_COUNT = 3;
    //    public UILevelSelectionButton levelSelectionButtonPrefab;
    public GameObject levelSelectionButtonPrefab;
    public GameObject levelSelectionTabButtonPrefab;
    public GameObject levelSelectionTab;

    // TODO: remove all these.
    public GameObject canvas;
    public GameObject menu;
    public GameObject infosheet;

    public UILevelSelectionButton[] uiLevelSelectionButtons;
    public UILevelSelectionTabButton[] uiLevelTabButtons;
    // Start is called before the first frame update
    void Start()
    {
        InitializeEmptyButtons();
        if (DataLoader.Instance.IsReady())
        {
            InitializeSelectionTabs();
            LoadLevelSelectionButtons();
        } else
        {
            DataLoader.Instance.onDataLoad += InitializeSelectionTabs;
            DataLoader.Instance.onDataLoad += LoadLevelSelectionButtons;
        }
    }

    private void OnDestroy()
    {
        DataLoader.Instance.onDataLoad -= LoadLevelSelectionButtons;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void InitializeEmptyButtons()
    {
        int row_count = ROW_COUNT;
        int col_count = COL_COUNT;
        System.Array.Resize(ref uiLevelSelectionButtons, row_count * col_count);
        for (int i = 0; i < row_count * col_count; i++)
        {
            // TODO: change all these to using card and size. 
            var new_card = Instantiate(levelSelectionButtonPrefab);
            new_card.transform.SetParent(this.transform, false);
            uiLevelSelectionButtons[i] = new_card.GetComponent<UILevelSelectionButton>();
        }

        return;
    }

    void InitializeSelectionTabs()
    {
        int row_count = ROW_COUNT;
        int col_count = COL_COUNT;
        int max = DataLoader.Instance.levelList.levelCount;
        int pages = (max-1) / (row_count * col_count) + 1;
        Array.Resize(ref uiLevelTabButtons, pages);

        // TODO: resize levelSelectionTab here.
        for (int i = 0; i < pages; i++)
        {
            // TODO: change all these to using card and size. 
            var new_card = Instantiate(levelSelectionTabButtonPrefab);
            new_card.transform.SetParent(levelSelectionTab.transform, false);
            uiLevelTabButtons[i] = new_card.GetComponent<UILevelSelectionTabButton>();
            uiLevelTabButtons[i].levelSelectionUI = this;
            uiLevelTabButtons[i].SetTabNumberAndEnable(i, "groundbig.png");
        }

        return;
    }

    public void LoadLevelSelectionButtons() {
        int row_count = ROW_COUNT;
        int col_count = COL_COUNT;
        Array.Resize(ref uiLevelSelectionButtons, row_count * col_count);

        int min = DataLoader.Instance.playerData.currentTab * row_count * col_count;
        int max = Math.Min(DataLoader.Instance.levelList.levelCount, DataLoader.Instance.playerData.levelProgress + 1);
        if (true) //Debug.isDebugBuild)
        {
            max = DataLoader.Instance.levelList.levelCount;
        }
        max = Math.Min(max, min + row_count * col_count);

        // 
        for (int i = min; i < max; i++)
        {
            // TODO: we only support 24 max stuff here. 
            uiLevelSelectionButtons[i - min].SetLevelAndEnable(i, DataLoader.Instance.levelList.levelInfo[i].imageName);
        }
        for (int i = max; i < min + row_count * col_count; i++)
        {
            uiLevelSelectionButtons[i - min].DisableLevel();
        }
    }

}