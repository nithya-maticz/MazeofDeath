using UnityEngine;

public class KeyFrameFunctions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    void MysteryBox()
    {
        Game_Manager.Instance.GetMysteryPage.gameObject.SetActive(false);
        Game_Manager.Instance.GetMysteryImage.gameObject.SetActive(false);
        Game_Manager.Instance.GetMysteryImage.gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
        Game_Manager.Instance.curretTreasure.GetTreasure();
    }
}
