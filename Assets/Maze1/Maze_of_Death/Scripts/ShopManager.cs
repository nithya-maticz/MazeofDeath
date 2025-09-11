using UnityEngine;
using System;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Temperory Data")]
    public string userWeaponString;
    public string userEquippedString;

    [Header("PLAYER WEAPON")]
    public WeaponsData userWeapons;
    public WeaponsData userEquipedWeapons;
    public List<WeaponImages> weaponSprites;
    public Animator SelectWeapon;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GetUserWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetUserWeapons()
    {
        userWeapons = JsonUtility.FromJson<WeaponsData>(userWeaponString);
        userEquipedWeapons = JsonUtility.FromJson<WeaponsData>(userEquippedString);

    }
}

[Serializable]
public class WeaponImages
{
    public string name;
    public Sprite weaponSprite;
}

[Serializable]
public class WeaponsData
{
    public List<Weapon> weapons;
}


[Serializable]
public class Weapon
{
    public string name;
    public string image;
    public bool isGun;
    public bool isKnife;
    public bool isThrowables;
    public List<Demage> demage;
    public float health;
    public Cost repairCost;
    public float reloadTime;
    public string bulletType;
    public int magazineSize;
    public int bulletCount;
    public int throwablesCount;
    public string fireRate;
    public float range;
    public Cost cost;
    public string userId;
    public string itemId;
}

[Serializable]
public class Cost
{
    public string currency;
    public float value;
}

[Serializable]
public class Demage
{
    public string zombieName;
    public float demage;
}
