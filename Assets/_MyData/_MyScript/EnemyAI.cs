using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;

    Rigidbody2D rb;
    bool shouldJump;
    bool isGrounded;

    public int damage = 1;

    public int maxHealth = 3;
    int currentHealth;
    SpriteRenderer sprite;
    Color ogColor;

    [Header("Loot")]
    public List<LootItem> lootTable = new List<LootItem>();

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        sprite = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        ogColor = sprite.color; 
    }

    private void Update()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << player.gameObject.layer);

        if (isGrounded)
        {
            rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);

            RaycastHit2D gapAHead = Physics2D.Raycast(transform.position + new Vector3(direction, 0,0), Vector2.down, 2f, groundLayer);

            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, groundLayer);

            if (!groundInFront.collider && !gapAHead.collider)
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                shouldJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            Vector2 direction = (player.position - transform.position).normalized;

            Vector2 jumpDirection = direction * jumpForce;

            rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashWhite()
    {
        sprite.color = Color.white;

        yield return new WaitForSeconds(0.2f);

        sprite.color = ogColor;
    }

    void Die()
    {
        foreach (LootItem lootItem in this.lootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                this.InstantiateLoot(lootItem.itemPrefab);
            }

            break;
        }

        Destroy(gameObject);
    }

    void InstantiateLoot(GameObject loot)
    {
        GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);

        droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
