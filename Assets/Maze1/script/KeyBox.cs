using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeyBox : MonoBehaviour
{
    public SpriteRenderer boxsprite;
    CircleCollider2D _circleCollider;

  
    public int TotalSeconds = 5;
    
    public int FillSeconds = 0;

    public BoxObjects SelectedObject;
    public Image fillImage;

    private Coroutine currentCoroutine = null;
    public GameObject CollideCircle;
    public GameObject FillCanvas;

    void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerRange"))
        {
            
            if (currentCoroutine == null)
            {
                currentCoroutine = StartCoroutine(OpenBox());
            }
        }
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerRange"))
        {
            if (currentCoroutine != null)
            {
                FillCanvas.SetActive(false);
                CollideCircle.SetActive(false);
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
                
                fillImage.fillAmount = 0;
                FillSeconds = 0;
            }
        }
    }

    IEnumerator OpenBox()
    {
        FillCanvas.SetActive(true);
        CollideCircle.SetActive(true);
        float timer = 0f;
        while (timer < TotalSeconds)
        {
            timer += Time.deltaTime;
            
            FillSeconds = Mathf.FloorToInt(timer);
            
            fillImage.fillAmount = Mathf.Clamp01(timer / TotalSeconds);
            yield return null;
        }

        Debug.Log("Filled");
        BoxOpened();
        currentCoroutine = null;
    }

    void BoxOpened()
    {
        FillCanvas.SetActive(false);
        CollideCircle.SetActive(false);
        boxsprite.sprite = ManagerMaze.instance.SpriteBoxOpen;
        _circleCollider.enabled = false;
        ManagerMaze.instance.GetTreasure(SelectedObject);
    }


}

[Serializable]
public enum BoxObjects
{
    Key,
    MediKit,
    Empty
}
