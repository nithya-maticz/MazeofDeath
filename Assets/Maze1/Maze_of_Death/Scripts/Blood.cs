using System.Collections;
using UnityEngine;

public class Blood : MonoBehaviour
{
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float duration = 20f;
        float elapsed = 0f;

        Color originalColor = sprite.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0.75f, elapsed / duration); // Fade from 1 to 0.5
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure final alpha is exactly 0.5
        sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.75f);

        // No Destroy here
    }

}
