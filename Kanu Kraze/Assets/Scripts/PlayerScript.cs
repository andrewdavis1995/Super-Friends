using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{

    public GameControllerScript Controller;

    public Image StatusImage;
    public Sprite[] StatusImages;   // inactive, active, dead

    // components
    public SpriteRenderer[] Renderers;

    Animator[] animators; // 0 = close arm, 1 = far arm, 2 legs
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

    public Sprite[] PunchImages;

    public bool Walking = true;

    private List<GameObject> InPunchRange = new List<GameObject>();
    private GameObject ActiveButton = null;

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
        if (Alive)
            StatusImage.sprite = StatusImages[0];
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
        StatusImage.sprite = StatusImages[1];
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
                if (!Walking)
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

            HandlePunch();
            HandleButtonPush();
        }
    }

    private void HandleButtonPush()
    {
        if (Input.GetKeyDown(KeyCode.E) && ActiveButton != null)
        {
            ActiveButton.GetComponent<ButtonScript>().Press();
            ActiveButton = null;
        }
    }

    private bool _punching = false;

    private void HandlePunch()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_punching)
                StartCoroutine(DoPunch());
        }
    }

    IEnumerator DoPunch()
    {
        _punching = true;
        animators[1].enabled = false;

        for (var i = 0; i < InPunchRange.Count; i++)
        {
            var impact = InPunchRange[i];

            // get enemy and remove health
            // get statue
            var citizen = impact.GetComponent<CitizenScript>();
            if (citizen != null)
            {
                var destroyed = citizen.Hit(15);
                if (destroyed) InPunchRange.Remove(impact);
            }
        }

        // loop through images
        for (int i = 0; i < PunchImages.Length; i++)
        {
            Renderers[3].sprite = PunchImages[i];
            yield return new WaitForSeconds(.025f);
        }

        animators[1].enabled = true;
        _punching = false;
        yield return new WaitForSeconds(0);
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
        InPunchRange.Remove(collision.gameObject);

        if (collision.transform.tag == "Button")
        {
            if (!collision.gameObject.GetComponent<ButtonScript>().Pressed)
            {
                ActiveButton = null;
                collision.gameObject.GetComponent<ButtonScript>().Instruction.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        InPunchRange.Add(collision.gameObject);

        if (collision.transform.tag == "Button")
        {
            if (!collision.gameObject.GetComponent<ButtonScript>().Pressed)
            {
                ActiveButton = collision.gameObject;
                collision.gameObject.GetComponent<ButtonScript>().Instruction.SetActive(true);
            }
        }

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

        if (!Controller.Players.Any(p => p.Alive))
        {
            // UH OH! Game over
            Debug.Log("GAME OVER");
            StatusImage.sprite = StatusImages[2];
            return;
        }

        if (Active)
        {
            var player = Controller.NextPlayer();

            foreach (var pl in Controller.Players)
            {
                pl.FollowScript.SetTarget(null);
            }

            while (!player.Alive)
            {
                player = Controller.NextPlayer();
            }
            player.Activate();
        }
        Controller.CameraScript.ChangePlayer(Controller.Players[Controller.SelectedPlayer]);
        StatusImage.sprite = StatusImages[2];
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var vel = collision.relativeVelocity;
        if (collision.gameObject.tag.ToLower().Contains("platform")) //  && vel.y < 0
        {
            onGround = true;
            transform.parent = collision.transform;
        }

        if (collision.transform.tag == "BouncyPlatform" && vel.y < 0)
        {
            var anim = collision.transform.GetComponent<Animator>();
            anim.SetTrigger("Land");
            Jump(1.4f);
            anim.SetTrigger("Stop");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.ToLower().Contains("platform")) //  && vel.y < 0
        {
            transform.parent = null;
        }
    }

}