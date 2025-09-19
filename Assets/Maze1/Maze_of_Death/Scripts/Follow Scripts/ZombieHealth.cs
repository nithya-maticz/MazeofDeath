using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZombieHealth : MonoBehaviour,IGetBulletDemage,IGetBlastTank, IGetKnifeDemage
{
    [SerializeField] SharedPathFollower enemy;

    [Header("Health")]
    public int Health = 30;
    public int maxHealth = 30;


    public GameObject HealthParent;
    public Image FillHealth;
    private Coroutine hideHealthCoroutine;



    void Start()
    {
        enemy = GetComponent<SharedPathFollower>();
        UpdateHealthUI();
        HealthParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetBullet(int demage)
    {
        Health = Health - demage;
        HealthUpdate();
    }

    public void HealthUpdate()
    {
        if (Health <= 0)
        {
            Instantiate(Game_Manager.Instance.BloodPrefab, transform.position, Quaternion.identity);
            Game_Manager.Instance.Enemies.Remove(enemy);
            Destroy(gameObject);
            Game_Manager.Instance.EnemyCount();
            return;
        }

        UpdateHealthUI();
        HealthParent.SetActive(true);

        if (hideHealthCoroutine != null)
            StopCoroutine(hideHealthCoroutine);

        hideHealthCoroutine = StartCoroutine(HideHealthAfterDelay());

        if (!enemy.playerDetected)
        {
            enemy.playerDetected = true;
            enemy.playerLostTime = Time.time + 10f;
        }
    }

    private void UpdateHealthUI() =>
       FillHealth.fillAmount = (float)Health / maxHealth;

    private IEnumerator HideHealthAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        HealthParent.SetActive(false);
        hideHealthCoroutine = null;
    }


    public void KnifeDemage(KnifeAttack knifeAttack, int demage)
    {
        Health = Health - demage;
        HealthUpdateByKnife(knifeAttack);
    }

    public void HealthUpdateByKnife(KnifeAttack knifeAttack)
    {
        if (Health <= 0)
        {
            Instantiate(Game_Manager.Instance.BloodPrefab, transform.position, Quaternion.identity);
            Game_Manager.Instance.Enemies.Remove(enemy);
            Destroy(gameObject);
            knifeAttack.IsStayEnemy = false;
            knifeAttack.collider.enabled = false;
            knifeAttack.collider.enabled = true;
            Game_Manager.Instance.EnemyCount();
            return;
        }

        UpdateHealthUI();
        HealthParent.SetActive(true);

        if (hideHealthCoroutine != null)
            StopCoroutine(hideHealthCoroutine);

        hideHealthCoroutine = StartCoroutine(HideHealthAfterDelay());

        if (!enemy.playerDetected)
        {
            enemy.playerDetected = true;
            enemy.playerLostTime = Time.time + 10f;
        }
    }
    public void TankBlastUpdate()
    {
        Health -= 3;

        if (Health <= 0)
        {
            Instantiate(Game_Manager.Instance.BloodPrefab, transform.position, Quaternion.identity);
            Game_Manager.Instance.Enemies.Remove(enemy);

            for (int i = 0; i < Game_Manager.Instance.patrolAreas.Count; i++)
            {
                if (Game_Manager.Instance.patrolAreas[i]._id == enemy.patrolArea._id)
                {
                    Game_Manager.Instance.patrolAreas[i].isOccupied = false;
                    break;
                }
            }

            Destroy(gameObject);
            Game_Manager.Instance.EnemyCount();
            return;
        }

        UpdateHealthUI();
        HealthParent.SetActive(true);

        if (hideHealthCoroutine != null)
            StopCoroutine(hideHealthCoroutine);

        hideHealthCoroutine = StartCoroutine(HideHealthAfterDelay());
    }

    
}
