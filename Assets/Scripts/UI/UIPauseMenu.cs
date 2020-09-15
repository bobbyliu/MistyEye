﻿using mgr;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class UIPauseMenu : MonoBehaviour
{
    // Optional buttons
    public GameObject NextLevel;
    public GameObject Continue;

    // Optional notice
    public GameObject Congratulations;
    public GameObject Paused;
    public GameObject Timeout;

    public enum State
    {
        SUCCESS,
        PAUSE,
        FAIL
    }

    public void SetState(State format)
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
        if (LevelManager.Instance.levelId == DataLoader.Instance.levelList.levelCount - 1)
        {
            NextLevel.SetActive(false);
        }
    }

    // Use this for initialization
    public void OnClickHome()
    {
        //        master.CardsActive++;
        //        master.central_menu.SetActive(false);
        LevelManager.Instance.ClearLevel();
        MenuManager.Instance.DestroyMenu();
        SceneManager.LoadScene("LevelSelection");
    }

    public void OnClickReplay()
    {
        int current_level = LevelManager.Instance.levelId;
        LevelManager.Instance.ClearLevel();
        LevelManager.Instance.SetLevel(current_level);
        LevelManager.Instance.LoadLevel();
        MenuManager.Instance.DestroyMenu();

        //        master.CardsActive--;
        //        StartCoroutine(master.GameReset());
        //        master.central_menu.SetActive(false);
    }

    public void OnClickNext()
    {
        int current_level = LevelManager.Instance.levelId;
        LevelManager.Instance.ClearLevel();
        LevelManager.Instance.SetLevel(current_level + 1);
        LevelManager.Instance.LoadLevel();
        MenuManager.Instance.DestroyMenu();
        //        master.CardsActive--;
        //        StartCoroutine(master.GameNext());
        //        master.central_menu.SetActive(false);
    }

    public void OnClickResume()
    {
        MenuManager.Instance.DestroyMenu();
//        master.CardsActive--;
//        master.timer_on_ = true;
//        master.central_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}