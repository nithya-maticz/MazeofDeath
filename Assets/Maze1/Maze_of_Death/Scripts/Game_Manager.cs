using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager Instance;
    public VariableJoystick movementJoystick;
    
    public IGetTreasure curretTreasure;
    public GameObject LostPage;
    public GameObject WinPage;
   

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
    public int PlayerHealthCount;
    public List<Sprite> HealthSprites;
    public Image HealthImage;
    

    public Image MedikitFillImage;
    public Button UseMediKit;
    public int MedikitCount;
    public TMP_Text MedikitCountText;
    private bool isFilling = false;

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

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
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
       // Debug.Log("LOBBY    ");

        /*Instantiate(F_PlayerPrefab, playerTransform.transform);
        SmoothFollowCamera.Instance.target = PlayerMovements.Instance.offSet;
        BulletSpawner = PlayerMovements.Instance.bulletSpawn;*/

       /* if (LobbyManager.currentIndex == 1)
        {
            Debug.Log("Male");
            Instantiate(playerPrefab, playerTransform.transform);
            SmoothFollowCamera.Instance.target = PlayerMovements.Instance.offSet;
            BulletSpawner = PlayerMovements.Instance.bulletSpawn;
        }
        else if (LobbyManager.currentIndex == 2)
        {
            Debug.Log("Female");
            Instantiate(F_PlayerPrefab, playerTransform.transform);
            SmoothFollowCamera.Instance.target = PlayerMovements.Instance.offSet;
            BulletSpawner = PlayerMovements.Instance.bulletSpawn;
        }
*/
        QualitySettings.vSyncCount = 0;
       // AutoAim = AutoAimOnly.Instance;
        PlayerHealthCount = 4;
        UpdateAttackSettings();
        UpdateUI();
        reloadFillImage.fillAmount = 0f;
        yield return new WaitForSeconds(0.5f);
        BoxCount();
        ZombieDoorCount();
    }

    public void SpawnPlayer(Vector3 worldPos)
    {
        playerTransform.position = worldPos;
        Instantiate(playerPrefab, playerTransform.transform);
        SmoothFollowCamera.Instance.target = PlayerMovements.Instance.offSet;
        BulletSpawner = PlayerMovements.Instance.bulletSpawn;
        AutoAim = AutoAimOnly.Instance;

        /*   if (LobbyManager.currentIndex == 1)
           {
               Debug.Log("Male");
               Instantiate(playerPrefab, worldPos, Quaternion.identity);
               SmoothFollowCamera.Instance.target = PlayerMovements.Instance.offSet;
               BulletSpawner = PlayerMovements.Instance.bulletSpawn;
           }
           else if (LobbyManager.currentIndex == 2)
           {
               Debug.Log("Female");
               Instantiate(F_PlayerPrefab, worldPos, Quaternion.identity);
               SmoothFollowCamera.Instance.target = PlayerMovements.Instance.offSet;
               BulletSpawner = PlayerMovements.Instance.bulletSpawn;
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

    public void UpdatePlayerHealth()
    {
        if(PlayerHealthCount <= 0)
        {
            HealthImage.gameObject.SetActive(true);
            HealthImage.sprite = HealthSprites[2];
            LostPage.SetActive(true);
            //Game Over
        }
        else
        {
            switch (PlayerHealthCount)
            {
                case 1:
                    HealthImage.gameObject.SetActive(true);
                    HealthImage.sprite = HealthSprites[1];
                    break;

                case 2:
                    HealthImage.gameObject.SetActive(true);
                    HealthImage.sprite = HealthSprites[0];
                    break;

                case 3:
                    HealthImage.gameObject.SetActive(false);
                    break;

                case 4:
                    HealthImage.gameObject.SetActive(false);
                    break;

            }

        }
    }

    public void OnUseMediKit()
    {
        if (MedikitCount > 0 && !isFilling)
        {
            MedikitCount--;
            UpdateMedikitUI();
            StartCoroutine(FillMedikit());
        }
    }

    void UpdateMedikitUI()
    {
        if(MedikitCount <= 0)
        {
            UseMediKit.interactable = false;
        }
        MedikitCountText.text = MedikitCount.ToString();
    }

    IEnumerator FillMedikit()
    {
        isFilling = true;

        // Set full opacity
        SetImageAlpha(MedikitFillImage, 1f);

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            MedikitFillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        // Done filling
        Debug.Log("Filled!");
        PlayerHealthCount = 4;
        UpdatePlayerHealth();
        SetImageAlpha(MedikitFillImage, 0.2f);

        isFilling = false;
    }

    void SetImageAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    public void LevelUP()
    {
        if(Enemies.Count == 0 && ZombieDoorCountInt == 0)
        {
            Invoke("win", 1f);
            
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
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void NextLevel()
    {
        LevelManager.levelToLoad++;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void SceneLoad(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void BackToLobby()
    {
        SceneManager.LoadScene("lobby");
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
        isBomb = true;
        PlayerMovements.Instance.bombSpawnPoint.SetActive(true);
        bomb = Instantiate(
    bombPrefab,
    targetTransform.position,
    Quaternion.identity // or targetTransform.rotation if needed
);
    

        Invoke("DestoryBomb", 2f);
    }
}


