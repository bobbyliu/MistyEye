using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using AssetBundles;

public class LevelInfo
{
    [XmlAttribute("level_name")]
    public string level_name_;
    [XmlAttribute("icon_name")]
    public string image_name_;
}

[XmlRoot("LevelList")]
public class LevelList
{
    [XmlElement("Levels")]
    public int level_count_;

    [XmlArray("LevelInfos"), XmlArrayItem("LevelInfo")]
    public List<LevelInfo> level_infos_;

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(LevelList));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static LevelList Load(TextAsset asset)
    {
        var serializer = new XmlSerializer(typeof(LevelList));
        using (var reader = new System.IO.StringReader(asset.text))
        {
            return serializer.Deserialize(reader) as LevelList;
        }
    }
}

public class LevelSelection : MonoBehaviour {
    public GameObject canvas;
    public GameObject playground;
    public GameObject level_selection_button_prefab;
    public float card_size_;
    public GameObject menu;
    public GameObject infosheet;

    public bool TotalUnlock = true;

    private List<GameObject> level_buttons = new List<GameObject>();

    // Use this for initialization
    void Start () {
        StartCoroutine(LoadLevelDataAsync());
    }

    protected IEnumerator LoadLevelDataAsync()
    {
        TextAsset temp_text_asset;
        while ((temp_text_asset = Sticky.GetTextAsset("leveldata", "levellist")) == null)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return StartCoroutine(LoadLevels(temp_text_asset));
    }

    public void ShowInfo()
    {
        infosheet.SetActive(true);
    }
    public void DisableInfo()
    {
        infosheet.SetActive(false);
    }

    public float border_size_ = 10;
    public float card_gap_ = 20;
    void InitializeBoard(int rowCount, int colCount)
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
            float target_height = 820.0f * window_height / 1136.0f;
            float target_width = 550.0f * window_height / 1136.0f;
            card_size_ = 120.0f * window_height / 1136.0f; // change it with respect to card number
            card_gap_ = 10.0f * window_height / 1136.0f;
            playground.GetComponent<RectTransform>().sizeDelta = new Vector2(target_width, target_height);

            menu.GetComponent<RectTransform>().sizeDelta = new Vector2(window_height * 640.0f / 1136.0f, 0.0f);
        }
    }

    protected void LoadSprites(LevelList level_list)
    {
        for (int i = 0; i < level_list.level_count_; i++)
        {
            string image_name = level_list.level_infos_[i].image_name_;
            Sticky.LoadSpriteAsync("carddata", image_name);
        }
    }

    protected IEnumerator RefreshLevels(int start)
    {
        int end = start + 24;
        if (end > Sticky.level_list.level_count_)
        {
            end = Sticky.level_list.level_count_;
        }

        int rowCount = 6;
        int colCount = 4;
        for (int i = start; i < end; i++)
        {
            int row = (i-start) % colCount;
            int col = rowCount - 1 - (i - start) / colCount;

            GameObject new_card = Instantiate(level_selection_button_prefab) as GameObject;
            new_card.transform.SetParent(playground.transform, false);
            new_card.GetComponent<RectTransform>().sizeDelta = new Vector2(card_size_, card_size_);
            new_card.transform.localPosition =
                new Vector3((float)((row - (colCount / 2.0 - 0.5)) * (card_size_ + card_gap_)), (float)((col - (rowCount / 2.0 - 0.5)) * (card_size_ + card_gap_)), 0);

            var new_cardcontroller = new_card.GetComponentInChildren<LevelSelectionButton>();
            new_cardcontroller.level_name_ = Sticky.level_list.level_infos_[i].level_name_;
            new_cardcontroller.image_name_ = Sticky.level_list.level_infos_[i].image_name_;
            new_cardcontroller.level_id_ = i;

            level_buttons.Add(new_card);

            if (Sticky.UnlockedLevels.Count > i || TotalUnlock)
            {
                while ((new_cardcontroller.image_ = Sticky.GetSprite("carddata", new_cardcontroller.image_name_))
                        == null)
                {
                    yield return new WaitForEndOfFrame();
                }

                new_cardcontroller.SetImage(new_cardcontroller.image_);
            }
            else
            {
                while ((new_cardcontroller.image_ = Sticky.GetSprite("carddata", "background"))
                        == null)
                {
                    yield return new WaitForEndOfFrame();
                }

                new_cardcontroller.SetLocked(new_cardcontroller.image_);
            }
        }

        yield break;
    }

    protected IEnumerator LoadLevels(TextAsset temp_text_asset)
    {
        Sticky.level_list = LevelList.Load(temp_text_asset);
        LoadSprites(Sticky.level_list);
        InitializeBoard(6, 4);

        yield return RefreshLevels(0);

        yield break;
    }

    public int current_page;
    public void SwitchPage()
    {
        for (int i = 0; i < level_buttons.Count; i++)
        {
            Destroy(level_buttons[i]);
        }
        level_buttons.Clear();

        if (current_page == 0)
        {
            StartCoroutine(RefreshLevels(24));
            current_page = 1;
        } else
        {
            StartCoroutine(RefreshLevels(0));
            current_page = 0;
        }
    }
}