using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EmptyBox : MonoBehaviour,IGetTreasure
{
    [SerializeField] SpriteRenderer _sprite;
    int TotalSeconds = 5;
    int FillSeconds = 0;
    [SerializeField] Image fillImage;

    private Coroutine currentCoroutine = null;
    [SerializeField] GameObject CollideCircle;
    [SerializeField] GameObject FillCanvas;

    public bool IsOpened { get; set; } = false;

    void Start()
    {
        Game_Manager.Instance.BoxCountInt += 1;
    }

    private void Awake()
    {

        
    }

    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerRange") && !IsOpened)
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

        BoxOpened();
        currentCoroutine = null;
    }



    public void GetTreasure()
    {

        //
    }

    public void BoxOpened()
    {
        IsOpened = true;
        _sprite.sprite = Game_Manager.Instance.SpriteBoxOpen;
        Game_Manager.Instance.BoxCountInt -= 1;
        Game_Manager.Instance.BoxCount();
        /* Game_Manager.Instance.GetMysteryImage.sprite = Game_Manager.Instance.MedikitSprite;
         Game_Manager.Instance.curretTreasure = this;
         Game_Manager.Instance.GetMysteryImage.gameObject.SetActive(true);
         Game_Manager.Instance.GetMysteryPage.gameObject.SetActive(true);
         Game_Manager.Instance.GetMysteryPage.SetTrigger("Play");*/
        FillCanvas.SetActive(false);
        CollideCircle.SetActive(false);

    }
}
