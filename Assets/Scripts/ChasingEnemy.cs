using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : MonoBehaviour
{



    [Header("Ground Check")]
    public Transform groundCheck;
    bool isGrounded;
    public float groundCheckDistance;

    [Header("Jump")]
    public float jumpForce = 9f;
    public float gravityForce = -20f;


    [Header("Chase Speed")]
    public float catchUpSpeed;
    public float speed;


    [Header("Line State")]
    public PlayerLineState playerLineState;
    public PlayerLineState enemyLineState;
    public float lineChangeSpeed = 15f;
    public static float LINETHRESHOLD = 3f;
    public bool shouldSwitchLines;


    [Header("Spawn Variables")]
    public float distanceToStop = 2.5f;
    public float teleportDistance = 10f;

    public float actionDelay = .5f;
    float _delayTimer = Mathf.Infinity;
    Animator anim;
    Rigidbody rb;
    PlayerMovement playerRef;
    Vector3 moveDir;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb=GetComponent<Rigidbody>();
        
        playerRef = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();

    }

    private void Start()
    {
        speed = playerRef.GetSpeed();
        catchUpSpeed = speed * 2;
        
        enemyLineState = PlayerLineState.middleLine;
    }

    private void FixedUpdate()
    {
        if (!GameStarter.instance.IsGameStarted()) return;
        //if (_health.IsPlayerDead()) { return; }

        Move();
    }

    private void Update()
    {
        CheckGrounded();
        CheckLineStates();
        UpdateTimers();
        UpdateAnimator();
    }

    void CheckLineStates()
    {
       playerLineState = playerRef.GetPlayerLineState();

        if (playerLineState != enemyLineState)
        {
            print("SWITCHHH");
            shouldSwitchLines = true;
            //SwitchLine();
        }
    }



    private void SwitchLine()
    {
        //switch (playerLineState)
        //{
        //    case PlayerLineState.leftLine:
        //        TurnLeft();
        //        break;
        //    case PlayerLineState.middleLine:

        //        if (enemyLineState == PlayerLineState.leftLine)
        //        {
        //            TurnRight();
        //        }
        //        else
        //        {
        //            TurnLeft();
        //        }
        //        break;
        //    case PlayerLineState.rightLine:
        //        TurnRight();
        //        break;
        //    default:
        //        break;
        //}
    }

    private void TurnRight()
    {
        Vector3 temp = transform.position;
        float xPos = temp.x;
        temp.x = Mathf.MoveTowards(temp.x, xPos + LINETHRESHOLD, lineChangeSpeed * Time.deltaTime);
        transform.position = temp;
    }

    private void TurnLeft()
    {

        Vector3 temp = transform.position;
        float xPos = temp.x;
        temp.x = Mathf.MoveTowards(temp.x, xPos - LINETHRESHOLD, lineChangeSpeed * Time.deltaTime);
        transform.position = temp;
    }


    void Move()
    {
        moveDir.z = speed;

        rb.velocity = moveDir;

    }


    void UpdateTimers()
    {
        _delayTimer += Time.deltaTime;
    }

    void UpdateAnimator()
    {
        anim.SetBool("isGrounded", isGrounded);

    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance);

        if (!isGrounded)
        {
            moveDir.y += gravityForce * Time.deltaTime;
        }
        else
            moveDir.y = 0;

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(groundCheck.position, groundCheck.up * -groundCheckDistance);
    }

}
