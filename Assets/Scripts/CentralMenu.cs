using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CentralMenu : MonoBehaviour {
    public GameController master;
    public Text Notice;

    // Three versions
    public GameObject NextLevel;
    public GameObject Continue;

    public GameObject Congratulations;
    public GameObject Timeout;
    public GameObject Paused;

    public enum State
    {
        SUCCESS,
        PAUSE,
        FAIL
    }

    public void Show(string notice_text, State format)
    {
//        Notice.text = notice_text;
        NextLevel.SetActive(false);
        Continue.SetActive(false);
        Congratulations.SetActive(false);
        Timeout.SetActive(false);
        Paused.SetActive(false);
        if (format == State.SUCCESS)
        {
            Congratulations.SetActive(true);
            NextLevel.SetActive(true);
        }
        else if (format == State.PAUSE)
        {
            Paused.SetActive(true);
            Continue.SetActive(true);
        }
        else if (format == State.FAIL)
        {
            Timeout.SetActive(true);
        }
        // Last level. Disable the "Next level".
        if (Sticky.CurrentLevel == Sticky.level_list.level_count_ - 1)
        {
            NextLevel.SetActive(false);
        }
    }

    // Use this for initialization
    public void OnClickHome () {
        master.CardsActive++;
        master.central_menu.SetActive(false);
        SceneManager.LoadScene("LevelSelection");
    }

    public void OnClickReplay()
    {
        master.CardsActive--;
        StartCoroutine(master.GameReset());
        master.central_menu.SetActive(false);
    }

    public void OnClickNext()
    {
        master.CardsActive--;
        StartCoroutine(master.GameNext());
        master.central_menu.SetActive(false);
    }

    public void OnClickResume()
    {
        master.CardsActive--;
        master.timer_on_ = true;
        master.central_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
