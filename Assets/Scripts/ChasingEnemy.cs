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
    float _jumpTimer = Mathf.Infinity;


    [Header("Chase Speed")]
    public float catchUpSpeed;
    public float speed;


    [Header("Line State")]
    public PlayerLineState playerLineState;
    public PlayerLineState enemyLineState;
    public float lineChangeSpeed = 15f;
    public static float LINETHRESHOLD = 3f;
    public bool shouldSwitchLines;
    static float MIDDLELINE=0;
    static float LEFTLINE = -3;
    static float RIGHTLINE = 3;


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
        rb = GetComponent<Rigidbody>();

        playerRef = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();

    }

    private void Start()
    {
        SetSpeed();

        enemyLineState = PlayerLineState.middleLine;
    }

    private void SetSpeed()
    {
        speed = playerRef.GetSpeed();
        catchUpSpeed = speed * 2;
    }


    private void Update()
    {
        CheckLineStates();
        CheckGrounded();
        UpdateTimers();
        UpdateAnimator();
        SetSpeed();
    }
    private void FixedUpdate()
    {
        if (!GameStarter.instance.IsGameStarted()) return;
        //if (_health.IsPlayerDead()) { return; }

        Move();
        SwitchLine();

    }


    void CheckLineStates()
    {
        playerLineState = playerRef.GetPlayerLineState();

        if (playerLineState != enemyLineState)
        {
            // print("SWITCHHH");
            shouldSwitchLines = true;
            //SwitchLine();
        }
    }



    private void SwitchLine()
    {
        //if (!shouldSwitchLines) return;

        //shouldSwitchLines = false;

        if (playerLineState == PlayerLineState.leftLine &&  enemyLineState == PlayerLineState.middleLine)
        {
            TurnLeft(false);
        }
        else if(playerLineState==PlayerLineState.middleLine && enemyLineState == PlayerLineState.leftLine)
        {
            TurnRight(true);
        }
        else if(playerLineState == PlayerLineState.middleLine && enemyLineState == PlayerLineState.rightLine)
        {
            TurnLeft(true);
        }
        else if (playerLineState==PlayerLineState.rightLine && enemyLineState == PlayerLineState.middleLine)
        {
            TurnRight(false);
        }

        //switch (playerLineState)
        //{
        //    case PlayerLineState.leftLine:
        //        StartCoroutine(TurnLeft());
        //        enemyLineState = PlayerLineState.leftLine;

        //        print("Turn Left;");
        //        break;
        //    case PlayerLineState.middleLine:

        //        if (enemyLineState == PlayerLineState.leftLine)
        //        {
        //            //StartCoroutine(TurnRight());
        //        }
        //        else
        //        {
        //            StartCoroutine(TurnLeft());
        //        }
        //        print("Middle");
        //        enemyLineState = PlayerLineState.middleLine;

        //        break;
        //    case PlayerLineState.rightLine:



        //        float targetX = LINETHRESHOLD;


        //        // Get the current position
        //        Vector3 currentPosition = transform.position;

        //        float newX = Mathf.MoveTowards(currentPosition.x, targetX, lineChangeSpeed * Time.deltaTime);

        //        transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);





        //        enemyLineState = PlayerLineState.rightLine;
        //        print("Right,");
        //        break;
        //    default:
        //        break;



        
    }

    //private IEnumerator TurnRight()
    //{

    //}


    //private IEnumerator TurnLeft()
    //{

    //    Vector3 temp = transform.position;
    //    float xPos = temp.x;
    //    while (true)
    //    {
    //        if (temp.x == xPos - LINETHRESHOLD)
    //        {
    //            break;
    //        }

    //        temp.x = Mathf.MoveTowards(temp.x, xPos - LINETHRESHOLD, lineChangeSpeed * Time.deltaTime);
    //        transform.position = temp;

    //        yield return null;
    //    }


    //    print("Left içi");

    //}

    private void TurnLeft(bool turnLeftForMiddle)
    {
        if (turnLeftForMiddle)
        {
            float targetX = MIDDLELINE;


            Vector3 currentPosition = transform.position;
            print("Turn left middle currentpos.x= " + currentPosition.x + " targetx= " + MIDDLELINE);

            float newX = Mathf.MoveTowards(currentPosition.x, targetX, lineChangeSpeed * Time.deltaTime);

            transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);

            if (targetX == currentPosition.x)
            {
                enemyLineState = PlayerLineState.middleLine;

            }
        }
        else
        {


            float targetX = LEFTLINE;

            Vector3 currentPosition = transform.position;

            float newX = Mathf.MoveTowards(currentPosition.x, targetX, lineChangeSpeed * Time.deltaTime);

            transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);


            if (targetX == currentPosition.x)
            {
                enemyLineState = PlayerLineState.leftLine;

            }


        }

          
    } 
    
    private void TurnRight(bool turnRightForMiddle)
    {
        if (turnRightForMiddle)
        {
            float targetX = MIDDLELINE;

            Vector3 currentPosition = transform.position;
            print("Turn right middle currentpos.x= " + currentPosition.x + " targetx= " + MIDDLELINE);

            float newX = Mathf.MoveTowards(currentPosition.x, targetX, lineChangeSpeed * Time.deltaTime);

            transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);

            if (targetX == currentPosition.x)
            {
                enemyLineState = PlayerLineState.middleLine;
            }

        }
        else
        {

            float targetX = RIGHTLINE;

            Vector3 currentPosition = transform.position;

            float newX = Mathf.MoveTowards(currentPosition.x, targetX, lineChangeSpeed * Time.deltaTime);

            transform.position = new Vector3(newX, currentPosition.y, currentPosition.z);

            if (targetX == currentPosition.x)
            {
                enemyLineState = PlayerLineState.rightLine;
            }

        }

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


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("ObstacleGround"))
        {
            moveDir.y = 0;
        }
    }
    public void StartJump(float jumpForce)
    {
        _jumpTimer = 0;
        print("Started Jump");
        StartCoroutine(Jump());
    }

    IEnumerator Jump()
    {

        while (true)
        {

            if (actionDelay <= _jumpTimer)
            {
                moveDir.y = jumpForce;
                _jumpTimer = 5f;
                print("Jump");
                break;
            }
            _jumpTimer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(groundCheck.position, groundCheck.up * -groundCheckDistance);
    }


}
