using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Title : MonoBehaviour {
    public Canvas canvas;
    public Button full_image;

	// Use this for initialization
	void Start () {
        float window_width = canvas.GetComponent<RectTransform>().rect.width;
        float window_height = canvas.GetComponent<RectTransform>().rect.height;

        float scale = window_width / (float)568;
        float target_height = (float)1136 * scale;

        full_image.GetComponent<RectTransform>().sizeDelta = new Vector2(window_width, target_height);
        if (window_width > window_height)
        {
            // Landscape
            full_image.GetComponent<RectTransform>().localPosition = new Vector2((float)0.0, (float)(-200*scale));
        } else
        {
            // Portrait
        }
    }

    public void LoadMainScene()
    {
        StartCoroutine(LoadMainSceneCoroutine());
        this.GetComponent<Button>().enabled = false;
    }

    private IEnumerator LoadMainSceneCoroutine()
    {
        Sticky.LoadTextAssetAsync("leveldata", "levellist");
        float fadeTime = this.GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene("LevelSelection");
        yield break;
    }

}
