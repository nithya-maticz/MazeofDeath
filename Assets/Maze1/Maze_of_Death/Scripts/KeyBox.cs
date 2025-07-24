using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeyBox : MonoBehaviour,IGetTreasure
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
            textImg.SetActive(true);
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
        Game_Manager.Instance.IsGetKey = true;
        Game_Manager.Instance.KeyImage.SetActive(true);
        Game_Manager.Instance.KeyImage.SetActive(true);
       
    }

   public void BoxOpened()
   {
        textImg.SetActive(false);
        IsOpened = true;
       
        blackImage.SetActive(true);
        knifeTaken = true; ;
        animatorRef.SetTrigger("heart");
    }

    public void EndAnimation()
    {
        if(knifeTaken)
        {
            Box.SetActive(true);
        }
       
        //box.sprite = boxbroke;
    }
}
