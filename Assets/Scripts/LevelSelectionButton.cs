using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour {
    public string level_name_;
    public string image_name_;
    public Sprite locked_image_;
    public Sprite image_;
    public int level_id_;
    public bool is_locked_;

    public void OnClick () {
        Sticky.CurrentLevel = level_id_;
        Sticky.CurrentLevelName = level_name_;
        SceneManager.LoadScene("Main");
    }

    public void SetImage(Sprite sprite)
    {
        Image button_image;
        button_image = this.GetComponentInChildren<Image>();
        button_image.sprite = sprite;
    }

    public void SetLocked(Sprite sprite)
    {
        SetImage(sprite);

        this.GetComponentInParent<Button>().interactable = false;
    }

}
