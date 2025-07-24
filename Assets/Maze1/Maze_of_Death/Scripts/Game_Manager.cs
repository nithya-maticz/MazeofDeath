using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

public class Game_Manager : MonoBehaviour
{
    public static Game_Manager Instance;
    public VariableJoystick movementJoystick;
    
    public IGetTreasure curretTreasure;
    

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

    public int MedikitCount;
    public TMP_Text MedikitCountText;

    [Header("Enemy Attributes")]
    public GameObject BloodPrefab;
    public GameObject EnemyPrefab;
    public List<Transform> PatrolPoints;
    public List<ZombieDoor> ZombieDoors;
    public TMP_Text ZombieDoorCountText;
    public List<SharedPathFollower> Enemies;
    public TMP_Text EnemyCountText;

    [Header("PLAYER Health")]
    public int PlayerHealthCount;
    public List<Sprite> HealthSprites;
    public Image HealthImage;


    private void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        PlayerHealthCount = 4;
        yield return new WaitForSeconds(0.5f);
        BoxCount();
        ZombieDoorCount();
    }

    public void EnemyCount()
    {
        EnemyCountText.text = Enemies.Count.ToString();
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

        ZombieDoorCountText.text = a.ToString();
    }

    public void UpdatePlayerHealth()
    {
        if(PlayerHealthCount <= 0)
        {
            HealthImage.gameObject.SetActive(true);
            HealthImage.sprite = HealthSprites[2];
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
}


