using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using Unity.Cinemachine;



public class ManagerMaze : MonoBehaviour
{
    public GameObject enemy;
    public Transform spawnPosition;

    //public Player playerRef;
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
    public Joystick joystick;

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
    public GameObject miniMap;

    [Header("CHARACTER SELECTION")]
    public GameObject characterSelectionPage;
    public Player malePlayer;
    public Player femalePlayer;
    public Transform playerSpawnPoint;
    public CinemachineCamera cineCam;

    void Start()
    {
        Invoke("LoadVideo", 3f);
        isPlayerGetKey = false;
        isGameOver = false;
        
        keyImg.SetActive(false);

        for (int i = 0; i < videoPlayer.Count; i++)
        {
            videoPlayer[i].videoPlayer.Pause();
        }
        EnemiesCount();
        DoorClosedCount();
        AssignRandomContents();
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
            if(count == 5)
            {
                blackScreen.SetActive(true);
                StartTyping1("His routine night on the job had transformed into something far more sinister, and now every shadow could be his last.");
            }
            
        }

    }


    public void Skipfun()
    {
        if(!gameStart)
        {
            gameStart = true;
            blackScreen.SetActive(false);
            FadeImg.SetActive(true);
            fadeAnimation.SetTrigger("fade");
            rawImage.SetActive(false);
            StopCoroutine(typingCoroutine);
            textMeshPro.text = "";
            characterSelectionPage.SetActive(true);
            //Invoke("startgame", 2f);
        }
    }

    public void Startgame()
    {
        characterSelectionPage.SetActive(false);
        FadeImg.SetActive(false);
        miniMap.SetActive(true);

        for (int i = 0; i < ZombieDoors.Count; i++)
        {
            ZombieDoors[i].SpawnEnemyFromDoor();
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
            try
            {
                Debug.Log("Space key pressed");
                Player.Instance.PlayerAttackButton();
            }
            catch (Exception e)
            {
                Debug.LogError("Error: " + e.Message);
            }
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

    void AssignRandomContents()
    {
        // Step 1: create a list of indices
        List<int> indices = new List<int>();
        for (int i = 0; i < KeyBoxes.Count; i++)
        {
            indices.Add(i);
        }

        // Step 2: shuffle indices
        ShuffleList(indices);

        // Step 3: assign contents
        for (int i = 0; i < KeyBoxes.Count; i++)
        {
            if (i == 0)
                KeyBoxes[indices[i]].SelectedObject  = BoxObjects.Key;
            else if (i == 1 || i == 2)
                KeyBoxes[indices[i]].SelectedObject = BoxObjects.MediKit;
            else
                KeyBoxes[indices[i]].SelectedObject = BoxObjects.Empty;
        }
    }

    // Fisher-Yates shuffle
    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }

    public void CharacterSelection(string selectedAvatar)
    {
        if(selectedAvatar == "Male")
        {

            GameObject player = Instantiate(malePlayer.gameObject, playerSpawnPoint.position, Quaternion.identity);
            joystick.StartJoystick();
            cineCam.Target.TrackingTarget = player.transform;
            Startgame();
        }
        else if(selectedAvatar == "Female")
        {
            GameObject player = Instantiate(femalePlayer.gameObject, playerSpawnPoint.position, Quaternion.identity);
            joystick.StartJoystick();
            cineCam.Target.TrackingTarget = player.transform;
            Startgame();
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
