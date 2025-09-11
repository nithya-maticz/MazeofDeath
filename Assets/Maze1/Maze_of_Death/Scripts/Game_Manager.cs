using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager Instance;
    public PlayerHealth playerHealth;
    public VariableJoystick movementJoystick;
    
    public IGetTreasure curretTreasure;
    public GameObject LostPage;
    public GameObject WinPage;
    public GameObject loadingScreen;
    public TMP_Text winLevelTxt;
    public TMP_Text loseLevelTxt;

    [Header("Sprites")]
    public Sprite SpriteBoxOpen;
    public Sprite KeySprite;
    public Sprite MedikitSprite;
    public Sprite DoorCloseSprite;

    [Header("Mystery Box")]
    public int BoxCountInt;
    public TMP_Text RemainingBoxText;
    int _boxCount;
    public TMP_Text OpenedBoxText;
    public Animator GetMysteryPage;
    public Image GetMysteryImage;
   

    [Header("Player Attributes")]
    public bool IsGetKey;
    public GameObject KeyImage;
    public bool IsShowKey;

    

    [Header("Enemy Attributes")]
    public GameObject BloodPrefab;
    public GameObject EnemyPrefab;
    public List<Transform> PatrolPoints;
  //  public List<GameObject> PatrolPoints;
    public List<ZombieDoor> ZombieDoors;
    public TMP_Text ZombieDoorCountText;
    public List<SharedPathFollower> Enemies;
    public TMP_Text EnemyCountText;
    public int ZombieDoorCountInt;


    [Header("PLAYER Health")]
    public List<Sprite> HealthSprites;
    public Image HealthImage;

    public Image MedikitFillImage;
    public Button UseMediKit;
    public int MedikitCount;
    public TMP_Text MedikitCountText;
    

    [Header("Shoot")]
    public AutoAimOnly AutoAim;
    public Transform BulletSpawner;
    public Bullet BulletPrefab;
    public int totalBullets = 15;     
    public int currentBullets = 5;
    public TMP_Text totalBulletsText;
    public Image reloadFillImage;
    public int maxMagazineSize = 5;
    public bool isReloading = false;

    [Header("Auto Controls")]
    public GameObject KnifeObject;
    public Button KnifeButton;
    public GameObject GunObject;
    public Button GunButton;
    public bool IsGun;
    public bool IsKnife;
    public Toggle AutoAttackToogle;
    public Toggle ManualShootToogle;
    //public Toggle AutoAimAndAutoShootToggle;
    public Toggle AutoAimAndManualShootToggle;
    public Toggle ManualAimAndShootToggle;
    public Transform playerTransform;
    public GameObject playerPrefab;
    public GameObject F_PlayerPrefab;

    public GameObject bombPrefab;
    public GameObject bomb;
    public bool isBomb;

    [Header("MATERIAL")]
    public Material litMat;

    [Header("Zombie Patrol")]
    public List<PatrolAreas> patrolAreas;
    public int currentPatrolId;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentPatrolId = 1;
       /* Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;*/
    }
   
    


    void Start()
    {
      
        //StartData();
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AttackFunction();
        }
    }

    public IEnumerator StartData()
    {
        print("Started..");

        Enemies.Clear();
        ZombieDoors.Clear();
        // AutoAim = AutoAimOnly.Instance;
        //PlayerHealthCount = 4;
        UpdateAttackSettings();
        UpdateUI();
        reloadFillImage.fillAmount = 0f;
        yield return new WaitForSeconds(0.5f);
        BoxCount();
        ZombieDoorCount();
        loadingScreen.SetActive(false);

    }

    public void SpawnPlayer(Vector3 worldPos)
    {
        playerTransform.position = worldPos;
        Instantiate(playerPrefab, playerTransform.transform);
        SmoothFollowCamera.Instance.target = PlayerMovements.Instance.offSet;
        BulletSpawner = PlayerMovements.Instance.bulletSpawn;
        AutoAim = AutoAimOnly.Instance;

        /*if (LobbyManager.currentCharacter == 1)
        {
            Debug.Log("Male");
            Instantiate(playerPrefab, worldPos, Quaternion.identity);
            SmoothFollowCamera.Instance.target = PlayerMovements.Instance.offSet;
            BulletSpawner = PlayerMovements.Instance.bulletSpawn;
            AutoAim = AutoAimOnly.Instance;
        }
        else if (LobbyManager.currentCharacter == 2)
        {
            Debug.Log("Female");
            Instantiate(F_PlayerPrefab, worldPos, Quaternion.identity);
            SmoothFollowCamera.Instance.target = PlayerMovements.Instance.offSet;
            BulletSpawner = PlayerMovements.Instance.bulletSpawn;
            AutoAim = AutoAimOnly.Instance;
        }*/
    }



    public void AttackFunction()
    {
        if (IsGun)
        {
            Shoot();
        }
        else
        {
            Attack();
        }
        
    }

   

    public void EnemyCount()
    {

        EnemyCountText.text = Enemies.Count.ToString();
        LevelUP();
    }

    public void BoxCount()
    {
        RemainingBoxText.text = BoxCountInt.ToString(); 

    }

    public void ZombieDoorCount()
    {
        int a = 0;

        foreach(ZombieDoor door in ZombieDoors)
        {
            if(!door.isClosed)
            {
                a++;
            }
            
        }

        ZombieDoorCountInt = a;
        ZombieDoorCountText.text = a.ToString();
        LevelUP();
    }

    public void GameOver()
    {
        LostPage.SetActive(true);
        loseLevelTxt.text = "LEVEL " + LobbyManager.currentLevel.ToString();
        Destroy(PlayerMovements.Instance.gameObject);
        foreach (SharedPathFollower ene in Enemies)
        {
            Destroy(ene.gameObject);
        }
        foreach (ZombieDoor door in ZombieDoors)
        {
            Destroy(door.gameObject);
        }
    }

   

    public void OnUseMediKit()
    {
        if (MedikitCount > 0 && !playerHealth.isFilling)
        {
            MedikitCount--;
            playerHealth.UpdateMedikitUI();
            StartCoroutine(playerHealth.FillMedikit());
        }
    }

    

    

    public void LevelUP()
    {
        //
      
        if (Enemies.Count == 0 && ZombieDoorCountInt == 0)
        {
            Invoke("win", 1f);
            LocalData data = new LocalData();
            winLevelTxt.text = "LEVEL " + LobbyManager.currentLevel.ToString();
            //print("Current Level : " + LobbyManager.currentLevel);
            data.level = LobbyManager.currentLevel + 1;
            data.avatar = LobbyManager.currentCharacter;
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("Localdata", json);
            PlayerPrefs.Save();
            LobbyManager.currentLevel = data.level;

            print("LevelUp : " + PlayerPrefs.GetString("Localdata"));

        }
    }
    public void win()
    {
        WinPage.SetActive(true);
    }

    public void Attack()
    {
        if (!AutoAttackToogle.isOn && PlayerMovements.Instance.isAttack==false)
        {
            PlayerMovements.Instance.isAttack = true;
            PlayerMovements.Instance.ResetAllTriggers();
            PlayerMovements.Instance._Animator.SetTrigger("Attack");
        }
           
    }

    public void Shoot()
    {
       if(ManualAimAndShootToggle.isOn)
       {
            if (currentBullets > 0 && !isReloading)
            {
                currentBullets--;
                Debug.Log("Shot fired! Bullets left: " + currentBullets);
                UpdateUI();
                PlayerMovements.Instance._Animator.SetTrigger("Shoot");
                Bullet bullet = Instantiate(BulletPrefab, BulletSpawner);
                bullet.transform.localPosition = Vector3.zero;
                bullet.Target = PlayerMovements.Instance.ManualTarget;
                bullet.GO = true;

                if (currentBullets == 0)
                {
                    StartCoroutine(Reload());
                }
            }
            else
            {
                Debug.Log("No bullets in magazine!");

                if (totalBullets <= 0)
                {
                    Debug.Log("Totally out of ammo!");
                    GunButton.interactable = false;

                }
            }

       }
       else
       {
            if (AutoAim.currentTarget != null)
            {

                if (currentBullets > 0 && !isReloading)
                {
                    currentBullets--;
                    Debug.Log("Shot fired! Bullets left: " + currentBullets);
                    UpdateUI();
                    PlayerMovements.Instance.isAttack = true;
                    PlayerMovements.Instance._Animator.SetTrigger("Shoot");
                    Bullet bullet = Instantiate(BulletPrefab, BulletSpawner);
                    bullet.transform.localPosition = Vector3.zero;
                    bullet.Target = AutoAim.currentTarget.transform;
                    bullet.GO = true;

                    if (currentBullets == 0)
                    {
                        StartCoroutine(Reload());
                    }
                }
                else
                {
                    Debug.Log("No bullets in magazine!");

                    if (totalBullets <= 0)
                    {
                        Debug.Log("Totally out of ammo!");
                        GunButton.interactable = false;

                    }
                }

            }
       }
        
    }

    public void ManualReload()
    {
        if(!isReloading && currentBullets < maxMagazineSize && totalBullets > 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        GunButton.interactable = false;
        reloadFillImage.fillAmount = 0f;
        float reloadTime = 3f;
        float elapsed = 0f;

        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            reloadFillImage.fillAmount = Mathf.Clamp01(elapsed / reloadTime);
            yield return null;
        }

        int bulletsToReload = Mathf.Min(maxMagazineSize, totalBullets);
        currentBullets = bulletsToReload;
        totalBullets -= bulletsToReload;

        Debug.Log("Reloaded! Current: " + currentBullets + ", Total: " + totalBullets);

        UpdateUI();
        reloadFillImage.fillAmount = 0f;
        isReloading = false;
        GunButton.interactable = true;
    }

    void UpdateUI()
    {
        totalBulletsText.text = currentBullets.ToString() + "/"+ totalBullets.ToString();
       
    }

    public void EnableAutoAim()
    {
        AutoAim.enabled = true;
    }

    public void DiableAutoAim()
    {
        AutoAim.enabled = false;
    }

    public void GunChange()
    {
        IsGun = true;
        IsKnife = false;
        GunObject.SetActive(true);
        KnifeObject.SetActive(false);

        if(AutoAimAndManualShootToggle.isOn)
        {
            AutoAim.enabled = true;
            GunButton.interactable = true;
           
        }
        else if(ManualAimAndShootToggle.isOn)
        {
            AutoAim.enabled = false;
            GunButton.interactable = true;
        }
    }

    public void KnifeChange()
    {
        IsGun = false;
        IsKnife = true;
        GunObject.SetActive(false);
        KnifeObject.SetActive(true);

        if(AutoAttackToogle.isOn)
        {
            KnifeButton.interactable = false;
        }
        else
        {
            KnifeButton.interactable= true;
        }

        AutoAim.enabled = false;
    }

    public void UpdateAttackSettings()
    {
        if (IsGun && !IsKnife)
        {
            GunChange();
        }
        else if(!IsGun && IsKnife)
        {
            KnifeChange();
        }
    }

    public void ReloadScene()
    {
        LobbyManager.currentLevel--;
        Scene currentScene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(currentScene.buildIndex);

        SceneManager.LoadScene(currentScene.buildIndex, LoadSceneMode.Single);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public void NextLevel()
    {
        //LobbyManager.currentLevel++;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("Game Loader", LoadSceneMode.Single);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public void RetryScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("Game Loader", LoadSceneMode.Single);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public void SceneLoad(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
    public void BackToLobby()
    {
        SceneManager.LoadScene("lobby");
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    public void BombFun()
    {
        PlayerMovements.Instance._Animator.SetTrigger("bomb");
    }
    public void DestoryBomb()
    {
        PlayerMovements.Instance.bombSpawnPoint.SetActive(false);
        isBomb = false;
        Destroy(bomb);
    }
    public void BombAnimattion()
    {
        Transform targetTransform = PlayerMovements.Instance.bombSpawnPoint.transform;
        PlayerMovements.Instance.bombSpawnPoint.SetActive(false);
        isBomb = true;
        PlayerMovements.Instance.bombSpawnPoint.SetActive(true);
        bomb = Instantiate(
    bombPrefab,
    targetTransform.position,
    Quaternion.identity // or targetTransform.rotation if needed
);
        Invoke("DestoryBomb", 7f);
    }

    public void GeneratePatrolAreas()
    {
        patrolAreas = new List<PatrolAreas>();
        Debug.Log("Started Generate....");
        int id = 1;
        for (int i = 0; i < PatrolPoints.Count; i += 2)
        {
            PatrolAreas area = new PatrolAreas();
            area._id = id++;
            area.isOccupied = false;

            // Add 2 points (if available)
            if (i < PatrolPoints.Count)
                area.patrolPoints.Add(PatrolPoints[i]);

            if (i + 1 < PatrolPoints.Count)
                area.patrolPoints.Add(PatrolPoints[i + 1]);

            patrolAreas.Add(area);
        }
        Debug.Log("Ended Generate....");
        Debug.Log($"Created {patrolAreas.Count} patrol areas.");
    }
}


[Serializable]
public enum TilemapType
{
    Walls,
    Ground,
    Obstacles
}

[Serializable]
public class PatrolAreas
{
    public int _id;
    public List<Transform> patrolPoints = new List<Transform>();
    public bool isOccupied;
}
