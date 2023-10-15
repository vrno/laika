using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    Animator anim;
    private bool ActionPressed = false;
    private float ActionDecayTime;

    [SerializeField] private AudioSource barksoundclip;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GameObject.Find("PlayerObj").GetComponent<Animator>();
    }

    private void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();

        SpeedControl();

        //handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        ResetSitTimer();

        animMove();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void animMove()
    {
        //ganti animator jalan

        if (horizontalInput !=0 || verticalInput !=0)
        {
            anim.SetBool("stat_jalan", true);
        }
        else
        {
            anim.SetBool("stat_jalan", false);
        }

        //ganti animator duduk

        if (Input.GetKeyDown("space") && !anim.GetBool("stat_duduk") && !ActionPressed)
        {
            //disabling multiple input
            ActionDecayTime = 0.5f;
            ActionPressed = true;

            //membuat duduk
            anim.SetBool("stat_duduk", true);
            moveSpeed = 0f;
        }
        else if (Input.GetKeyDown("space") && anim.GetBool("stat_duduk") && !ActionPressed || horizontalInput != 0 || verticalInput != 0 || Input.GetMouseButtonDown(0))
        {
            anim.SetBool("stat_duduk", false);
            moveSpeed = 9f;
        }

        //ganti animator bark

        if (Input.GetMouseButtonDown(0) && !ActionPressed)
        {
            //disabling multiple input
            ActionDecayTime = 1f;
            ActionPressed = true;

            //membuat bark
            anim.SetTrigger("stat_bark");

            //adding sounds
            barksoundclip.Play();
        }
    }

    private void ResetSitTimer()
    {
        if (ActionPressed && ActionDecayTime > 0)
        {
            ActionDecayTime -= Time.deltaTime;
        }

        if (ActionDecayTime < 0)
        {
            ActionDecayTime = 0;
            ActionPressed = false;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        //calculating movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}
