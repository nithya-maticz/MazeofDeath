using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrimaryKnife : MonoBehaviour
{
    public static PrimaryKnife Instance;
    public Weapon data;
    public Image weaponImage;
    public TMP_Text name;
 
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

            name.text = data.name;
            
        }
    }
}
