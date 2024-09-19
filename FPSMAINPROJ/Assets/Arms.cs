using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arms : MonoBehaviour
{
    [SerializeField] Animator animator;
    PlayerController player;
    int lastGun;

    private void Start()
    {
        player = gameManager.gameInstance.playerScript;
        lastGun = 0;
    }

    public void NoGunMovement()
    {
        animator.SetFloat("Movement", Mathf.Lerp(animator.GetFloat("Movement"), player.move.normalized.magnitude, Time.deltaTime * 5));

    }
    public void StartAnimationSlide()
    {
        animator.SetFloat("Movement", 0);
        animator.SetBool("Slide", true);

    }

    public void StopAnimationSlide()
    {
        animator.SetBool("Slide", false);
    }

    public void ChangeGun(int aniLayer)
    {
        if (lastGun != 0)
            animator.SetLayerWeight(lastGun, 0);

        animator.SetLayerWeight(aniLayer, 1);

        lastGun = aniLayer;

    }

}
