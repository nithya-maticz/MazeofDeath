using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator bombAnimation;
    public Animator bombAnimation1;
    public Animator bombAnimation2;
    [Header("Timing")]
    public float countdownTime = 1f;   // Time before blast
    public float blastDuration = 2f;   // How long blast lasts before fire

   
    void Start()
    {
        StartCoroutine(BombSequence());
    }

    private IEnumerator BombSequence()
    {
        // Step 1: Bomb placed, start countdown
        bombAnimation.SetTrigger("bomb");
       // yield return new WaitForSeconds(countdownTime);

        // Step 2: Blast
        bombAnimation1.SetTrigger("blast");
        yield return new WaitForSeconds(countdownTime);

        // Step 3: Fire (damage effect)
        bombAnimation2.SetTrigger("fire");
        yield return new WaitForSeconds(blastDuration);

        // Reset triggers (so it can be reused)
        bombAnimation.ResetTrigger("bomb");
        bombAnimation1.ResetTrigger("blast");
    }
}



