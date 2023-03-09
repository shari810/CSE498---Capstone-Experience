using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayUISound : MonoBehaviour
{
    private AudioSource clickSound;
    private AudioSource spec2Sound;
    private AudioSource spec3Sound;
    private AudioSource spec5Sound;

    // Start is called before the first frame update
    void Start()
    {
        clickSound = GameObject.Find("SoundBank/UI/Click").GetComponent<AudioSource>();
        spec2Sound = GameObject.Find("SoundBank/UI/Special2").GetComponent<AudioSource>();
        spec3Sound = GameObject.Find("SoundBank/UI/Special3").GetComponent<AudioSource>();
        spec5Sound = GameObject.Find("SoundBank/UI/Special5").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void PlayClick()
    {
        clickSound.ignoreListenerPause = true;
        clickSound.Play();
    }
    public void PlaySpecial2()
    {
        spec2Sound.ignoreListenerPause = true;
        spec2Sound.Play();
    }
    public void PlaySpecial3()
    {
        spec3Sound.ignoreListenerPause = true;
        spec3Sound.Play();
    }

    public void PlaySpecial5()
    {
        spec5Sound.ignoreListenerPause = true;
        spec5Sound.Play();
    }
}
