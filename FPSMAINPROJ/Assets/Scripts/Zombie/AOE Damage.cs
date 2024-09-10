using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDamage : MonoBehaviour
{
  [SerializeField]  bool isSlow;
    float Duration;
    float StartSpeed;

    // Start is called before the first frame update
    void Start()
    {
        StartSpeed = gameManager.gameInstance.playerScript.GetSpeed();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (isSlow && other.CompareTag("Player"))
        {
            StartCoroutine(slow());
            if (gameManager.gameInstance.isUserKareDead == false)
            {
                gameManager.gameInstance.Userkare.LightGautling();
            }
        }
        else
            return;
    }


    IEnumerator slow()
    {

        gameManager.gameInstance.playerScript.SetSpeed(StartSpeed / 2);
       yield return new WaitForSeconds(5);

        gameManager.gameInstance.playerScript.SetSpeed(StartSpeed);


    }

    public void SetDuration(float duration)
    {
        Duration = duration;
    }
}
