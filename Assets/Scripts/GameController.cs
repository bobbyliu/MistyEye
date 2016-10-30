using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using AssetBundles;
using UnityEngine.SceneManagement;

public class CardInfo {
	[XmlAttribute("image_name")]
	public string image_name_;
	[XmlAttribute("group_id")]
	public int group_index_;
}

public class GroupInfo {
	[XmlArray("Items"),XmlArrayItem("Item")]
	public List<int> index_list_;
	[XmlAttribute("ordered")]
	public bool in_order_; // Ignored for now.

	public GroupInfo() {}

	public GroupInfo(List<int> index_list) {
		index_list_ = index_list;
	}

	public enum MatchStatus {
		NOT_MATCH,
		PARTIAL_MATCH,
		FULL_MATCH
	}

	// Assume income without the last element is a partial match. 
	public MatchStatus IncrementalMatch(List<int> income) {
		int last_element = income [income.Count - 1];
        bool is_partial_match;
        if (in_order_)
        {
            is_partial_match = (index_list_[income.Count - 1] == last_element);
        }
        else
        {
            int count = 0;
            for (int i = 0; i < index_list_.Count; i++)
            {
                if (index_list_[i] == last_element)
                {
                    count++;
                }
            }

            for (int i = 0; i < income.Count; i++)
            {
                if (income[i] == last_element)
                {
                    count--;
                }
            }
            is_partial_match = (count >= 0);
        }

		if (!is_partial_match) {
			return MatchStatus.NOT_MATCH;
		} else if (income.Count < index_list_.Count) {
			return MatchStatus.PARTIAL_MATCH;
		} else {
			return MatchStatus.FULL_MATCH;
		}
	}
}

[XmlRoot("Board")]
public class Board
{
	[XmlElement("Row")]
	public int Row;
	public int Column;

	[XmlArray("Cards"),XmlArrayItem("Card")]
	public List<CardInfo> cards_info_;

	[XmlArray("Groups"),XmlArrayItem("Group")]
	public List<GroupInfo> groups_info_;

    [XmlElement("TimeLimit")]
    public float TimeLimit;

    [XmlElement("Formula")]
    public string formula_image_name_;

    public enum Mode {
        NORMAL,
        ENDLESS1,
        ENDLESS2
    }

    [XmlElement("Mode")]
    public Mode PlayMode = Mode.NORMAL;

	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(Board));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}

	public static Board Load(string path)
	{
		var serializer = new XmlSerializer(typeof(Board));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as Board;
		}
	}

	public static Board Load(TextAsset asset) {
		var serializer = new XmlSerializer(typeof(Board));
		using(var reader = new System.IO.StringReader(asset.text))
		{
			return serializer.Deserialize(reader) as Board;
		}
	}
}

public class GameController : MonoBehaviour {
	public GameObject canvas;
	public GameObject card_prefab;
    public GameObject playground;
    public GameObject menu;
    public Text timer;
    public GameObject central_menu;
    public Text central_notify;
    public GameObject formula;
    public Text scoreboard;

    public Board board;
    private float card_size_;

    private float card_gap_ = (float)10.0;

    private List<CardController> cards_ = new List<CardController>();
	private List<CardInfo> cards_info_ = new List<CardInfo>();
	private List<GroupInfo> groups_info_;

	private List<int> previous_clicks_ = new List<int>();
	private List<int> previous_ids_ = new List<int>();
	private List<int> available_groups_ = new List<int>();

    private float timer_;
    public bool timer_on_;

    public int CardsActive = 0;

    public int endless_cycle = 0;
    public int endless_score = 0;

    void Start()
    {
        StartCoroutine(LoadLevel(Sticky.CurrentLevelName));
    }

	// Use this for initialization
	public IEnumerator LoadLevel (string level_name) {
        timer_on_ = false;
        TextAsset temp_text_asset;
        while ((temp_text_asset = Sticky.GetTextAsset("leveldata", level_name)) == null)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return StartCoroutine(LoadBoard(temp_text_asset, true));
	}

    // Use this for initialization
    public IEnumerator ContinueEndless(string level_name)
    {
        timer_on_ = false;
        TextAsset temp_text_asset;
        while ((temp_text_asset = Sticky.GetTextAsset("leveldata", level_name)) == null)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return StartCoroutine(LoadBoard(temp_text_asset, false));
    }

    protected void LoadSprites(Board board)
    {
        Sticky.LoadSpriteAsync("carddata", "background");
        for (int i = 0; i < board.Row * board.Column; i++)
        {
            Sticky.LoadSpriteAsync("carddata", board.cards_info_[i].image_name_);
        }
    }

    void UpdateScore(float t)
    {
        int add_score = (int)(((-0.0067 * t*t*t + 1.3 * t*t - 85.333 * t + 1890) / 5) + 1) * 5;
        endless_score += add_score;
        scoreboard.text = string.Format("{0:D}", endless_score);
    }

    private float max_timer_;
    protected IEnumerator LoadBoard(TextAsset temp_text_asset, bool new_game) {
		board = Board.Load(temp_text_asset);
        Sticky.LoadSpriteAsync("leveldata", board.formula_image_name_);
        groups_info_ = board.groups_info_;
        if (new_game)
        {
            endless_cycle = 0;
            endless_score = 0;
            timer_ = board.TimeLimit;
        } else
        {
            UpdateScore(max_timer_ - timer_);
            if (board.PlayMode == Board.Mode.ENDLESS1)
            {
                timer_ += (board.TimeLimit - 5 * endless_cycle > 0) ? board.TimeLimit - 5 * endless_cycle : 0;
            } else if (board.PlayMode == Board.Mode.ENDLESS2)
            {
                timer_ = (board.TimeLimit - 5 * endless_cycle > 10.0f) ? board.TimeLimit - 5 * endless_cycle : 10.0f;
            }
        }
        max_timer_ = timer_;
        LoadSprites(board);
        InitializeBoard(board.Row, board.Column);
        InitializeCardsAndGroupsInfo();
        yield return InitializeCards ();

		yield break;
	}

	public int CheckStatus (int id) {
        timer_on_ = true;
        StartCoroutine(CheckStatusCoroutine (id));
        return 1;
	}

	private void ResetPrevious () {
		previous_clicks_.Clear ();
		previous_ids_.Clear ();
		available_groups_.Clear ();
		for (int i = 0; i < groups_info_.Count; i++) {
			available_groups_.Add (i);
		}
	}

	IEnumerator CheckStatusCoroutine (int id) {
		previous_ids_.Add (id);
		previous_clicks_.Add (cards_info_[id].group_index_);
		for (int i = 0; i < available_groups_.Count; i++) {
			GroupInfo.MatchStatus status = 
				groups_info_ [available_groups_ [i]].IncrementalMatch (previous_clicks_);
			if (status == GroupInfo.MatchStatus.PARTIAL_MATCH) {
				// Do nothing
			}
			if (status == GroupInfo.MatchStatus.NOT_MATCH) {
				available_groups_.RemoveAt (i);
				i--;
			}
			if (status == GroupInfo.MatchStatus.FULL_MATCH) {
                // Found match
                yield return new WaitForSeconds(0.25f);
                groups_info_.RemoveAt (available_groups_ [i]);
				for (int j = 0; j < previous_ids_.Count; j++) {
					cards_ [previous_ids_ [j]].Fix ();
				}
                ResetPrevious();
                if (groups_info_.Count == 0)
                {
                    CardsActive--;
                    LevelComplete();
                    yield break;
                }
				break;
			}
		}

		if (available_groups_.Count == 0) {
            // No match.
			yield return new WaitForSeconds(0.25f);
			for (int i = 0; i < previous_ids_.Count; i++) {
				cards_ [previous_ids_ [i]].FlipBack ();
			}
			ResetPrevious ();
		}
        CardsActive--;
    }

    void LevelComplete()
    {
        GameClear();
        endless_cycle++;
        if (board.PlayMode != Board.Mode.NORMAL)
        {
            StartCoroutine(ContinueEndless(Sticky.CurrentLevelName));
        }
        else
        {
            if (Sticky.CurrentLevel >= Sticky.UnlockedLevels.Count - 1)
            {
                Sticky.UnlockedLevels.Add(Sticky.CurrentLevel + 1);
                Sticky.Save();
            }
            ShowMenu("Congratulations!", CentralMenu.State.SUCCESS);
        }
    }

    void GameClear()
    {
        timer_on_ = false;
        for (int i = 0; i < cards_.Count; i++)
        {
            cards_[i].Delete();
        }
        cards_.Clear();
        cards_info_.Clear();
        groups_info_.Clear();

        previous_clicks_.Clear();
        previous_ids_.Clear();
        available_groups_.Clear();
    }

    public IEnumerator GameComplete()
    {
        GameClear();
        SceneManager.LoadScene("LevelSelection");
        yield break;
    }

    public IEnumerator GameReset()
    {
        GameClear();
        StartCoroutine(LoadLevel(Sticky.CurrentLevelName));
        yield break;
    }

    public IEnumerator GameNext()
    {
        GameClear();
        Sticky.CurrentLevel++;
        Sticky.CurrentLevelName = Sticky.level_list.level_infos_[Sticky.CurrentLevel].level_name_;
        StartCoroutine(LoadLevel(Sticky.CurrentLevelName));
        yield break;
    }

    void InitializeCardsAndGroupsInfo() {
		int total = board.Row * board.Column;
		int[] unrandomize = new int[total];

		for (int i = 0; i < total; i++) {
			unrandomize[i] = i;
		}
		for (int i = 0; i < total; i++) {
			int temp = UnityEngine.Random.Range (i, total);
			int valuetemp;
			valuetemp = unrandomize [temp];
			unrandomize [temp] = unrandomize [i];
			unrandomize [i] = valuetemp;
		}
		for (int i = 0; i < total; i++) {
			cards_info_.Add(board.cards_info_[unrandomize[i]]);
		}

		ResetPrevious ();
	}

    void InitializeBoard(int rowCount, int colCount)
    {
        float window_width = canvas.GetComponent<RectTransform>().rect.width;
        float window_height = canvas.GetComponent<RectTransform>().rect.height;

        Debug.Log("width=" + window_width);
        Debug.Log("window_height=" + window_height);

        if (window_width > window_height)
        {
            // Landscape, ignore
        }
        else
        {
            // Portrait
            float target_width = 550.0f * window_height / 1136.0f;
            //            card_size_ = 80.0f * window_height / 1136.0f; // change it with respect to card number
            card_size_ = target_width / (colCount * 90.0f + 10.0f) * (80.0f);
            card_gap_ = card_size_ / 8.0f;
            float target_height = target_width / (colCount * 90.0f + 10.0f) * (rowCount * 90.0f + 10.0f);
            playground.GetComponent<RectTransform>().sizeDelta = new Vector2(target_width, target_height);
            float local_y = -85.0f;
            if (rowCount == colCount)
            {
                local_y = -27.0f;
            }
            playground.GetComponent<RectTransform>().localPosition = new Vector2(0, local_y / 1136.0f * window_height);

            menu.GetComponent<RectTransform>().sizeDelta = new Vector2(window_height * 640.0f / 1136.0f, 0.0f);
            timer.fontSize = (int)(36.0f * window_height / 1136.0f);

            central_menu.GetComponent<RectTransform>().sizeDelta = new Vector2(window_height * 570.0f / 1136.0f, window_height * 360.0f / 1136.0f);
            central_notify.fontSize = (int)(36.0f * window_height / 1136.0f);
        }
        CardsActive = 0;
    }

    IEnumerator InitializeCards() {
        Sprite card_back;
        while ((card_back = Sticky.GetSprite("carddata", "background")) == null)
        {
            yield return new WaitForEndOfFrame();
        }
        Sprite card_blank;
        while ((card_blank = Sticky.GetSprite("carddata", "ground")) == null)
        {
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < board.Column; i++) {
			for (int j = 0; j < board.Row; j++) {
				int index = i * board.Row + j;
				GameObject new_card = Instantiate (card_prefab) as GameObject;
				new_card.transform.SetParent (playground.transform, false);
                new_card.GetComponent<RectTransform>().sizeDelta = new Vector2(card_size_, card_size_);
				new_card.transform.localPosition = 
					new Vector3 ((float)((i-(board.Column / 2.0-0.5))* (card_size_ + card_gap_)), (float)((j-(board.Row / 2.0-0.5))* (card_size_ + card_gap_)), 0);

                var new_cardcontroller = new_card.GetComponentInChildren<CardController>();
                new_cardcontroller.image_name_ = cards_info_[index].image_name_;
                new_cardcontroller.master = this;
                new_cardcontroller.id_ = index;
                new_cardcontroller.this_button_ = new_card;

                while ((new_cardcontroller.face_ = Sticky.GetSprite("carddata", new_cardcontroller.image_name_))
                        == null)
                {
                    yield return new WaitForEndOfFrame();
                }
                new_cardcontroller.back_ = card_back;
                new_cardcontroller.blank_ = card_blank;

                new_card.GetComponentInChildren<CardController> ().SetImage(card_back);
				cards_.Add (new_card.GetComponentInChildren<CardController> ());
			}
		}
        Sprite formula_image;
        while ((formula_image = Sticky.GetSprite("leveldata", board.formula_image_name_)) == null)
        {
            yield return new WaitForEndOfFrame();
        }
        formula.GetComponentInChildren<Image>().sprite = formula_image;
    }

    void SetTimer()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timer_);
        timer.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    void ShowMenu(string notice_text, CentralMenu.State state)
    {
        timer_on_ = false;
        CardsActive++;
        central_menu.GetComponentInChildren<CentralMenu>().Show(
            notice_text, state);
        central_menu.SetActive(true);
    }

    public void Pause()
    {
        Debug.Log("Paused!");
        ShowMenu("Paused!", CentralMenu.State.PAUSE);
    }

    void Update()
    {
        if (timer_on_)
        {
            timer_ -= Time.deltaTime;
            if (timer_ <= 0)
            {
                timer_ = 0.0f;
                ShowMenu("Time out!", CentralMenu.State.FAIL);
            }
        }
        SetTimer();
    }
}
