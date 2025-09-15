using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyWeapon : MonoBehaviour
{
    public Weapon data;

    public bool isBuyed;
    public Image weaponImage;
    public TMP_Text name;
    public TMP_Text costText;
    public GameObject ownedIcon;
    public Button buyButton;
    public GameObject goldIcon;
    public GameObject DiamondIcon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignData()
    {
        foreach (WeaponImages img in ShopManager.Instance.weaponSprites)
        {
            if (img.name == data.name)
            {
                weaponImage.sprite = img.weaponSprite;
                weaponImage.SetNativeSize();

                float maxWidth = 350f;
                float maxHeight = 150f;

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
        

        if(!isBuyed)
        {
            buyButton.gameObject.SetActive(true);
            ownedIcon.SetActive(false);
            costText.text = data.cost.value.ToString();

            if (data.cost.currency == "Gold")
                goldIcon.SetActive(true);
            else if (data.cost.currency == "Diamond")
                DiamondIcon.SetActive(true);
        }
        else
            ownedIcon.SetActive(true);
    }
}
