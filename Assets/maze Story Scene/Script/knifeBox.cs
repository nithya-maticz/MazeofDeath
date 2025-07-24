using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class knifeBox : MonoBehaviour
{
    [SerializeField] SpriteRenderer _sprite;
    int TotalSeconds = 5;
    int FillSeconds = 0;
    [SerializeField] Image fillImage;
    public GameObject blackImage;

    private Coroutine currentCoroutine = null;
    [SerializeField] GameObject CollideCircle;
    [SerializeField] GameObject FillCanvas;
    public GameObject Box;
   // public GameObject key;
    public bool knifeTaken;


    public Animator animatorRef;
    public GameObject textImg;


    public bool IsOpened { get; set; } = false;

    void Start()
    {
        blackImage.SetActive(false);
    }

    private void Awake()
    {

        // Game_Manager.Instance.Boxes.Add(this);
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
        textImg.SetActive(true);
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



    public void BoxOpened()
    {
        textImg.SetActive(false);
        IsOpened = true;
        Box.SetActive(true);
        blackImage.SetActive(true);
        animatorRef.SetTrigger("knife");
    }

    public void EndAnimation()
    {
        
        knifeTaken = true;
        Invoke("textfun", 2f);
        //box.sprite = boxbroke;
    }
    public void textfun()
    {
        FindObjectOfType<StoryManager>().FillCanvas1.SetActive(false);
        textImg.SetActive(false);

    }
}
