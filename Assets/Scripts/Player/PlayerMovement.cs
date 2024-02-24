using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerForwardSpeed = 5f;
    [SerializeField] float slopeSpeedMultiplier = 1.2f;
    [SerializeField] float animationSpeedMultiplier = 1f;
    [SerializeField] float speedTickRate = 1f;

    [SerializeField] Transform leftLine, rightLine, middleLine;
    [SerializeField] float lineChangeSpeed = 5f;


    //FLAG
    public bool isTakingDamage = false;

    [Header("Jump")]
    public float jumpForce = 5f;
    public bool isJumping = false;
    public bool isGrounded = false;
    public KeyCode jumpKey = KeyCode.Space;
    public Transform groundCheck;
    public float raycastDistance = 0.5f;
    public LayerMask groundLayer;
    public float gravityForce = -20f;
    public float groundDamping = .9f;

    [Header("Roll")]
    public KeyCode rollKey = KeyCode.S;
    public float rollDownForce = -40f;
    public float rollCollisionHeight = .5f;
    public float rollDuration = .5f;
    public float originalHeight;
    public Vector3 originalCenter;
    public bool isRolling;

    [Header("Slopes")]

     [SerializeField] float slopeRaycastDist = .1f;
    [SerializeField] float maxSlopeAngle;
    private Vector3 slopeMoveDir;
    RaycastHit slopeHit;

    
    

    Vector3 moveDir;

    private Vector3 _lineStartPos;
    private Vector3 _lineEndPos;


    [Header("Timers")]
    float speedTimer;



    Coroutine rollCoroutine;
    CapsuleCollider _collider;
    Rigidbody _rb;
    Animator _anim;
    Health _health;
    PlayerLineState _playerLineState;
    PlayerLineState _playerPrevLineState;

    #region Monobehaviour Methods
    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _health = GetComponent<Health>();
    }
    void Start()
    {
        _playerLineState = PlayerLineState.middleLine;
        _playerPrevLineState = _playerLineState;
        originalCenter = _collider.center;
        originalHeight = _collider.height;

        rollCoroutine = StartCoroutine(RollSequence());
        StopRoll();
    }

    void Update()
    {

        IncreaseSpeed();
        CheckJump();
        CheckRoll();
        CheckSlopeDir();
        CheckGrounded();
        HandleLines();
        UpdateAnimator();
    }

    
    private void FixedUpdate()
    {
        if (!GameStarter.instance.IsGameStarted()) return;
        if (_health.IsPlayerDead()){ return; }

        ChangeLine();

        moveDir.z = playerForwardSpeed;
        if (OnSlope())
        {
            moveDir.y = 0;
            var newMovdir = GetSlopeMoveDirection();
            newMovdir.y *= slopeSpeedMultiplier;
            newMovdir.z *= slopeSpeedMultiplier;
            _rb.velocity = newMovdir;
        }
        else
            _rb.velocity = moveDir;

    }
    #endregion

    #region Roll
    private void CheckRoll()
    {
        if (Input.GetKeyDown(rollKey) && !isRolling)
        {
            isRolling = true;
            _anim.SetTrigger("Roll");
            rollCoroutine = StartCoroutine(RollSequence());

        }

        if (isRolling && !isGrounded)
        {
            _rb.AddForce(Vector3.down*rollDownForce,ForceMode.Acceleration);
        }
    }

    IEnumerator RollSequence()
    {
        _collider.height = rollCollisionHeight;


        float heightDifference = originalHeight - rollCollisionHeight;
        Vector3 newCenter = originalCenter - new Vector3(0, heightDifference / 2f, 0);
        _collider.center = newCenter;


        yield return new WaitForSeconds(rollDuration);

        isRolling = false;
        _collider.height = originalHeight;
        _collider.center = originalCenter;
    }

    void StopRoll()
    {
        StopCoroutine(rollCoroutine);

        isRolling = false;

        _collider.height = originalHeight;
        _collider.center = originalCenter;
        
    }
    #endregion
    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, slopeRaycastDist))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;

        }
        return false;
    }
    private void CheckSlopeDir()
    {
        slopeMoveDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);    
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;

    }
    private void UpdateAnimator()
    {
        _anim.SetBool("isJumping", isJumping);
        _anim.SetBool("isGrounded", isGrounded);
        _anim.SetBool("isGameStarted", GameStarter.instance.IsGameStarted());
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, raycastDistance, groundLayer);
        if (!isGrounded )
            moveDir.y += gravityForce * Time.deltaTime;

    }




    private void CheckJump()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded && !isJumping)
        {
            isJumping = true;
            StopRoll();
            Jump();
        }

    }

    private void Jump()
    {
        moveDir.y = jumpForce;
        _anim.SetTrigger("Jump");
    }

    private void IncreaseSpeed()
    {
        speedTimer += Time.deltaTime;

        if (speedTimer <= speedTickRate) return;
        //Character speed
        animationSpeedMultiplier += .002f;
        playerForwardSpeed += .02f;
        //Animatino speed
        _anim.SetFloat("speedMultiplier", animationSpeedMultiplier);
        speedTimer = 0;


    }

    void ChangeLine()
    {
        // The issiue with lerp approach is that if the player is between lines and change line again
        // character should complete changing line in fixed duratiion in my case its: 0.25 second
        // -3 to 0 and -1 to 0 changing process is completed at the same time

        // if (_playerPrevLineState == PlayerLineState.middleLine && _playerLineState == PlayerLineState.leftLine)
        //{

        //    Vector3 temp = transform.position;
        //    temp.x = Mathf.Lerp(_lineStartPos.x, _lineEndPos.x, lineCurve.Evaluate(_percentageComplete));
        //    transform.position = temp;
        //    _elapsedTime += Time.deltaTime;
        //    _percentageComplete = _elapsedTime / desiredDurationForLineChange;


        //}

        //else if (_playerPrevLineState == PlayerLineState.middleLine && _playerLineState == PlayerLineState.rightLine)
        //{

        //    //transform.position = Vector3.Lerp(_lineStartPos, _lineEndPos, lineCurve.Evaluate(_percentageComplete));

        //    Vector3 temp = transform.position;
        //    temp.x = Mathf.Lerp(_lineStartPos.x, _lineEndPos.x, lineCurve.Evaluate(_percentageComplete));
        //    transform.position = temp;

        //    _elapsedTime += Time.deltaTime;
        //    _percentageComplete = _elapsedTime / desiredDurationForLineChange;


        //}

        //else if (_playerPrevLineState == PlayerLineState.leftLine && _playerLineState == PlayerLineState.middleLine)
        //{

        //    Vector3 temp = transform.position;
        //    temp.x = Mathf.Lerp(_lineStartPos.x, _lineEndPos.x, lineCurve.Evaluate(_percentageComplete));
        //    transform.position = temp;

        //    _elapsedTime += Time.deltaTime;
        //    _percentageComplete = _elapsedTime / desiredDurationForLineChange;


        //}

        //else if (_playerPrevLineState == PlayerLineState.rightLine && _playerLineState == PlayerLineState.middleLine)
        //{
        //    Vector3 temp = transform.position;
        //    temp.x = Mathf.Lerp(_lineStartPos.x, _lineEndPos.x, lineCurve.Evaluate(_percentageComplete));
        //    transform.position = temp;

        //    _elapsedTime += Time.deltaTime;
        //    _percentageComplete = _elapsedTime / desiredDurationForLineChange;


        //}

        if (_playerPrevLineState == PlayerLineState.middleLine && _playerLineState == PlayerLineState.leftLine)
        {
            Vector3 temp = transform.position;
            temp.x = Mathf.MoveTowards(temp.x, _lineEndPos.x, lineChangeSpeed * Time.deltaTime);
            transform.position = temp;
           


        }
        else if (_playerPrevLineState == PlayerLineState.middleLine && _playerLineState == PlayerLineState.rightLine)
        {



            Vector3 temp = transform.position;
            temp.x = Mathf.MoveTowards(temp.x, _lineEndPos.x, lineChangeSpeed * Time.deltaTime);
            transform.position = temp;

            


        }

        else if (_playerPrevLineState == PlayerLineState.leftLine && _playerLineState == PlayerLineState.middleLine)
        {


            Vector3 temp = transform.position;
            temp.x = Mathf.MoveTowards(temp.x, _lineEndPos.x, lineChangeSpeed * Time.deltaTime);
            transform.position = temp;

           


        }

        else if (_playerPrevLineState == PlayerLineState.rightLine && _playerLineState == PlayerLineState.middleLine)
        {


            Vector3 temp = transform.position;
            temp.x = Mathf.MoveTowards(temp.x, _lineEndPos.x, lineChangeSpeed * Time.deltaTime);
            transform.position = temp;

            
        }

    }
    private void HandleLines()
    {
        //Turn Left on middle line
        if (_playerLineState == PlayerLineState.middleLine && Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _lineStartPos = transform.position;
            
            _lineEndPos = new Vector3(leftLine.position.x, transform.position.y, transform.position.z);
           

            _playerPrevLineState = PlayerLineState.middleLine;
            _playerLineState = PlayerLineState.leftLine;
            //anim play

            StopRoll();
            if (!isJumping)
                _anim.SetTrigger("TurnLeft");
        }

        //Turn Right on middle line
        else if (_playerLineState == PlayerLineState.middleLine && Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {

            
            _lineStartPos = transform.position;
            _lineEndPos = new Vector3(rightLine.position.x, transform.position.y, transform.position.z);



            _playerPrevLineState = PlayerLineState.middleLine;
            _playerLineState = PlayerLineState.rightLine;
            //anim play
            StopRoll();
            if (!isJumping)
                _anim.SetTrigger("TurnRight");


        }
        //Turn right on left line
        else if (_playerLineState == PlayerLineState.leftLine && Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {

            
            _lineStartPos = transform.position;
            _lineEndPos = new Vector3(middleLine.position.x, transform.position.y, transform.position.z);


            _playerPrevLineState = PlayerLineState.leftLine;
            _playerLineState = PlayerLineState.middleLine;
            //anim play
            StopRoll();
            if (!isJumping)
                _anim.SetTrigger("TurnRight");

        }
        //Turn left on right line
        else if (_playerLineState == PlayerLineState.rightLine && Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {

            
            _lineStartPos = transform.position;
            _lineEndPos = new Vector3(middleLine.position.x, transform.position.y, transform.position.z);


            _playerPrevLineState = PlayerLineState.rightLine;
            _playerLineState = PlayerLineState.middleLine;
            //anim play
            StopRoll();
            if (!isJumping)
                _anim.SetTrigger("TurnLeft");

        }


    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("ObstacleGround"))
        {
            moveDir.y = 0;
            isJumping = false;
        }
    }
 

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(groundCheck.position, groundCheck.up * -raycastDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down  *slopeRaycastDist);
    }

    public float GetSpeed()
    {
        return playerForwardSpeed;
    }
}

public enum PlayerLineState
{
    leftLine = 0,
    middleLine = 1,
    rightLine = 2
}