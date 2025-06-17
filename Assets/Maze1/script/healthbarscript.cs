using UnityEngine;
using UnityEngine.UI;

public class healthbarscript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider slider;
     
    public void UpdateHealthBar( float currentval, float maxval)
    {
        Debug.Log("update healthbar");
        slider.value = currentval / maxval;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
