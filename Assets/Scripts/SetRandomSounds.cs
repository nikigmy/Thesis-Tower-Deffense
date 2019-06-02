using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomSounds : MonoBehaviour {

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    List<AudioClip> sounds;
    [SerializeField]
    bool playOnStart = true;


    bool shouldBePlaying;

	// Use this for initialization
	void Start () {
        shouldBePlaying = false;
        if(audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.Stop();
        }
        if (playOnStart)
        {
            PlayRandomSound();
        }
	}

    private void PlayRandomSound()
    {
        var sound = sounds[UnityEngine.Random.Range(0, sounds.Count)];
        if(audioSource != null)
        {
            audioSource.clip = sound;
            audioSource.Play();
            Debug.Log(audioSource.outputAudioMixerGroup.name);
            shouldBePlaying = true;
        }
    }

    public void Play()
    {
        PlayRandomSound();
    }

    public void Stop()
    {
        if(audioSource != null)
        {
            audioSource.Stop();
            shouldBePlaying = false;
        }
    }

    // Update is called once per frame
    void Update () {
        if(shouldBePlaying && audioSource != null && !audioSource.isPlaying)
        {
            PlayRandomSound();
        }
	}
}
