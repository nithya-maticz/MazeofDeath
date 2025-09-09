using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IGetBlastTank
{
    [Header("PLAYER Health")]
    public int PlayerHealthCount;
    public bool isFilling = false;

    void Start()
    {
        Game_Manager.Instance.playerHealth = this;
        PlayerHealthCount = 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TankBlastUpdate()
    {
        PlayerHealthCount -= 2;
        UpdatePlayerHealth();
    }

    void UpdatePlayerHealth()
    {
        if (PlayerHealthCount <= 0)
        {
            Game_Manager.Instance.HealthImage.gameObject.SetActive(true);
            Game_Manager.Instance.HealthImage.sprite = Game_Manager.Instance.HealthSprites[2];
            Game_Manager.Instance.GameOver();
        }
        else
        {
            switch (PlayerHealthCount)
            {
                case 1:
                    Game_Manager.Instance.HealthImage.gameObject.SetActive(true);
                    Game_Manager.Instance.HealthImage.sprite = Game_Manager.Instance.HealthSprites[1];
                    break;

                case 2:
                    Game_Manager.Instance.HealthImage.gameObject.SetActive(true);
                    Game_Manager.Instance.HealthImage.sprite = Game_Manager.Instance.HealthSprites[0];
                    break;

                case 3:
                    Game_Manager.Instance.HealthImage.gameObject.SetActive(false);
                    break;

                case 4:
                    Game_Manager.Instance.HealthImage.gameObject.SetActive(false);
                    break;

            }

        }
    }

    public void UpdateMedikitUI()
    {
        if (Game_Manager.Instance.MedikitCount <= 0)
        {
            Game_Manager.Instance.UseMediKit.interactable = false;
        }
        Game_Manager.Instance.MedikitCountText.text = Game_Manager.Instance.MedikitCount.ToString();
    }

    public IEnumerator FillMedikit()
    {
        isFilling = true;

        // Set full opacity
        SetImageAlpha(Game_Manager.Instance.MedikitFillImage, 1f);

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Game_Manager.Instance.MedikitFillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        // Done filling
        Debug.Log("Filled!");
        PlayerHealthCount = 4;
        UpdatePlayerHealth();
        SetImageAlpha(Game_Manager.Instance.MedikitFillImage, 0.2f);

        isFilling = false;
    }

    void SetImageAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    public void GetAttack(int _health)
    {
        PlayerHealthCount -= _health;
            UpdatePlayerHealth();
    }
}
