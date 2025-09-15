using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal.Internal;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public List<WeaponImages> weaponSprites;

    [Header("Temperory Data")]
    public string weaponDB;
    public string userWeaponString;
    public string userEquippedString;

    [Header("PLAYER WEAPON")]
    public WeaponsData userEquipedWeapons;
    public PrimaryGun primaryGun;
    public SecondaryGun secondaryGun;
    public PrimaryKnife primaryKnife;
    public Animator SelectWeapon;

    [Header("Owned Weapons")]
    public WeaponsData userWeapons;
    public OwnedWeapon ownedWeaponPrefab;
    public Transform ownedWeaponContent;
    public OwnedWeapon ownedThrowablesPrefab;
    public Transform ownedThrowablesContent;

    [Header("All Weapons")]
    public WeaponsData allWeapons;
    public BuyWeapon buyWeaponPrefab;
    public Transform buyWeaponContent;

    [Header("QUICK EQUIP")]
    public QuickEquipWeapon quickEquipPrefab;
    public Transform quickEquipContent;
    public string currentSelection;

    
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        allWeapons = JsonUtility.FromJson<WeaponsData>(weaponDB);
        userWeapons = JsonUtility.FromJson<WeaponsData>(userWeaponString);
        userEquipedWeapons = JsonUtility.FromJson<WeaponsData>(userEquippedString);
        GetAllWeapons();
        GetOwnedWeapons();
        GetEquipedWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetEquipedWeapons()
    {
        int i = 0;
        primaryGun.UnEquip();
        secondaryGun.UnEquip();

        foreach (Weapon weapon in userEquipedWeapons.weapons)
        {
            if(weapon.isGun)
            {
                i++;
                if(i == 1)
                {
                    primaryGun.data = weapon;
                    primaryGun.AssignData();
                }
                else if(i == 2)
                {
                    secondaryGun.data = weapon;
                    secondaryGun.AssignData();
                }
            }
            else if(weapon.isKnife)
            {
                primaryKnife.data = weapon;
                primaryKnife.AssignData();
            }
        }
    }

    void GetOwnedWeapons()
    {
        DeleteAllChild(ownedWeaponContent);
        foreach (Weapon weapon in userWeapons.weapons)
        {
            if((weapon.isGun || weapon.isKnife) && !weapon.isThrowables)
            {
                OwnedWeapon newWeapon = Instantiate(ownedWeaponPrefab, ownedWeaponContent);
                newWeapon.data = weapon;
                newWeapon.AssignData();
            }
        }

        DeleteAllChild(ownedThrowablesContent);
        foreach (Weapon weapon in userWeapons.weapons)
        {
            if (weapon.isThrowables)
            {
                OwnedWeapon newWeapon = Instantiate(ownedThrowablesPrefab, ownedThrowablesContent);
                newWeapon.data = weapon;
                newWeapon.AssignData();
            }
        }
    }

    void GetAllWeapons()
    {
        DeleteAllChild(buyWeaponContent);

        // Pass 1: Bought weapons first
        foreach (Weapon weapon in allWeapons.weapons)
        {
            if ((weapon.isGun || weapon.isKnife) && !weapon.isThrowables)
            {
                bool isBuyed = false;
                foreach (Weapon userWeapon in userWeapons.weapons)
                {
                    if(userWeapon.name == weapon.name)
                    {
                        isBuyed = true;
                        break;
                    }
                }

                if (isBuyed)
                {
                    BuyWeapon newWeapon = Instantiate(buyWeaponPrefab, buyWeaponContent);
                    newWeapon.data = weapon;
                    newWeapon.isBuyed = true;
                    newWeapon.AssignData();
                }
            }
        }

        // Pass 2: Not bought weapons
        foreach (Weapon weapon in allWeapons.weapons)
        {
            if ((weapon.isGun || weapon.isKnife) && !weapon.isThrowables)
            {
                bool isBuyed = false;
                foreach (Weapon userWeapon in userWeapons.weapons)
                {
                    if (userWeapon.name == weapon.name)
                    {
                        isBuyed = true;
                        break;
                    }
                }

                if(!isBuyed)
                {
                    BuyWeapon newWeapon = Instantiate(buyWeaponPrefab, buyWeaponContent);
                    newWeapon.data = weapon;
                    newWeapon.isBuyed = false;
                    newWeapon.AssignData();
                }
            }
        }
    }

    public void QuickEquipShow(string type)
    {
        if (type == "Gun")
        {
            DeleteAllChild(quickEquipContent);
            foreach (Weapon weapon in userWeapons.weapons)
            {
                if (weapon.isGun)
                {
                    QuickEquipWeapon quickWeapon = Instantiate(quickEquipPrefab, quickEquipContent);
                    quickWeapon.data = weapon;
                    quickWeapon.AssignData();
                }
            }
        }
        else if(type == "Knife")
        {
            int count = 0;
            DeleteAllChild(quickEquipContent);
            foreach (Weapon weapon in userWeapons.weapons)
            {
                if (weapon.isKnife)
                {
                    if(weapon.name != PrimaryKnife.Instance.data.name)
                    {
                        QuickEquipWeapon quickWeapon = Instantiate(quickEquipPrefab, quickEquipContent);
                        quickWeapon.data = weapon;
                        quickWeapon.AssignData();
                        count++;
                    }
                   
                }
            }

            if(count > 0)
                LobbyManager.Instance.quickEquipAnimator.SetTrigger("OPEN");
        }
    }

    public void DeleteAllChild(Transform _parent)
    {
        for (int i = _parent.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(_parent.GetChild(i).gameObject);
        }
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
    public float demage;
    public float health;
    public Cost repairCost;
    public float reloadTime;
    public string bulletType;
    public int magazineSize;
    public int bulletCount;
    public int throwablesCount;
    public int fireRate;
    public float range;
    public float freezeTime;
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
