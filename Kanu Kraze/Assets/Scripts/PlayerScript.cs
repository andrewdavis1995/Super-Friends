using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class PlayerScript : MonoBehaviour
{

    public GameControllerScript Controller;

    // components
    public SpriteRenderer[] Renderers;

    Animator[] animators; // 0 = close arm, 1 = far arm
    public Rigidbody2D RigidBody;
    Quaternion rotation;
    public Transform LegCollider;

    // status 
    public static bool MovingLeft = false;
    public bool onGround = false;
    public float Health = 100;
    public bool Camera = true;

    public bool Active = false;
    public bool Alive = true;

    public JohnScript JohnScript;
    public FollowScript FollowScript;

    // attributes
    float WALK_SPEED = 2.7f;
    float JUMP_SPEED = 1000;
    public float LEFT_LIMIT;
    float _lastPos = 1000;

    public bool Walking = true;


    // misc
    public float DistToGround;



    // Use this for initialization
    void Start()
    {
        RigidBody = transform.GetComponent<Rigidbody2D>();
        rotation = transform.rotation;
        animators = transform.GetComponentsInChildren<Animator>();

        LegCollider = GetComponentsInChildren<Transform>().Where(t => t.gameObject.name == "legs").FirstOrDefault();

        //_legCollider = GetComponent<Transform>();
        DistToGround = LegCollider.GetComponent<Collider2D>().bounds.extents.y;

        Renderers = GetComponentsInChildren<SpriteRenderer>();

        DisablePlayerCollisions();
    }

    private void DisablePlayerCollisions()
    {
        var currentColliders = GetComponentsInChildren<BoxCollider2D>();

        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var playerColliders = GetComponentsInChildren<BoxCollider2D>();
            foreach (var pC in playerColliders)
            {
                foreach (var cC in currentColliders)
                {
                    Physics2D.IgnoreCollision(cC, pC);
                }
            }
        }
    }

    public void Follow(PlayerScript target)
    {
        Active = false;
        // enable following
        FollowScript.enabled = true;
        FollowScript.SetTarget(target);
    }

    public void BeginWalk()
    {
        Walking = true;
        animators[2].SetTrigger("Walk");
        animators[1].SetTrigger("Walk");
        animators[0].SetTrigger("Walk");
    }

    public void Activate()
    {
        // disable following
        FollowScript.Reset();
        // set follow target to -9999
        Active = true;
    }

    public void StopWalk()
    {
        Walking = false;
        animators[2].SetTrigger("Stop");
        animators[1].SetTrigger("Stop");
        animators[0].SetTrigger("Stop");
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = rotation;

        if (Active)
        {
            if (transform.position.x < LEFT_LIMIT)
            {
                transform.position = new Vector3(LEFT_LIMIT, transform.position.y, -5);
            }

            var currPos = transform.position;

            _lastPos = currPos.y;

            var dampener = 1f;

            if (JohnScript != null && JohnScript.Climbing)
            {
                dampener = .5f;
                JohnScript.ClimberAnim.enabled = true;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                if(!Walking)
                //if (onGround)
                {
                    BeginWalk();
                }

                if (Input.GetKey(KeyCode.D))
                {
                    transform.Translate(new Vector2(dampener * WALK_SPEED * Time.deltaTime, 0));
                    foreach (var r in Renderers)
                    {
                        r.flipX = false;
                    }

                    MovingLeft = false;

                }
                else if (Input.GetKey(KeyCode.A))
                {
                    transform.Translate(new Vector2(dampener * -WALK_SPEED * Time.deltaTime, 0));
                    foreach (var r in Renderers)
                    {
                        r.flipX = true;
                    }

                    MovingLeft = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (onGround)
                {
                    Jump(1 * dampener);
                }
            }

            if (!onGround)
            {
                StartCoroutine(JumpAnim());
            }

            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                if (Walking)
                    StopWalk();

                if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                {
                    if (JohnScript != null)
                    {
                        JohnScript.ClimberAnim.enabled = false;
                    }
                }

            }
        }
    }

    private IEnumerator JumpAnim()
    {
        yield return new WaitForEndOfFrame();
        //animators[0].SetTrigger("Jump");
        //animators[2].SetTrigger("Jump");
    }


    public void Jump(float scale)
    {
        CheckGrounded();

        // TODO Jump animation
        if (onGround)
        {
            RigidBody.AddForce(new Vector2(0, JUMP_SPEED * scale));
            //animators[0].SetTrigger("Jump");
            //animators[1].SetTrigger("Jump");
            onGround = false;
        }
    }

    void CheckGrounded()
    {
        if (onGround)
        {
            var touching = Physics2D.RaycastAll(LegCollider.position, new Vector2(0, -1), 1.4f + DistToGround);

            var vari = false;
            if (touching.Length > 0)
            {
                foreach (var t in touching)
                {
                    //if ((t.distance - distToGround) < 0.70055f)
                    if (t.collider.name != "legs")
                    {
                        vari = true;
                    }
                }
            }

            if (!vari)
            {
                onGround = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.transform.tag == "Coin")
        //{
        //    GameObject.Destroy(collision.gameObject);
        //    GameManager.CoinsCollected(1);
        //}

        if (collision.transform.tag == "Death")
        {
            Die();
        }

        //if (collision.transform.tag == "Enemy")
        //{
        //    // update health
        //    var scr = collision.gameObject.GetComponent<BombScript>();
        //    if (scr && scr.Exploding) { Health -= 15; } else { Health -= 2; }


        //    rigidBody.AddForce(new Vector2(JUMP_SPEED *0.1f, JUMP_SPEED * 0.1f));

        //    GameManager.UpdateHealth(Health);

        //    if (Health < 1)
        //    {
        //        MovementAllowed = false;
        //        Camera = false;
        //        Ended = true;
        //        GameManager.Lost();
        //        foreach (var ren in _renderers) { ren.enabled = false; }
        //        enabled = false;
        //        // show popup
        //    }
        //}
    }

    private void Die()
    {
        Alive = false;

        var player = Controller.NextPlayer();

        if (!Controller.Players.Any(p => p.Alive))
        {
            // UH OH! Game over
            Debug.Log("GAME OVER");
            return;
        }

        while (!player.Alive)
        {
            player = Controller.NextPlayer();
        }
        player.Activate();
        Controller.CameraScript.ChangePlayer(Controller.Players[Controller.SelectedPlayer]);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var vel = collision.relativeVelocity;
        if (collision.gameObject.tag.ToLower().Contains("platform")) //  && vel.y < 0
        {
            onGround = true;
        }

        if (collision.transform.tag == "BouncyPlatform" && vel.y < 0)
        {
            var anim = collision.transform.GetComponent<Animator>();
            anim.SetTrigger("Land");
            Jump(1.4f);
            anim.SetTrigger("Stop");
        }
    }
}