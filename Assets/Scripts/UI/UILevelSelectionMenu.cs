using mgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelSelectionMenu : MonoBehaviour
{
    public GameObject background;
    [SerializeField] [Tooltip("制作人按钮")] private Button infoButton;

    // Start is called before the first frame update
    void Start()
    {
        infoButton.onClick.AddListener(LoadInfoMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LoadInfoMenu()
    {
        MenuManager.Instance.CreateMenu(background, "info_menu");
    }
}
