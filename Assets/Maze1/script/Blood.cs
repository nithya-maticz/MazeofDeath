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
        float duration = 10f;
        float elapsed = 0f;

        Color originalColor = sprite.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure alpha is set to 0 exactly
        sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Destroy the GameObject
        Destroy(gameObject);
    }
}
