﻿using mgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInfoMenu : MonoBehaviour
{
    // Use this for initialization
    public void OnClick()
    {
        MenuManager.Instance.DestroyMenu();
    }
}
