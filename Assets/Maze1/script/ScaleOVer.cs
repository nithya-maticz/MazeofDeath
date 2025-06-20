using UnityEngine;

public class ScaleOVer : MonoBehaviour
{
    public float scaleSpd;
    public float minSclae, maxScale;
    Vector2 scale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        scale = new Vector2(Mathf.Clamp(scale.x += scaleSpd * Time.deltaTime, minSclae, maxScale), scale.y);
        scale = new Vector2(scale.x, Mathf.Clamp(scale.y += scaleSpd * Time.deltaTime, minSclae, maxScale));

        transform.localScale = scale;
        print(scale);
       
    }
}
