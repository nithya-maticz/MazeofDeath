using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpriteAnimator : MonoBehaviour
{
    public Image targetImage;           // Assign your UI Image in Inspector
    public Sprite[] frames;             // Add animation frames
    public float frameRate = 0.1f;      // Seconds per frame
    public bool loop = true;            // ✅ Toggle looping
    private int currentFrame = 0;

    void Start()
    {
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        do
        {
            for (int i = 0; i < frames.Length; i++)
            {
                targetImage.sprite = frames[i];
                yield return new WaitForSeconds(frameRate);
            }
        }
        while (loop); // ✅ loop if true
    }
}
