using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _audioInstance;

    public AudioSource _SFXAudio;

    public AudioSource _musicAudio;

    public AudioClip _mainMenuBGMusic;
    public float _mainMenuBGVol;
    public AudioClip _gameBGMusic;
    public float _gameBGVol;
    
  
    public static AudioManager audioInstance
    {
        get
        {
            if (_audioInstance == null)
            {
                _audioInstance = FindObjectOfType<AudioManager>();
                if(_audioInstance == null)
                {
                    Debug.Log("AudioManager is null");
                    GameObject audioManagerObject = new GameObject("AudioManager");
                    _audioInstance = audioManagerObject.AddComponent<AudioManager>();
                    DontDestroyOnLoad(audioManagerObject);
                }
              
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
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (_musicAudio != _gameBGMusic && _gameBGMusic != null)
            {
                _musicAudio.volume = _gameBGVol;
            }
        }
    }

    private void Start()
    {
        if(_musicAudio != null && _mainMenuBGMusic != null)
        {
            _musicAudio.clip = _mainMenuBGMusic;
            _musicAudio.volume = _mainMenuBGVol;
            _musicAudio.Play();
        }
    }
    public void playSFXAudio(AudioClip audClip, float vol)
    {
        if(_SFXAudio !=  null && audClip != null)
        {
            _SFXAudio.PlayOneShot(audClip, vol);
        }
     
    }
}
