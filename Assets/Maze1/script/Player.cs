using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int keys = 0;
    public float speed = 5.0f;
    public Text keyAmount;
    public Text youWin;
    public Sprite keySprite;
    public GameObject door;
    public GameObject enemy;
    public GameObject keyRef;
    public bool keyTaken;
    public int keyCount;
    public TMP_Text keyCountTxt;
    public bool openDoor;
    public bool playerDeath;
    public bool reach;
    public GameObject keyHead;
    public Animator animatorRef;
    public int maxhealth = 10;
    public int health;
    public int damage;
    public bool attackEnemy;
    public bool spaceClick;
    public Joystick joystick;
    public Rigidbody2D rb;
    public float timer = 10f;
    public float remainTime;
    public TMP_Text playerLifeTxt;
    public healthbarscript healthBar;
    public Animator enemyAnimator;
    public GameObject playerCollider;
    public GameObject gameoverpage;
    public GameObject winpage;
    public GameObject player;
    public ManagerMaze managerRef;
    public bool attacks;


    // Start is called before the first frame update
    void Start()
    {
       
        health = maxhealth;
        healthBar = GetComponentInChildren<healthbarscript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(attackEnemy==true && openDoor==false)
        {
            if (timer > 0)
            {
                timer = timer - Time.deltaTime;
                remainTime = Mathf.RoundToInt(timer);
                Debug.Log(" TIME " + remainTime);
                playerLifeTxt.text = remainTime.ToString();
                if(remainTime<=0)
                {

                    animatorRef.SetTrigger("death");
                    playerDeath = true;
                    LevelUp();
                    Debug.Log("PlayerDeath" + playerDeath);
                    openDoor = false;
                }

                healthBar.UpdateHealthBar(remainTime, maxhealth);
            }
        }
        float moveH = joystick.Horizontal;
        float moveV = joystick.Vertical;
        Vector2 moveDir = new Vector2(moveH, moveV);
        rb.linearVelocity = moveDir * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "walls")
        {
            Debug.Log("wallhit");
        }
        else if (collision.gameObject.tag == "keys")
        {
            keyTaken = true;
           collision.gameObject.GetComponent<SpriteRenderer>().sprite = keySprite;
            keyRef = collision.gameObject;
            if (keyTaken && reach==false)
            {
                Debug.Log("keytaken");
                keyCount++;
                reach = true;
                keyHead.SetActive(true);
                keyCountTxt.text = keyCount.ToString();
                Destroy(keyRef);
            }
           
        }
       else  if (collision.gameObject.tag=="box")
        {
            Debug.Log("Box hit!");
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "door" && keyCount>=1)
        {
            openDoor = true;
            LevelUp();
            keyHead.SetActive(false);
            Debug.Log("door hit!");
           
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
           
            Debug.Log("Enemy hit!");
            attackEnemy = true;
          //  animatorRef.SetTrigger("damage");

        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        attacks = false;
        attackEnemy = false;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {

        Debug.Log("exit collider");
        keyTaken = false;
       
    }
    
   
    public void PlayerAttack()
    {
        if(openDoor== false && playerDeath==false)
        {
            attacks = true;
            Debug.Log("dfdsf");
            animatorRef.SetTrigger("playerattack");
        }
       
    }
    public void LevelUp()
    {
        if(openDoor )
        {
            for (int i = 0; i <= managerRef.enemys.Length-1; i++)
            {
                managerRef.enemys[i].GetComponent<Enemy>().idleFun();
            }
            player.tag = "unplayer";
            winpage.SetActive(true);
        }
        else if(playerDeath)
        {
            for(int i=0;i<= managerRef.enemys.Length-1;i++)
            {
                //  managerRef.enemys[i].GetComponent<Enemy>().idleFun();
                Destroy(managerRef.enemys[i].gameObject);
               // Destory(managerRef.enemys[i]);

            }
            player.tag = "unplayer";
            StartCoroutine(ShowdeathPageWithDelay(1f)); // 1 second delay
           
        }
    }
    IEnumerator ShowdeathPageWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameoverpage.SetActive(true);
    }
}
