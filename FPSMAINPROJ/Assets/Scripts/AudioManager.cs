using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _audioInstance;
    [SerializeField] public AudioSource _audioSource;
    [SerializeFeild] AudioClip background;
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

    public void playAudio(AudioClip audClip, float vol)
    {
        _audioSource.PlayOneShot(audClip, vol);
    }
}
