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

    [Header("Roll")]
    bool isRolling;
    public float rollDownForce;

    [Header("Chase Speed")]
    public float catchUpSpeed;
    public float speed;
    public float slowingSpeed;

    [Header("Chase Variables")]
    public bool shouldChasePlayer;
    public bool isStartedChasing;
    public bool isInThreshold;
    public bool isInSlowingState;

    public bool isCatchedPlayer;
    public float playerChaseThreshold=3f;

    public float chaseDuration = 3f;
    public float chaseTimer = Mathf.Infinity;

    public float disappearThreshold = 10f;


    [Header("Line State")]
    public PlayerLineState playerLineState;
    public PlayerLineState enemyLineState;
    public float lineChangeSpeed = 15f;
    public static float LINETHRESHOLD = 3f;
    public bool shouldSwitchLines;
    static float MIDDLELINE = 0;
    static float LEFTLINE = -3;
    static float RIGHTLINE = 3;
    public float snapThreshold = 0.1f;

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
        shouldChasePlayer = true;
        isStartedChasing = true;
        enemyLineState = PlayerLineState.middleLine;
    }

   


    private void Update()
    {
        CheckLineStates();
        CheckGrounded();
        UpdateTimers();
        UpdateAnimator();
        SetSpeed();
        Chase();
    }
    private void FixedUpdate()
    {
        if (!GameStarter.instance.IsGameStarted()) return;
        //if (_health.IsPlayerDead()) { return; }

        Move();
        SwitchLine();

    }

    public void StartChase(bool v) => shouldChasePlayer = v; 

    void Chase()
    {
        if (!shouldChasePlayer) return;

        if (Vector3.Distance(transform.position, playerRef.transform.position) <= playerChaseThreshold && !isInThreshold && !isInSlowingState)
        {
            isInThreshold = true;
            chaseTimer = 0;
            print("isInThreshold=true");
            isStartedChasing = false;
        }
        
        if (isInThreshold && chaseTimer>=chaseDuration)
        {
            isInThreshold = false;
            isInSlowingState = true;
            print("isSlowing = true");

        }

        if (isInSlowingState)
        {
            if (Vector3.Distance(transform.position, playerRef.transform.position) >= disappearThreshold)
            {
                shouldChasePlayer = false;
                isInSlowingState = false;
                print("Should end now");
                gameObject.SetActive(false);
            }
        }
    }
    private void SetSpeed()
    {
       // print("DISTANCE = = "+Vector3.Distance(transform.position, playerRef.transform.position));
        speed = playerRef.GetSpeed();
        catchUpSpeed = speed * 1.2f;
        slowingSpeed = speed * .5f;
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

        if (playerLineState == PlayerLineState.leftLine && enemyLineState == PlayerLineState.middleLine)
        {
            float targetX = LEFTLINE;


            Vector3 temp = transform.position;
            temp.x = Mathf.MoveTowards(temp.x, targetX, lineChangeSpeed * Time.deltaTime);
            transform.position = temp;

            if (Mathf.Abs(targetX - transform.position.x) < snapThreshold)
            {
                transform.position = new Vector3(targetX, temp.y, temp.z);
                enemyLineState = PlayerLineState.leftLine;
            }
        }
        else if (playerLineState == PlayerLineState.middleLine && enemyLineState == PlayerLineState.leftLine)
        {
            float targetX = MIDDLELINE;


            Vector3 temp = transform.position;
            temp.x = Mathf.MoveTowards(temp.x, targetX, lineChangeSpeed * Time.deltaTime);
            transform.position = temp;

            if (Mathf.Abs(targetX - transform.position.x) < snapThreshold)
            {
                transform.position = new Vector3(targetX, temp.y, temp.z);
                enemyLineState = PlayerLineState.middleLine;
            }
        }
        else if (playerLineState == PlayerLineState.middleLine && enemyLineState == PlayerLineState.rightLine)
        {
            float targetX = MIDDLELINE;


            Vector3 temp = transform.position;
            temp.x = Mathf.MoveTowards(temp.x, targetX, lineChangeSpeed * Time.deltaTime);
            transform.position = temp;

            if (Mathf.Abs(targetX - transform.position.x) < snapThreshold)
            {
                transform.position = new Vector3(targetX, temp.y, temp.z);
                enemyLineState = PlayerLineState.middleLine;
            }
        }
        else if (playerLineState == PlayerLineState.rightLine && enemyLineState == PlayerLineState.middleLine)
        {
            float targetX = RIGHTLINE;


            Vector3 temp = transform.position;

            temp.x = Mathf.MoveTowards(temp.x, targetX, lineChangeSpeed * Time.deltaTime);

            transform.position = temp;

            if (Mathf.Abs(targetX - transform.position.x) < snapThreshold)
            {
                transform.position = new Vector3(targetX, temp.y, temp.z);
                enemyLineState = PlayerLineState.rightLine;
            }
        }
        else if (playerLineState == enemyLineState)
        {
            float targetX = 0;

            if (playerLineState == PlayerLineState.leftLine)
            {
                targetX = LEFTLINE;
            }
            else if (playerLineState == PlayerLineState.rightLine)
            {
                targetX = RIGHTLINE;
            }
            else
                targetX = MIDDLELINE;


            Vector3 temp = transform.position;
            temp.x = Mathf.MoveTowards(temp.x, targetX, lineChangeSpeed * Time.deltaTime);
            transform.position = temp;

        }
        else if (playerLineState == PlayerLineState.leftLine && enemyLineState == PlayerLineState.rightLine ||
            playerLineState == PlayerLineState.rightLine && enemyLineState == PlayerLineState.leftLine)
        {
            float targetX = 0;
            if (playerLineState == PlayerLineState.rightLine)
            {
                targetX = RIGHTLINE;
            }
            else
                targetX = LEFTLINE;

            Vector3 temp = transform.position;

            temp.x = Mathf.MoveTowards(temp.x, targetX, lineChangeSpeed * Time.deltaTime);

            transform.position = temp;

            if (Mathf.Abs(targetX - transform.position.x) < snapThreshold)
            {
                transform.position = new Vector3(targetX, temp.y, temp.z);
                enemyLineState = PlayerLineState.leftLine;
            }
        }


    }

    void Move()
    {
        if (isStartedChasing)
        {
            print("moving = ChaseSpeed");
            moveDir.z = catchUpSpeed;
        }
        else if (isInThreshold)
        {
            moveDir.z = speed;
            print("moving = normalSpeed");

        }
        else if (isInSlowingState)
        {
            moveDir.z = slowingSpeed;
            print("moving = slowingSpeed");

        }

        rb.velocity = moveDir;

    }


    void UpdateTimers()
    {
        _delayTimer += Time.deltaTime;
        chaseTimer += Time.deltaTime;
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
                anim.SetTrigger("Jump");
                break;
            }
            _jumpTimer += Time.deltaTime;
            yield return null;
        }

    }

    public void Roll()
    {
        if (isRolling && !isGrounded)
        {
            rb.AddForce(Vector3.down * rollDownForce, ForceMode.Acceleration);
        }
        else
            anim.SetTrigger("Roll");

        _jumpTimer += Time.deltaTime;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(groundCheck.position, groundCheck.up * -groundCheckDistance);
    }


}
