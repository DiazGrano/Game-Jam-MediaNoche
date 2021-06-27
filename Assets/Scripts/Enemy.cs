using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int direction = 1;

    public float speed = 10f;

    public int damage = 1;

    public Rigidbody2D rb;


    private void Start()
    {
        if (Random.value <= 0.5f)
        {
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        this.direction *= -1;
        this.transform.localScale = new Vector3(this.direction, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.sharedInstance.Playing())
        {
            this.rb.velocity = Vector2.right * direction * speed * Time.deltaTime;
            //this.rb.AddForce(Vector2.right * direction * speed * Time.deltaTime, ForceMode2D.Force);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.sharedInstance.Playing())
        {
            if (collision.gameObject.tag == "Player")
            {
                Player.sharedInstance.PlayerHealth(-this.damage, true);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.sharedInstance.Playing())
        {
            if (collision.tag == "Wall")
            {
                ChangeDirection();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManager.sharedInstance.Playing())
        {
            if (collision.tag == "Platform")
            {
                ChangeDirection();
            }
        }
    }
}
