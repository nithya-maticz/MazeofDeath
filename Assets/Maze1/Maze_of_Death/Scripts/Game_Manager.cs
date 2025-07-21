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


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
       
    }

}


