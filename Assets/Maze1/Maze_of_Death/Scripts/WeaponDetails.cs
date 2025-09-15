using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponDetails : MonoBehaviour
{
    // public Weapon data;
    public static WeaponDetails Instance;
    public GameObject detailsPage;
    public Image weaponImage;
    public TMP_Text weaponName;
    public Image healthFiller;
    public GameObject gunObj;
    public GameObject knifeObj;

    [Header("GUN DETAILS")]
    public Image fillMagazineSize;
    public TMP_Text magazineText;
    public Image fillBaseDemage;
    public TMP_Text baseDemageText;
    public Image fillFirerate;
    public TMP_Text fireRateText;
    public Image fillReloadTime;
    public TMP_Text reloadTimeText;
    public TMP_Text effectiveRange;

    [Header("Knife Details")]
    public Image fillKnifeDemage;
    public TMP_Text knifeDemageText;

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

    public void AssignData(Weapon data)
    {
        weaponName.text = data.name;


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
                    maxWidth = 735;
                    maxHeight = 461;
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


        if (data.isGun)
        {
            gunObj.SetActive(true);
            knifeObj.SetActive(false);
            // Define min/max for all stats (based on your weapon design)
            int minMagazine = 5;
            int maxMagazine = 100;

            int minDamage = 10;
            int maxDamage = 100;

            float minFireRate = 0.2f;  
            float maxFireRate = 15f;

            float minReload = 0.5f;    
            float maxReload = 5f;

           
            float magNorm = (data.magazineSize - minMagazine) / (float)(maxMagazine - minMagazine);
            fillMagazineSize.fillAmount = Mathf.Clamp01(magNorm);

           
            float dmgNorm = (data.demage - minDamage) / (float)(maxDamage - minDamage);
            fillBaseDemage.fillAmount = Mathf.Clamp01(dmgNorm);

            
            float fireRateNorm = (data.fireRate - minFireRate) / (maxFireRate - minFireRate);
            fillFirerate.fillAmount = Mathf.Clamp01(fireRateNorm);

            
            float reloadNorm = 1f - ((data.reloadTime - minReload) / (maxReload - minReload));
            fillReloadTime.fillAmount = Mathf.Clamp01(reloadNorm);

        }
        else if(data.isKnife)
        {
            int minDamage = 10;
            int maxDamage = 100;

            float dmgNorm = (data.demage - minDamage) / (float)(maxDamage - minDamage);
            fillKnifeDemage.fillAmount = Mathf.Clamp01(dmgNorm);
        }


        healthFiller.fillAmount = data.health / 100;
        detailsPage.SetActive(true);
    }
}
