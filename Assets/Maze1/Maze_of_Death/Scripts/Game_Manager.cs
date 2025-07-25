using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;


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

    [Header("Auto Controls")]
    public GameObject KnifeObject;
    public Button KnifeButton;
    public GameObject GunObject;
    public Button GunButton;
    public bool IsGun;
    public bool IsKnife;
    public Toggle AutoAttackToogle;
    public Toggle ManualShootToogle;
    public Toggle AutoAimAndAutoShootToggle;
    public Toggle AutoAimAndManualShootToggle;
    public Toggle ManualAimAndShootToggle;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    IEnumerator Start()
    {
        //Application.targetFrameRate = 60; // or 30 for very low-end devices
        QualitySettings.vSyncCount = 0;   // Disable VSync to let targetFrameRate control FPS
        AutoAim = AutoAimOnly.Instance;
        PlayerHealthCount = 4;
        UpdateAttackSettings();
        yield return new WaitForSeconds(0.5f);
        BoxCount();
        ZombieDoorCount();
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
            WinPage.SetActive(true);
        }
    }

    public void Attack()
    {
        if (!AutoAttackToogle.isOn)
            PlayerMovements.Instance._Animator.SetTrigger("Attack");
    }

    public void Shoot()
    {
       if(ManualAimAndShootToggle.isOn)
       {
            Bullet bullet = Instantiate(BulletPrefab, BulletSpawner);
            bullet.transform.localPosition = Vector3.zero;
            bullet.Target = PlayerMovements.Instance.ManualTarget;
            bullet.GO = true;
       }
       else
       {
            if (AutoAim.currentTarget != null)
            {
                Bullet bullet = Instantiate(BulletPrefab, BulletSpawner);
                bullet.transform.localPosition = Vector3.zero;
                bullet.Target = AutoAim.currentTarget.transform;
                bullet.GO = true;
            }
       }
        
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

        if(AutoAimAndAutoShootToggle.isOn || AutoAimAndManualShootToggle.isOn)
        {
            AutoAim.enabled = true;

            if(AutoAimAndManualShootToggle.isOn)
            {
                GunButton.interactable = true;
            }
            else
            {
                GunButton.interactable = false;
            }
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
}


