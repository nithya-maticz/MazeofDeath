using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeyBox : MonoBehaviour,IGetTreasure
{
    [SerializeField] SpriteRenderer _sprite;
    CircleCollider2D _circleCollider;
    int TotalSeconds = 5;
    int FillSeconds = 0;
    public Image fillImage;

    private Coroutine currentCoroutine = null;
    public GameObject CollideCircle;
    public GameObject FillCanvas;

    public bool IsOpened { get; set; } = false;

    void Start()
    {
       
    }

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        Game_Manager.Instance.Boxes.Add(this);
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

    

    public void GetTreasure(int count)
    {
        PlayerAttributes.Instance.IsGetKey = true;
        Game_Manager.Instance.KeyImage.SetActive(true);
       
    }

   public void BoxOpened()
   {
        IsOpened = true;
        _sprite.sprite = Game_Manager.Instance.SpriteBoxOpen;
        Game_Manager.Instance.GetMysteryImage.sprite = Game_Manager.Instance.KeySprite;
        Game_Manager.Instance.curretTreasure = this;
        Game_Manager.Instance.GetMysteryImage.gameObject.SetActive(true);
        Game_Manager.Instance.GetMysteryPage.gameObject.SetActive(true);
        Game_Manager.Instance.GetMysteryPage.SetTrigger("Play");

   }
}
