using UnityEngine;

public class KeyFrameFunctions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FadeOff()
    {
        Game_Manager.Instance.Fader.gameObject.SetActive(false);
    }
}
