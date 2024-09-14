using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonSounds : MonoBehaviour
{
    [SerializeFeild] public AudioClip buttonClick;
    public float clickVol = 0;

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(playSound);

    }

   void playSound()
   {
        AudioManager.audioInstance.playSFXAudio(buttonClick, clickVol);
   }
}
