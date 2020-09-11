using mgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoardMenu : MonoBehaviour
{
    // TODO: this is unlike the others, maybe could be derived?
    public GameObject background;

    [SerializeField] [Tooltip("公式图标")] private Image formulaImage;
    [SerializeField] [Tooltip("计时器/计步器")] private Text timerText;
    [SerializeField] [Tooltip("撤销")] private Button revertButton;
    [SerializeField] [Tooltip("计分器")] private Text scorerText;
    [SerializeField] [Tooltip("退出")] private Button exitButton;
    // Start is called before the first frame update
    void Start()
    {
        exitButton.onClick.AddListener(LoadPauseMenu);
        revertButton.onClick.AddListener(UndoRemove);
    }

    public void Update()
    {
        timerText.text = LevelManager.Instance.GetCurrentTimer();
    }

    // Update is called once per frame
    public void SetFormulaImage(Sprite formula_image)
    {
        // TODO: change to set/get format?
        formulaImage.sprite = formula_image;
    }

    public void SetTimerText(string timer_text)
    {
        // TODO: change to set/get format?
        timerText.text = timer_text;
    }
    public void SetScorerText(string scorer_text)
    {
        // TODO: change to set/get format?
        scorerText.text = scorer_text;
    }

    void LoadPauseMenu()
    {
        MenuManager.Instance.CreateMenu(background, "pause_menu");
        MenuManager.Instance.currentMenu.GetComponent<UIPauseMenu>().SetState(UIPauseMenu.State.PAUSE);
    }

    void UndoRemove()
    {
        LevelManager.Instance.UndoRemove();
    }
}
