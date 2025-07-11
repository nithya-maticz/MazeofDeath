using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UISpriteSheetAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public Sprite[] frames;               // Array of sprites to animate
    public float framesPerSecond = 10f;   // Animation speed
    public bool loop = true;              // Should loop

    private Image image;
    private int currentFrame = 0;
    private float timer;

    void Start()
    {
        image = GetComponent<Image>();

        if (frames != null && frames.Length > 0)
        {
            image.sprite = frames[0];
        }
    }

    void Update()
    {
        if (frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;
        float frameDuration = 1f / framesPerSecond;

        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                    currentFrame = 0;
                else
                    currentFrame = frames.Length - 1; // Stay on last frame
            }

            image.sprite = frames[currentFrame];
        }
    }
}
