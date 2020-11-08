using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UITitle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMainScene()
    {
        StartCoroutine(LoadMainSceneCoroutine());
        this.GetComponent<Button>().enabled = false;
    }
    private IEnumerator LoadMainSceneCoroutine()
    {
        float fadeTime = this.GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene("LevelSelection");
        yield break;
    }
}
