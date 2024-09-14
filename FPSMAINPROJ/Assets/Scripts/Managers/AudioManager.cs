using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _audioInstance;

    public AudioSource _SFXAudio;
  
    public static AudioManager audioInstance
    {
        get
        {
            if (_audioInstance == null)
            {
                Debug.LogError("AudioManager is null");
            }
            return _audioInstance;
        }
    }

    private void Awake()
    {
        if (_audioInstance != null && _audioInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _audioInstance = this;
        }

        
    }


    public void playSFXAudio(AudioClip audClip, float vol)
    {
        _SFXAudio.PlayOneShot(audClip, vol);
    }
}
