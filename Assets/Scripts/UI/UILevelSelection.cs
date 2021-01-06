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
    public GameObject menu;
    public GameObject infosheet;

    private bool needInitialize;

    public UILevelSelectionButton[] uiLevelSelectionButtons;
    // Start is called before the first frame update
    void Start()
    {
        InitializeEmptyButtons();
        if (DataLoader.Instance.IsReady())
        {
            LoadLevelSelectionButtons();
        } else
        {
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
            int row = i % col_count;
            int col = row_count - 1 - i / col_count;

            // TODO: change all these to using card and size. 
            var new_card = Instantiate(levelSelectionButtonPrefab);
            new_card.transform.SetParent(this.transform, false);
            uiLevelSelectionButtons[i] = new_card.GetComponent<UILevelSelectionButton>();
        }

        return;
    }

    void LoadLevelSelectionButtons() {
        int row_count = ROW_COUNT;
        int col_count = COL_COUNT;
        Array.Resize(ref uiLevelSelectionButtons, row_count * col_count);

        int min = 0;
        int max = Math.Min(DataLoader.Instance.levelList.levelCount, DataLoader.Instance.playerData.levelProgress + 1);
        if (true) //Debug.isDebugBuild)
        {
            max = DataLoader.Instance.levelList.levelCount;
        }
        for (int i = min;  i < Math.Min(max, row_count * col_count); i++)
        {
            // TODO: we only support 24 max stuff here. 
            uiLevelSelectionButtons[i].SetLevelAndEnable(i, DataLoader.Instance.levelList.levelInfo[i].imageName);
        }
    }

}