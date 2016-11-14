using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {
    public AudioSource SEPlayer;
    public AudioClip Opening;
    public AudioClip Slide;
    public AudioClip Click;
    public AudioClip Lost;
    public AudioClip Pop;
    public AudioClip Tap;
    public AudioClip Disappear;

	public void PlayDisappear () {
        SEPlayer.PlayOneShot(Disappear, 0.5f);
    }
	
	public void PlaySlide () {
        SEPlayer.PlayOneShot(Slide);
    }

    public void PlayPop()
    {
        SEPlayer.PlayOneShot(Pop);
    }

    public void PlayTap()
    {
        SEPlayer.PlayOneShot(Tap, 0.5f);
    }

    public void PlayLost()
    {
        SEPlayer.PlayOneShot(Lost);
    }

    public void PlayClick()
    {
        SEPlayer.PlayOneShot(Click, 0.5f);
    }
}
