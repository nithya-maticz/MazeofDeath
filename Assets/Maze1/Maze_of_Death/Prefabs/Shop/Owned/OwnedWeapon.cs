using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OwnedWeapon : MonoBehaviour
{
    public Weapon data;
    public Image weaponImage;
    public TMP_Text name;
    public TMP_Text bulletsText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignData()
    {
        foreach(WeaponImages img in ShopManager.Instance.weaponSprites)
        {
            if(img.name == data.name)
            {
                weaponImage.sprite = img.weaponSprite;
                break;
            }
        }

        name.text = data.name;

        //bulletsText.text = 
    }
}
