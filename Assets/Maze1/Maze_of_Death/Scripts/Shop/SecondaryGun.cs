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
    public Image weaponImage;
    public Image ButtonImage;
    public TMP_Text name;
    public TMP_Text bulletsText;


    private int currentMag;
    private int reserve;
    private int totalBullets;
    private int magazineCapacity;
    public bool isOccupied;

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
    }

    public void UnEquip()
    {
        emptyObject.SetActive(true);
        loadObject.SetActive(false);
        isOccupied = false;
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
        if(isOccupied)
        {

        }
    }
}
