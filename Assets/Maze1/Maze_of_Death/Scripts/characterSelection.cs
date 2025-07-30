using UnityEngine;
using UnityEngine.UI;

public class characterSelection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Sprite maleChar;
    public Sprite femaleChar;
    public Sprite maleSelectChar;
    public Sprite femaleSelectChar;
    public Image maleImage;
    public Image femaleImage;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MaleCharSelect()
    {
        maleImage.sprite = maleSelectChar;
        femaleImage.sprite = femaleChar;
    }
    public void FemaleCharSelect()
    {
        maleImage.sprite = maleChar;
        femaleImage.sprite = femaleSelectChar;
    }
}
