using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UITitle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AssetPreLoad.PreloadAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainScene()
    {
        if (SplashScreen.isFinished)
        {
            StartCoroutine(LoadMainSceneCoroutine());
            this.GetComponent<Button>().interactable = false;
        }
    }
    private IEnumerator LoadMainSceneCoroutine()
    {
        float fadeTime = this.GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene("LevelSelection");
        yield break;
    }
}
