using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using NavMeshPlus.Components;
public class ManagerMaze : MonoBehaviour
{
    public GameObject enemy;
    public Transform spawnPosition;

    public Player playerRef;
    public bool attackPlayer;
    public bool isPlayerGetKey;
    public GameObject keyImg;
    public GameObject Playerweapon;
    public bool isGameOver;

    public static ManagerMaze instance;
    public bool CreateEnemy;
    public List<GameObject> Enemies;
    public int EnemyCount;
    public SpriteRenderer EnemyDoorSprite;
    public SpriteRenderer PlayerDoorSprite;
    public List<ZombieDoor> ZombieDoors;
    public List<KeyBox> KeyBoxes;
    public Blood BloodPrefab;
    public Transform BloodTransform;
    int _boxCount;
    public TMP_Text OpenedBoxText;

    public TMP_Text enemyCountText;
    public TMP_Text zombieDoorCount;
    public GameObject SplashScreen;
    public GameObject videoRawImage;
    [Header("SPRITES")]
    public Sprite SpriteBoxOpen;
    public Sprite SpriteBoxClose;
    public Sprite DoorOpen;
    public Sprite DoorClose;

    public Sprite damage1;
    public Sprite damage2;
    public Sprite damage3;
    public Sprite damage4;

    [Header("TREASURE HANDLE")]
    public Animator TreasureAnimation;
    public GameObject TreasureKey;
    public GameObject TreasureMedikit;

    [Header("WIN PAGE")]
    public GameObject WinPage;

    [Header("GAMEOVER")]
    public GameObject GameOverPage;

    public GameObject PlayerImage;

    [Header("PATROL POINTS")]
    public List<PatrolPoints> PatrolAreas;

    public Animator enemyAnimatorRef;
    public Image bloodImage;

    public Transform[] partolPoints;
    public int targetPoint;
    public float enemySpeed;

    
    [Header("Videoplayer")]
    public List<VideoControl> videoPlayer;

    public int count = 0;
    public  bool loadVideo;

    public Animator fadeAnimation;
    public GameObject FadeImg;
    public GameObject rawImage;
    public bool gameStart;
    public TextMeshProUGUI textMeshPro;
    public TextMeshProUGUI textMeshPro1;
    
    public float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;


    public GameObject blackScreen;
    

    void Start()
    {
        Invoke("LoadVideo", 3f);
        isPlayerGetKey = false;
        isGameOver = false;
        
        keyImg.SetActive(false);

        /*for(int i=0;i<videoPlayer.Count;i++)
        {
            videoPlayer[i].videoPlayer.Pause();
        }*/
        EnemiesCount();
        DoorClosedCount();
    }


    public void StartTyping()
    {
       if(videoPlayer.Count>count && gameStart==false)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText());
        }
       
        
    }

    IEnumerator TypeText()
    {
        textMeshPro.text = "";
        foreach (char c in videoPlayer[count].fullText)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        Debug.Log("completed");
        if( gameStart == false)
        {
            Invoke("playFun", 5f);
        }
       
        


    }

    public void playFun()
    {
       
        if (count<5&& gameStart==false)
        {
            FadeImg.SetActive(true);
            fadeAnimation.SetTrigger("fade");

        }
        else
        {
            blackScreen.SetActive(true);
            StartTyping1("His routine night on the job had transformed into something far more sinister, and now every shadow could be his last.");
        }

    }


    public void Skipfun()
    {
        if(gameStart==false)
        {

            gameStart = true;
            blackScreen.SetActive(false);
            Debug.Log("Skifun");
            FadeImg.SetActive(true);
            fadeAnimation.SetTrigger("fade");
            rawImage.SetActive(false);
            StopCoroutine(typingCoroutine);
            textMeshPro.text = "";
            Invoke("startgame", 2f);
        }
        



    }

    public void startgame()
    {
        FadeImg.SetActive(false);
        for (int i = 0; i < ZombieDoors.Count; i++)
        {
            ZombieDoors[i].StartGame();
        }

    }





    public void LoadVideo()
    {
        SplashScreen.SetActive(false);
        videoRawImage.SetActive(true);
        Debug.Log("lOADvIDEO");
        FadeImg.SetActive(true);
        fadeAnimation.SetTrigger("fade");
        loadVideo = true;
        

    }

    public void DelayCountEnemies()
    {
        Invoke("EnemiesCount", 0.5f);
    }

    public void EnemiesCount()
    {
        Enemies.Clear();
        Enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("enemy"));
        enemyCountText.text =  Enemies.Count.ToString();
        CheckLevelUp();
    }


    public void DoorClosedCount()
    {
        int count = 0;
        foreach(ZombieDoor door in ZombieDoors)
        {
            if(!door.isClosed)
            {
                count++;
            }
        }

        zombieDoorCount.text = count.ToString();
    }
    private void Awake()
    {
        instance = this;
       
    }

    // Update is called once per frame


   
   

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
           Player.Instance.PlayerAttackButton();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
           LoadVideo();
        }

    }


    public void AttackEnemy()
    {
        attackPlayer = true;
    }
    public void RestartFun()
    {
        SceneManager.LoadScene("Maze1");
    }



    public void GetTreasure(BoxObjects boxObjects)
    {
        if (boxObjects == BoxObjects.Key)
        {
            TreasureAnimation.gameObject.SetActive(true);
            TreasureKey.SetActive(true);
            TreasureAnimation.SetTrigger("Key");
        }
        else if (boxObjects == BoxObjects.MediKit)
        {
            TreasureAnimation.gameObject.SetActive(true);
            TreasureMedikit.SetActive(true);
            TreasureAnimation.SetTrigger("MediKit");
        }
        else if (boxObjects == BoxObjects.Empty)
        {
            return;
        }
    }

    public void CheckLevelUp()
    {
        bool allclosed = true;
        
        foreach (ZombieDoor door in ZombieDoors)
        {
            if (!door.isClosed)
                allclosed = false;
        }

        if (allclosed && Enemies.Count == 0)
            Invoke("LevelUP", 1f);
        else
            return;
    }


    void LevelUP()
    {
        foreach (GameObject enemy in Enemies)
        {
            Destroy(enemy);
        }
        WinPage.SetActive(true);
    }

    public void GameOver()
    {
       /* foreach (Enemy enemy in Enemies)
        {
            Debug.Log("GameOver-----------");

            Destroy(enemy.gameObject);
        }
        Invoke("game_over", 1f);*/
    }

    void game_over()
    {

        GameOverPage.SetActive(true);
    }

    public void CheckBoxCount()
    {
        _boxCount = 0;
        foreach (KeyBox box in KeyBoxes)
        {
            if (box.IsOpened)
            {
                _boxCount++;
            }
        }

        OpenedBoxText.text = _boxCount.ToString() + "/" + KeyBoxes.Count.ToString();

    }


    public void StartTyping1(string text)
    {
        if (videoPlayer.Count > count && gameStart == false)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText1(text));
        }


    }

    IEnumerator TypeText1(string text)
    {
        textMeshPro1.text = "";
        foreach (char c in text)
        {
            textMeshPro1.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        Debug.Log("completed");
        if (gameStart == false)
        {
            Invoke("Skipfun", 5f);
        }




    }

}


[Serializable]
public class CharacterObj
{
    public ObjectDatas rightObj;
    public ObjectDatas leftObj;
}

[Serializable]
public class ObjectDatas
{
    public GameObject obj;

}

[Serializable]
public struct PatrolPoints
{
    public string Area;
    public List<Transform> Points;
}

[Serializable]
public class VideoControl
{
  
    public VideoPlayer videoPlayer;
    public RenderTexture videoTexture;

    [TextArea] public string fullText;
}
