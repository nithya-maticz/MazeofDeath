using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickEquipWeapon : MonoBehaviour
{
    public Weapon data;
    public Image weaponImage;
    public TMP_Text name;
    [SerializeField] Button ClickButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ClickButton.onClick.AddListener(Click);
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

                float maxWidth = 0;
                float maxHeight = 0;

                if (data.isGun || data.isKnife)
                {
                    maxWidth = 258;
                    maxHeight = 69;
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
    }

    /*void Click()
    {
        if(ShopManager.Instance.currentSelection == "primary")
        {
            PrimaryGun.Instance.data = data;
            PrimaryGun.Instance.AssignData();
        }
        else if(ShopManager.Instance.currentSelection == "secondary")
        {
            SecondaryGun.Instance.data = data;  
            SecondaryGun.Instance.AssignData();
        }
        else if(ShopManager.Instance.currentSelection == "knife")
        {
            foreach(Weapon weapon in ShopManager.Instance.userEquipedWeapons.weapons)
            {
                if(weapon.isKnife)
                {
                    ShopManager.Instance.userEquipedWeapons.weapons.Remove(weapon);
                }
            }

            ShopManager.Instance.userEquipedWeapons.weapons.Add(data);
            PrimaryKnife.Instance.data = data;
            PrimaryKnife.Instance.AssignData();
        }

        LobbyManager.Instance.quickEquipAnimator.SetTrigger("CLOSE");
    }*/

    void Click()
    {
        if (ShopManager.Instance.currentSelection == "primary")
        {
            PrimaryGun.Instance.data = data;
            PrimaryGun.Instance.AssignData();
        }
        else if (ShopManager.Instance.currentSelection == "secondary")
        {
            SecondaryGun.Instance.data = data;
            SecondaryGun.Instance.AssignData();
        }
        else if (ShopManager.Instance.currentSelection == "knife")
        {
            // ✅ Remove any previously equipped knife(s) before adding the new one
            ShopManager.Instance.userEquipedWeapons.weapons.RemoveAll(w => w.isKnife);

            ShopManager.Instance.userEquipedWeapons.weapons.Add(data);
            PrimaryKnife.Instance.data = data;
            PrimaryKnife.Instance.AssignData();
        }

        LobbyManager.Instance.quickEquipAnimator.SetTrigger("CLOSE");
    }

}
