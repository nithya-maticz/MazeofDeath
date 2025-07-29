
using System.Collections.Generic;
using TMPro;
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

   
    public GameObject attackBut;
    public GameObject knifecontent4;
    public GameObject content;
    public GameObject blackscreen;

  

    [Header("Enemy Attributes")]
    public GameObject BloodPrefab;
    public GameObject EnemyPrefab;
    public List<Transform> PatrolPoints;
    public List<ZombieDoor> ZombieDoors;
    public TMP_Text ZombieDoorCountText;
    public List<SharedPathFollower> Enemies;
    public TMP_Text EnemyCountText;
    public int ZombieDoorCountInt;

    [Header("PLAYER Health")]
    public int PlayerHealthCount;
    public List<Sprite> HealthSprites;
    public Image HealthImage;


    void Start()
    {
       // Invoke("ContentFun1", 2f);
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
        attackBut.SetActive(true);
        Invoke("delayknife", 1f);
        FindObjectOfType<knifeBox>().EndAnimation();
            
    }
    public void SecondBox()
    {
        CollideCircle1.SetActive(false);
       
        FindObjectOfType<KeyBox1>().EndAnimation();
    }
    
    public void AttackFun()
    {
        StoryPlayer.Instance.AttackFunction();
    }

    public void delayknife()
    {
        content.SetActive(true);
        knifecontent4.SetActive(true);
       
    }
}
