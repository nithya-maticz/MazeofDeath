
using UnityEngine;
using UnityEngine.UI;


public class StoryManager : MonoBehaviour
{
   /* public Player malePlayer;
    public Player femalePlayer;
    public Transform playerSpawnPoint;*/

    
    public Joystick joystick;
    public GameObject enemy;
    public static StoryManager Instance;
    public GameObject box_heart;
    public GameObject fillImage;
    public GameObject box_key;

    public GameObject CollideCircle;
    public GameObject FillCanvas;
    public GameObject CollideCircle1;
    public GameObject FillCanvas1;
    public bool keyTaken;
    public bool enemyDestory;
    public bool knifeTaken;

    public GameObject content1;
    public GameObject content2;
    public GameObject content3;
    public GameObject content4;
    public GameObject content5;
    public GameObject attackBut;


    void Start()
    {
        Invoke("ContentFun1", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake()
    {
        Instance = this;
    }
    public void FirstBox()
    {
        CollideCircle.SetActive(false);
        FillCanvas.SetActive(false);
        Invoke("content3visiblefun", 1f);
        Invoke("content3fun", 5f);
        attackBut.SetActive(true);
        FindObjectOfType<knifeBox>().EndAnimation();
            
    }
    public void SecondBox()
    {
        CollideCircle1.SetActive(false);
       
        FindObjectOfType<KeyBox1>().EndAnimation();
    }
    public void ContentFun1()
    {
        content1.SetActive(false);
    }
     public void content3visiblefun()
    {
        content4.SetActive(true);
        Invoke("content3fun", 5f);
    }
     public void content3fun()
     {
          content4.SetActive(false);
          content3.SetActive(true);
        Invoke("content3invisible", 5f);


    }
    public void content3invisible()
    {
        content3.SetActive(false);
    }
    public void content4visiblefun()
    {
        content4.SetActive(true);
        Invoke("content4Invisiblefun", 5f);
    }
    public void content4Invisiblefun()
    {
        content4.SetActive(false);
     
    }
    public void content5visiblefun()
    {
        content5.SetActive(true);
        Invoke("content5Invisiblefun", 5f);
    }
    public void content5Invisiblefun()
    {
        content5.SetActive(false);

    }
    public void AttackFun()
    {
        StoryPlayer.Instance.AttackFunction();
    }
}
