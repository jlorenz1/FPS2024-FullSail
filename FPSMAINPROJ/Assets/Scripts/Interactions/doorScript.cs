using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorScript : MonoBehaviour
{
    public AudioClip openingDoor;
    [Range(0, 1)] public float doorVol;

    public void slide()
    {
        StartCoroutine(slideDoor(this.transform));
    }
    IEnumerator slideDoor(Transform transform)
    {
            //get door pos
        Vector3 doorPos = transform.position;
            //get desired endPos
        Vector3 endPos = doorPos - new Vector3(0, 6, 0);
            //speed to slide
        float slidespeed = 1f;
            //time it takes
        float timeToOpen = 0f;

        AudioManager.audioInstance.playSFXAudio(openingDoor, doorVol);

        while (timeToOpen < slidespeed)
        {
            //lerp over the positions smoothly
            transform.position = Vector3.Lerp(doorPos, endPos, (timeToOpen / slidespeed));
            timeToOpen += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
    }
}
