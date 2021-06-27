using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Player : MonoBehaviour
{
    public static Player sharedInstance;

    public PlayerState state = PlayerState.Alive;

    public Rigidbody2D rb;
    public int maxLives;
    public int vidas = 3;
    public float speed = 1f;
    public float fuerzaSalto = 2f;
    public bool tocandoPlataforma = false;
    public bool immunity = false;
    public float immunityTime = 2f;
    private Coroutine immunityCoroutine = null;

    public Vector2? startingTravelPosition = null;
    public Vector2? endingTravelPosition = null;
    public float jumpTraveledDistance;


    public Animator anim;
    public CapsuleCollider2D cCollider;
    public SpriteRenderer sRenderer;

    public Light2D torch;

    public bool canHideInDoor = false;
    public Transform door;
    public Animator doorAnim;
    public bool inDoor = false;
    private Coroutine doorCoroutine;

    private void Awake()
    {
        sharedInstance = this;
    }

    void Start()
    {
        if (this.rb == null)
        {
            this.rb = GetComponent<Rigidbody2D>();
        }

        if (this.cCollider == null)
        {
            this.cCollider = GetComponent<CapsuleCollider2D>();
        }

        if (this.sRenderer == null)
        {
            this.sRenderer = GetComponent<SpriteRenderer>();
        }

        this.maxLives = GameManager.sharedInstance.maxLives;
        this.vidas = this.maxLives;

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.sharedInstance.Playing() && this.state == PlayerState.Alive)
        {
            PlayerController();
        }
    }


    public void SetPlayerAnimation(PlayerAnimations animation)
    {
        switch (animation)
        {
            case PlayerAnimations.Idle:
                this.anim.SetBool("Idle", true);
                this.anim.SetBool("Walking", false);
                this.anim.SetBool("Jumping", false);
                this.anim.SetBool("Falling", false);
                break;
            case PlayerAnimations.Walking:
                this.anim.SetBool("Walking", true);
                this.anim.SetBool("Idle", false);
                break;
            case PlayerAnimations.Jumping:
                this.anim.SetBool("Jumping", true);
                this.anim.SetBool("Idle", false);
                this.anim.SetBool("Walking", false);
                this.anim.SetBool("Falling", false);
                break;
            case PlayerAnimations.Falling:
                this.anim.SetBool("Falling", true);
                this.anim.SetBool("Idle", false);
                this.anim.SetBool("Walking", false);
                this.anim.SetBool("Jumping", false);
                break;
            case PlayerAnimations.Death:
                this.state = PlayerState.Death;
                this.anim.SetTrigger("Death");
                this.anim.SetBool("Idle", false);
                this.anim.SetBool("Walking", false);
                this.anim.SetBool("Jumping", false);
                this.anim.SetBool("Falling", false);
                break;
        }
    }



    private void PlayerAnimationsController()
    {
        if (this.rb.velocity.x > 0.1f)
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
            SetPlayerAnimation(PlayerAnimations.Walking);
        }
        else if(this.rb.velocity.x < -0.1f)
        {
            this.transform.localScale = new Vector3(-1f, 1f, 1f);
            SetPlayerAnimation(PlayerAnimations.Walking);
        }
        else if(this.rb.velocity.x > -0.1f && this.rb.velocity.x < 0.1f && this.rb.velocity.y < 0.1f && this.rb.velocity.y > -0.1f)
        {
            SetPlayerAnimation(PlayerAnimations.Idle);
        }

        if (this.rb.velocity.y > 0.1f)
        {
            SetPlayerAnimation(PlayerAnimations.Jumping);
        }
        else if (this.rb.velocity.y <  -0.1f)
        {
            SetPlayerAnimation(PlayerAnimations.Falling);
        }
        else if (this.rb.velocity.y < 0.1f && this.rb.velocity.y > -0.1f)
        {
            this.anim.SetBool("Falling", false);
            this.anim.SetBool("Jumping", false);
        }
    }

    private void PlayerController()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.rb.AddForce(Vector2.left * speed * Time.deltaTime, ForceMode2D.Force);
            
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.rb.AddForce(Vector2.right * speed * Time.deltaTime, ForceMode2D.Force);
        }

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))  && (this.rb.velocity.y >= -0.01f && this.rb.velocity.y <= 0.01f) && this.tocandoPlataforma)
        {
            this.rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            SetPlayerAnimation(PlayerAnimations.Jumping);
        }

        if (Input.GetKeyDown(KeyCode.Space) && this.canHideInDoor && this.door != null && !this.inDoor && (this.rb.velocity.y >= -0.01f && this.rb.velocity.y <= 0.01f))
        {
            if (this.doorCoroutine == null)
            {
                this.doorCoroutine = StartCoroutine(Door(true));
            } 
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !this.canHideInDoor && this.door != null && this.inDoor)
        {
            if (this.doorCoroutine == null)
            {
                this.doorCoroutine = StartCoroutine(Door(false));
            }
        }


        PlayerAnimationsController();
    }


    IEnumerator Door(bool enter)
    {
        float auxTime = 1f / 60f;
        if (enter)
        {
            this.doorAnim.SetTrigger("Enter");

            this.rb.velocity = Vector2.zero;
            this.rb.bodyType = RigidbodyType2D.Static;
            this.cCollider.enabled = false;
            if (this.door.gameObject.tag == "Door")
            {
                yield return new WaitForSeconds(auxTime * 17f);
            }
            else
            {
                yield return new WaitForSeconds(auxTime * 9f);
            }
            this.inDoor = true;
            this.transform.position = new Vector3(this.door.position.x, this.transform.position.y, this.transform.position.z);
            
            this.sRenderer.enabled = false;
            this.torch.enabled = false;

            if (this.door.gameObject.tag == "Goal")
            {
                yield return new WaitForSeconds(auxTime * 11f);
                GameManager.sharedInstance.Victory();
            }

            
            
        }
        else
        {
            this.doorAnim.SetTrigger("Exit");
            yield return new WaitForSeconds(auxTime * 17f);
            this.door = null;
            this.doorAnim = null;
            this.cCollider.enabled = true;
            this.rb.bodyType = RigidbodyType2D.Dynamic;
            this.rb.velocity = Vector2.zero;
            this.inDoor = false;
            this.sRenderer.enabled = true;
            this.torch.enabled = true;
        }

        this.doorCoroutine = null;

        yield break;
        
    }

    public void PlayerHealth(int cantidad, bool fromEnemy = false)
    {
        if (GameManager.sharedInstance.Playing())
        {
            if (cantidad < 0 && fromEnemy == true)
            {
                if (this.immunity == false)
                {
                    this.vidas += cantidad;

                    if (this.immunityCoroutine == null)
                    {
                        this.immunityCoroutine = StartCoroutine(Immunity(this.immunityTime));
                    }
                }
                else
                {
                    return;
                }
                
            }
            else
            {
                this.vidas += cantidad;
            }
            
            GameManager.sharedInstance.LivesUI(cantidad);

            

            if (this.vidas <= 0)
            {
                this.vidas = 0;
                SetPlayerAnimation(PlayerAnimations.Death);
                GameManager.sharedInstance.GameOver();
            }
            else if (this.vidas > this.maxLives)
            {
                this.vidas = this.maxLives;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            this.tocandoPlataforma = true;

            if ((this.rb.velocity.y >= -0.01f && this.rb.velocity.y <= 0.01f) && this.startingTravelPosition != null && !this.inDoor )
            {
                this.endingTravelPosition = this.transform.position;

                this.jumpTraveledDistance = Mathf.Abs(this.endingTravelPosition.Value.y - this.startingTravelPosition.Value.y);

                this.endingTravelPosition = null;
                this.startingTravelPosition = null;

                if (this.jumpTraveledDistance >= 5f && this.jumpTraveledDistance < 7.5f)
                {
                    PlayerHealth(-1);
                }
                else if (this.jumpTraveledDistance >= 7.5f && this.jumpTraveledDistance < 9.5f)
                {
                    PlayerHealth(-2);
                }
                else if(this.jumpTraveledDistance >= 9.5f)
                {
                    PlayerHealth(-10);
                }
            }

        }
    }

    IEnumerator Immunity(float time)
    {
        this.immunity = true;
        this.anim.ResetTrigger("Normal");
        this.anim.SetTrigger("Immunity");
        yield return new WaitForSeconds(time);
        this.anim.ResetTrigger("Immunity");
        this.anim.SetTrigger("Normal");
        this.immunity = false;
        this.immunityCoroutine = null;
        yield break;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform" && !this.inDoor)
        {
            this.tocandoPlataforma = false;
            if (this.startingTravelPosition == null)
            {
                this.startingTravelPosition = this.transform.position;
            }
        }
    }
}
