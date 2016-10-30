using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour {
    public Texture2D FadeOutTexture;
    public float FadeSpeed = 0.1f;

    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDir = -1;

    void OnGUI ()
    {
        alpha += fadeDir * FadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), FadeOutTexture);
    }

    public float BeginFade (int direction)
    {
        fadeDir = direction;
        return (1.0f / FadeSpeed);
    }

    void OnLevelWasLoaded()
    {
        BeginFade(-1);
    }
}
