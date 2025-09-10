using UnityEngine;
using System;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("PLAYER WEAPON")]
    public List<WeaponImages> weaponSprites;
    public Animator SelectWeapon;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class WeaponImages
{
    public string name;
    public Sprite weaponSprite;
}
