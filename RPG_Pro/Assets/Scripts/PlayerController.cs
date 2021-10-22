using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public int gold;
    public int curHp;
    public int maxHp;
    public bool dead;
    public int level;
    public bool ghostpick;

    [Header("Attack")]
    public int damage;
    public float attackRange;
    public float attackRate;
    private float lastAttackTime;

    [Header("Components")]
    public Rigidbody2D rig;
    public Player photonPlayer;
    public SpriteRenderer sr;
    public Animator weaponAnim;
    public HeaderInfo headerInfo;
    public GameObject Shield;

    // local player 
    public static PlayerController me;

    [PunRPC]
    public void Initialize (Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        // initialize the health bar 
        headerInfo.Initialze(player.NickName, maxHp);
        level = 1;
        ghostpick = false;

        if (player.IsLocal)
            me = this;
        else
            rig.isKinematic = false;
    }

    void Update ()
    {
        if(!photonView.IsMine)
            return;

        Move();

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackRate)
            Attack();

        float mouseX = (Screen.width / 2) - Input.mousePosition.x;

        if (mouseX < 0)
            weaponAnim.transform.parent.localScale = new Vector3(1, 1, 1);
        else
            weaponAnim.transform.parent.localScale = new Vector3(-1, 1, 1);
    }

    void Move ()
    {
        // get horizontal and vetical inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // apply that to our velocity
        rig.velocity = new Vector2(x, y) * moveSpeed;
    }

    // melee attacks towards the mouse
    void Attack ()
    {
        lastAttackTime = Time.time;

        // calculate the direction
        Vector3 dir = (Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position)).normalized;

        // shoot a raycast in the direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir, dir, attackRange);

        // did we hit an enemy
        if(hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            // get the enemy and damage them
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            enemy.photonView.RPC("TakeDamage", RpcTarget.MasterClient, damage);
        }

        // play attack animation
        weaponAnim.SetTrigger("Attack");
    }

    [PunRPC]
    public void TakeDamage (int damage)
    {
        curHp -= damage;

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);

        if (curHp <= 0)
            Die();
        else
        {
            photonView.RPC("FlashDamage", RpcTarget.All);
        }
    }

    [PunRPC]
    void FlashDamage ()
    {
        StartCoroutine(DamageFlash());

        IEnumerator DamageFlash()
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sr.color = Color.white;
        }
    }

    void Die ()
    {
        dead = true;
        moveSpeed = 6;
        rig.isKinematic = true;

        transform.position = new Vector3(-38, -25, 0);

        Vector3 spawnPos = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));  
    }

    IEnumerator Spawn (Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        dead = false;
        if (ghostpick == false)
            moveSpeed = 3;
        else if (ghostpick == true)
            moveSpeed = 6;
        transform.position = spawnPos;
        curHp = maxHp;
        rig.isKinematic = false;

        // update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    void Heal (int amountToHeal)
    {
        curHp = Mathf.Clamp(curHp + amountToHeal, 0, maxHp);

        // update the health bar 
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    void GiveGold (int goldToGive)
    {
        gold += goldToGive;
        if (gold >= 50 && level == 1)
        {
            level = 2;
            maxHp += 10;
            damage += 2;
            curHp = maxHp;
            headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
            //Invoke("DisplayLevelText", 3.0f);
            headerInfo.DisplayLevelText();
            
        }
        else if (gold >= 150 && level == 2)
        {
            level = 3;
            maxHp += 10;
            damage += 2;
            curHp = maxHp;
            headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
            headerInfo.DisplayLevelText();
        }
        else if (gold >= 300 && level == 3)
        {
            level = 4;
            maxHp += 10;
            damage += 2;
            curHp = maxHp;
            headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
            headerInfo.DisplayLevelText();
            // GameUI.instance.SkillMenu();
        }

        // update the ui
        GameUI.instance.UpdateGoldText(gold);

    }

    [PunRPC]
    void GhostPickup (Pickup The)
    {
        ghostpick = true;
        The.gameObject.SetActive(false);
    }
}
