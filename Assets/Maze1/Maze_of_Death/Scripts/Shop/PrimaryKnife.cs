using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryKnife : MonoBehaviour
{
    public static PrimaryKnife Instance;
    public Weapon data;
    public Image weaponImage;
    public TMP_Text name;
    public Button clickButton;
    public GameObject options;

    [SerializeField] Button details;
    private void Awake()
    {
        Instance = this;
        clickButton.onClick.AddListener(Click);
    }

    private void Start()
    {
        details.onClick.AddListener(() => WeaponDetails.Instance.AssignData(data));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignData()
    {
        options.SetActive(false);
        foreach (WeaponImages img in ShopManager.Instance.weaponSprites)
        {
            if (img.name == data.name)
            {
                weaponImage.sprite = img.weaponSprite;
                weaponImage.SetNativeSize();

               float maxWidth = 220;
               float  maxHeight = 150f;
              
                // current native size
                float width = weaponImage.rectTransform.sizeDelta.x;
                float height = weaponImage.rectTransform.sizeDelta.y;

                // scale factor to fit inside box while keeping ratio
                float scale = Mathf.Min(maxWidth / width, maxHeight / height);

                // apply new size
                weaponImage.rectTransform.sizeDelta = new Vector2(width * scale, height * scale);
                break;
            }

            name.text = data.name;
            
        }

        
    }

    void Click()
    {
        ShopManager.Instance.QuickEquipShow("Knife");
        ShopManager.Instance.currentSelection = "knife";
        options.SetActive(true);
        PrimaryGun.Instance.options.SetActive(false);
        SecondaryGun.Instance.options.SetActive(false);
    }

    public void RemovePrevious()
    {
        options.SetActive(false);
        foreach (Weapon weapon in ShopManager.Instance.userEquipedWeapons.weapons)
        {
            if(weapon.isKnife)
            {
                ShopManager.Instance.userEquipedWeapons.weapons.Remove(weapon);
               
            }
        }
    }
}
