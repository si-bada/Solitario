using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour 
{
    #region Static
    public static AudioManager Instance;
    #endregion

    #region Script Parameters
    public Sound[] Sounds;
    #endregion

    #region Fields
    private List<float> audioVolumes = new List<float>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else Destroy(this);

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.loop = s.Loop;
            s.Source.playOnAwake = s.PlayOnAwake;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
        }
    }

    private void Start()
    {
        Play("MusicBackground");

        DontDestroyOnLoad(this);

        RegisterStartingAudioVolumes();
    }
    #endregion

    #region Methods
    public void Play(string name)
    {
        Sound MySound = Array.Find(Sounds, sound => sound.Name == name);

        if (MySound == null)
        {
            return;
        }

        else
        {
            if (MySound.Source.isPlaying)
            {
                return;
            }

            MySound.Source.Play();
        }
    }

    public void PlayOneShot(string name)
    {
        Sound mySound = Array.Find(Sounds, sound => sound.Name == name);

        if (mySound == null)
        {
            return;
        }

        if (mySound.Source.isPlaying)
        {
            mySound.Source.Stop();
        }

        mySound.Source.PlayOneShot(mySound.Clip, 1f);
    }

    public void MuteSFX(bool v)
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            if (Sounds[i].Name.Contains("SFX"))
            {
                if (v == false)
                {
                    Sounds[i].Source.volume = 0;
                }
                else
                {
                    Sounds[i].Source.volume = audioVolumes[i];
                }
            }
        }
        PlayerPrefs.SetInt("SFX", v == true ? 0 : 1);
    }

    public void MuteMusic(bool v)
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            if (!Sounds[i].Name.Contains("SFX"))
            {
                if(v == false)
                {
                    Sounds[i].Source.volume = 0;
                }
                else
                {
                    Sounds[i].Source.volume = audioVolumes[i];
                }
            }
        }
        PlayerPrefs.SetInt("Music", v == true ? 0 : 1);
    }
    #endregion

    #region Implementations
    private void RegisterStartingAudioVolumes()
    {
        for (int i = 0; i < Sounds.Length; i++)
        {
            Sound s = Sounds[i];
            audioVolumes.Add(s.Source.volume);
        }
    }
    #endregion
}
