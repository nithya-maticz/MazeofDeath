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
    public List<IGetTreasure> Boxes;
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
    public List<SharedPathFollower> Enemies;
    public TMP_Text EnemyCountText;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
       
    }

    public void EnemyCount()
    {
        EnemyCountText.text = Enemies.Count.ToString();
    }


}


