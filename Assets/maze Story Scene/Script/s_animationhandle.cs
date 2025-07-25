using UnityEngine;

public class s_animationhandle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void knifefun()
    {

        StoryManager.Instance.FirstBox();

    }
    public void keyImage()
    {

        StoryManager.Instance.SecondBox();
    }
    
    public void knifewalk()
    {
        StoryPlayer.Instance.knifeWalkStart = false;
    }
    public void walk()
    {
        StoryPlayer.Instance.walkStart = false;
    }
    public void attack()
    {
        StoryPlayer.Instance.attackImage.SetActive(false);
        StoryPlayer.Instance.attack = false;
    }
}

