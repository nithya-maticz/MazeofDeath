using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryGun : MonoBehaviour
{
    public static SecondaryGun Instance;
    public PrimaryGun primaryGun;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite selectedSprite;
    public Weapon data;
    public GameObject emptyObject;
    public GameObject loadObject;
    public GameObject options;
    public Image weaponImage;
    public Image ButtonImage;
    public TMP_Text name;
    public TMP_Text bulletsText;


    private int currentMag;
    private int reserve;
    private int totalBullets;
    private int magazineCapacity;
    public bool isOccupied;
    public bool isSelected;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignData()
    {
        if(!primaryGun.isOccupied)
        {
            primaryGun.data = data;
            primaryGun.AssignData();
            ButtonImage.sprite = defaultSprite;
            isSelected = false;
            return;
        }

        emptyObject.SetActive(false);
        loadObject.SetActive(true);
        ButtonImage.sprite = defaultSprite;

        foreach (WeaponImages img in ShopManager.Instance.weaponSprites)
        {
            if (img.name == data.name)
            {

                weaponImage.sprite = img.weaponSprite;
                weaponImage.SetNativeSize();

                float maxWidth = 0;
                float maxHeight = 0;

                if (data.isGun || data.isKnife)
                {
                    maxWidth = 350f;
                    maxHeight = 150f;
                }
                else if (data.isThrowables)
                {
                    maxWidth = 175f;
                    maxHeight = 175f;
                }


                // current native size
                float width = weaponImage.rectTransform.sizeDelta.x;
                float height = weaponImage.rectTransform.sizeDelta.y;

                // scale factor to fit inside box while keeping ratio
                float scale = Mathf.Min(maxWidth / width, maxHeight / height);

                // apply new size
                weaponImage.rectTransform.sizeDelta = new Vector2(width * scale, height * scale);
                break;
            }
        }

        name.text = data.name;
        bulletsText.text = Initialize(data.bulletCount, data.magazineSize);
        isOccupied = true;
        AddEquipedWeapons();


    }

    public void UnEquip()
    {
        options.SetActive(false);
        emptyObject.SetActive(true);
        loadObject.SetActive(false);
        isOccupied = false;
        RemoveEquippedWeapon();
    }

    string Initialize(int total, int magCap)
    {
        totalBullets = Mathf.Max(0, total);
        magazineCapacity = Mathf.Max(1, magCap);

        // load the magazine as full as possible
        currentMag = Mathf.Min(magazineCapacity, totalBullets);
        reserve = totalBullets - currentMag;
        return (currentMag.ToString() + "/" + reserve.ToString());
    }

    public void Click()
    {

        primaryGun.Deselect();
        PrimaryKnife.Instance.options.SetActive(false);

        if (isOccupied)
        {
            options.SetActive(true);
           
           
            if (!isSelected)
            {
                AnimatorStateInfo state = LobbyManager.Instance.quickEquipAnimator.GetCurrentAnimatorStateInfo(0);

                if (!state.IsName("CONFIRMWEAPON")) // replace "Closed" with your actual state name
                {
                    LobbyManager.Instance.quickEquipAnimator.SetTrigger("CLOSE");
                }
            }
                
        }
        else
        {
            if (!isSelected)
            {
                ShopManager.Instance.QuickEquipShow("Gun");
                ButtonImage.sprite = selectedSprite;
                AnimatorStateInfo state = LobbyManager.Instance.quickEquipAnimator.GetCurrentAnimatorStateInfo(0);

                if (!state.IsName("SELECTWEAPON"))
                {
                    LobbyManager.Instance.quickEquipAnimator.SetTrigger("OPEN");
                }
                isSelected = true;
                ShopManager.Instance.currentSelection = "secondary";
                if(primaryGun.isSelected)
                {
                    primaryGun.isSelected = false;
                    primaryGun.ButtonImage.sprite = defaultSprite;
                }
            }
        }
    }

    void AddEquipedWeapons()
    {
        foreach (Weapon weapon in ShopManager.Instance.userEquipedWeapons.weapons)
        {
            if (weapon == data)
            {
                return;
            }
        }
        ShopManager.Instance.userEquipedWeapons.weapons.Add(data);
    }

    void RemoveEquippedWeapon()
    {
        ShopManager.Instance.userEquipedWeapons.weapons.Remove(data);
    }

    public void Deselect()
    {
        isSelected = false;
        ButtonImage.sprite = defaultSprite;
        options.SetActive(false);
    }
}
