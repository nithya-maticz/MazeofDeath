using UnityEngine;
using UnityEngine.UI;

public class Tanker : MonoBehaviour
{
    public Animator animator;
    public int health = 3;
    public int maxHealth = 3;
    public GameObject attributes;
    public Image fillImage;
    public GameObject tank;
    public GameObject blast;
    public CircleCollider2D colloider;
    public TankBlaster radius;

    void Start()
    {
        UpdateHealth();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);

            if (!attributes.activeSelf)
            {
                attributes.SetActive(true);
                animator.SetTrigger("Radius");
            }
                

            health--;
            UpdateHealth();
        }
    }

    void UpdateHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        fillImage.fillAmount = (float)health / maxHealth;

        if (fillImage.fillAmount <= 0f)
        {
           attributes.SetActive(false);
            colloider.enabled = false;
            Destroy(tank);

            blast.SetActive(true);
            animator.SetTrigger("Blast");
            radius.gameObject.SetActive(true);
           // radius.gameObject.SetActive(false);
        }
        else
        {
            animator.SetTrigger("Shoot");
        }
    }
}
