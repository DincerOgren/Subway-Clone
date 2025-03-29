using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] bool controlWithKeyboard = true;
    [SerializeField] float playerForwardSpeed = 8f;
    [SerializeField] float defaultForwardSpeed= 8f;
    [SerializeField] float maxPlayerSpeed = 15f;
    [SerializeField] float slopeSpeedMultiplier = 1.2f;
    [SerializeField] float animationSpeedMultiplier = 1f;
    [SerializeField] float speedTickRate = 1f;

    [SerializeField] Transform leftLine, rightLine, middleLine;
    [SerializeField] float lineChangeSpeed = 5f;
    [SerializeField] float instantLineChangeSpeed = 15f;

    // DELETE LATER
    public bool onSlope = false;

    //FLAG
    public bool isTakingDamage = false;
    private bool isInstantMoving = false;

    [Header("Jump")]
    public float jumpForce = 9f;
    public bool isJumping = false;
    public bool isGrounded = false;
    public KeyCode jumpKey = KeyCode.Space;
    public Transform groundCheck;
    public float raycastDistance = 0.5f;
    public LayerMask groundLayer;
    public float gravityForce = -20f;
    public float groundDamping = .9f;
    public float coyoteTime = 0.3f;
    float _coyoteTimeTimer = 0;
    private bool jumpQueued = false;

    [Header("Roll")]
    public KeyCode rollKey = KeyCode.S;
    public float rollDownForce = -40f;
    public float rollCollisionHeight = .5f;
    public float rollDuration = .5f;
    public float originalHeight;
    public Vector3 originalCenter;
    public bool isRolling;
    bool assignRoutine = false;
    [Header("Slopes")]

    [SerializeField] float slopeRaycastDist = .1f;
    [SerializeField] float maxSlopeAngle;
    private Vector3 slopeMoveDir;
    RaycastHit slopeHit;

    [Header("Turns")]
    public Transform playerModel;
    public float turnAngle = 45f;
    public bool isTurning = false;
    public float turnTimer = .5f;
    Quaternion targetRotation;
    public float rotationSpeed = 5f;
    public float rotateTimer = 0;

    // Swipe detection variables
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    public float swipeThreshold = 20f;


    [Header("SkateBoard")]
    public PowerUp skateData;
    public float totalSkateTime = 10f;
    public float skateTimer = 0;
    public Vector3 skatingModelOffset;
    public GameObject skate;
    public float doubleTapThreshold = 0.3f;
    bool _shouldUseSkate;
    private float lastTapTime = 0f;
    private int tapCount = 0;
    SkateHandler _skateHandler;


    [Header("PowerUp Variables")]
    [SerializeField] float _powerBootsJumpMultiplier = 2f;
    [SerializeField] bool _isSkating;

    Vector3 moveDir;

    private Vector3 _lineStartPos;
    private Vector3 _lineEndPos;

    [Header("Chasing Enemy")]
    ChasingEnemy enemy;

    [Header("Timers")]
    float speedTimer;

    Coroutine rollCoroutine;
    CapsuleCollider _collider;
    Rigidbody _rb;
    Animator _anim;
    Health _health;
    PlayerLineState _playerLineState;
    PlayerLineState _playerPrevLineState;


    [Header("Error Period")]
    bool detectSwipeForErrorPeriod;

    #region Monobehaviour Methods
    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _health = GetComponent<Health>();
        enemy = GameObject.FindWithTag("Enemy").GetComponent<ChasingEnemy>();
        _skateHandler = GetComponent<SkateHandler>();
    }
    void Start()
    {
        _playerLineState = PlayerLineState.middleLine;
        _playerPrevLineState = _playerLineState;
        originalCenter = _collider.center;
        originalHeight = _collider.height;
        skateData.Duration = totalSkateTime;

        rollCoroutine = StartCoroutine(RollSequence());
        StopRoll();
        assignRoutine = true;
    }

    void Update()
    {
        //Delete late
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnDoubleTap();
        }
        if (GameStarter.Instance.IsGameStarted())
        {
            onSlope = OnSlope();
            //print("game Started");
            HandleTouchInput();
            IncreaseSpeed();
            //CheckJump();
            CheckQueedJump();
            CheckRoll();
            CheckSlopeDir();
            CheckGrounded();
            //HandleLines();
            CheckRotation();
            //HandleSkate();
            // print("player speed = " + playerForwardSpeed);
        }
        else
        {
            print("game not started");
        }
        UpdateAnimator();
        rotateTimer += Time.deltaTime;


    }



    private void FixedUpdate()
    {
        //print("Speed "+ moveDir);
        if (!GameStarter.Instance.IsGameStarted())
        {
            moveDir = Vector3.zero;
            return;
        }
        if (_health.IsPlayerDead()) { return; }


        //print("in fixed");
        ChangeLine();

        moveDir.z = playerForwardSpeed;
        if (OnSlope())
        {
            moveDir.y = 0;
            var newMovdir = GetSlopeMoveDirection();
            //newMovdir.y *= currentSpeed * slopeSpeedMultiplier;
            //newMovdir.z *= currentSpeed * slopeSpeedMultiplier;
            float slopeSpeed = playerForwardSpeed * slopeSpeedMultiplier;
            newMovdir *= slopeSpeed;
            _rb.velocity = newMovdir;
        }
        else
            _rb.velocity = moveDir;

    }
    #endregion

    #region Roll
    private void CheckRoll()
    {
        

        if (isRolling && !isGrounded)
        {
            if (enemy.isActiveAndEnabled)
            {
                enemy.Roll();
            }
            _rb.AddForce(Vector3.down * rollDownForce, ForceMode.Acceleration);
        }
    }

    IEnumerator RollSequence()
    {
        if (assignRoutine)
            MissionCounter.Instance.RollCounter();

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

    #region Slope Handling
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
    #endregion
    private void UpdateAnimator()
    {
        _anim.SetBool("isJumping", isJumping);
        _anim.SetBool("isGrounded", isGrounded);
        _anim.SetBool("isGameStarted", GameStarter.Instance.IsGameStarted());

    }

    #region Jump & Grounded
    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, raycastDistance, groundLayer);
        if (!isGrounded)
        {
            moveDir.y += gravityForce * Time.deltaTime;
            _coyoteTimeTimer += Time.deltaTime;
        }
        else
        {
            _coyoteTimeTimer = 0;
            if (OnSlope() && isGrounded)
            {
                isJumping = false;
            }
        }
    }


    private void CheckJump()
    {
        if (Input.GetKeyDown(jumpKey) && (isGrounded || _coyoteTimeTimer <= coyoteTime) && !isJumping)
        {
            isJumping = true;
            StopRoll();
            Jump();
        }

    }

    void CheckQueedJump()
    {
        if (isGrounded && jumpQueued && !isJumping)
        {
            Debug.Log("Executing queued jump.");
            isJumping = true;
            StopRoll();
            Jump();
            jumpQueued = false; // Reset the queued jump after executing it
        }
    }

    private void Jump()
    {
        if (PowerUpManager.instance.GetPowerUpData(PowerUpType.PowerBoots).IsActive)
        {
            moveDir.y = jumpForce * _powerBootsJumpMultiplier;
            print("EnemyShouldJumpHigher");
            if (enemy.isActiveAndEnabled)
            {

                enemy.StartJump(moveDir.y);
            }
        }
        else
        {

            moveDir.y = jumpForce;
            print("EnemyShouldJump");
            enemy.StartJump(moveDir.y);
        }

        _anim.SetTrigger("Jump");
        _coyoteTimeTimer = coyoteTime;
        MissionCounter.Instance.JumpCounter();

        jumpQueued = false;
    }
    #endregion
    private void IncreaseSpeed()
    {
        speedTimer += Time.deltaTime;

        if (speedTimer <= speedTickRate) return;
        //Character speed
        if (playerForwardSpeed <= maxPlayerSpeed)
        {
            animationSpeedMultiplier += .002f;
            playerForwardSpeed += .01f;
        }
        //Animatino speed
        _anim.SetFloat("speedMultiplier", animationSpeedMultiplier);
        speedTimer = 0;


    }

    #region Error Swipe Detection

    public void EnableSwipeDetectionDuringErrorPeriod(bool enable)
    {
        detectSwipeForErrorPeriod = enable;
    }

    public bool CheckSwipeDuringErrorPeriod()
    {
        if (!detectSwipeForErrorPeriod) return false;

        if (Input.touchCount > 0)
        {
            print("in swiptte detection");
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    endTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPosition = touch.position;

                    if (_skateHandler.IsSkating()) // Check if the swipe is valid for recovery
                    {
                        //Maybe jump too?

                        _skateHandler.OnSkateContact();
                        return true; // Valid swipe detected, player should recover
                    }
                    else if (DetectSwipeInErrorPeriod())
                    {


                        

                        return true;

                    }
                    break;
            }
        }

        return false;
    }


    private IEnumerator ShakePlayer()
    {
        float shakeDuration = .5f;  // Duration of the shake
        float shakeMagnitude = 0.1f; // Magnitude of the shake

        Vector3 originalPosition = playerModel.position;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            playerModel.position = new Vector3(transform.position.x + offsetX, transform.position.y, transform.position.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // After shaking, restore the original position
        playerModel.position = originalPosition;
    }


    public IEnumerator InstantMoveToPreviousLine()
    {
        isInstantMoving = true;

        while (true)
        {

            if (Mathf.Approximately(_lineEndPos.x, transform.position.x))
            {
                break;
            }


            if (_playerPrevLineState == PlayerLineState.middleLine && _playerLineState == PlayerLineState.rightLine)
            {
                TurnLeftOnRight();

                Vector3 temp = transform.position;
                temp.x = Mathf.MoveTowards(temp.x, _lineEndPos.x, lineChangeSpeed * Time.deltaTime);
                transform.position = temp;

            }
            //TurnLeftOnMiddle
            else if (_playerPrevLineState == PlayerLineState.leftLine && _playerLineState == PlayerLineState.middleLine)
            {
                TurnLeftOnMiddle();

                Vector3 temp = transform.position;
                temp.x = Mathf.MoveTowards(temp.x, _lineEndPos.x, lineChangeSpeed * Time.deltaTime);
                transform.position = temp;

            }
            //TurnRightOnLeft
            else if (_playerPrevLineState == PlayerLineState.middleLine && _playerLineState == PlayerLineState.leftLine)
            {
                TurnRightOnLeft();

                Vector3 temp = transform.position;
                temp.x = Mathf.MoveTowards(temp.x, _lineEndPos.x, lineChangeSpeed * Time.deltaTime);
                transform.position = temp;

            }
            //TurnRightOnMiddle
            else if (_playerPrevLineState == PlayerLineState.rightLine && _playerLineState == PlayerLineState.middleLine)
            {
                TurnRightOnMiddle();

                Vector3 temp = transform.position;
                temp.x = Mathf.MoveTowards(temp.x, _lineEndPos.x, lineChangeSpeed * Time.deltaTime);
                transform.position = temp;
            }

            yield return null;
        }

        isInstantMoving = false;
    }
    
    public void MoveToPreviousLine()
    {
        //yield return new WaitForSeconds(0.5f); // Wait for the shake to finish

        //TurnLeftOnRight
        if (_playerPrevLineState == PlayerLineState.middleLine && _playerLineState == PlayerLineState.rightLine)
        {
            TurnLeftOnRight();
        }
        //TurnLeftOnMiddle
        else if (_playerPrevLineState == PlayerLineState.leftLine && _playerLineState == PlayerLineState.middleLine)
        {
            TurnLeftOnMiddle();
        }
        //TurnRightOnLeft
        else if (_playerPrevLineState == PlayerLineState.middleLine && _playerLineState == PlayerLineState.leftLine)
        {
            TurnRightOnLeft();
        }
        //TurnRightOnMiddle
        else if (_playerPrevLineState == PlayerLineState.rightLine && _playerLineState == PlayerLineState.middleLine)
        {
            TurnRightOnMiddle();
        }


    }
    
    private bool DetectSwipeInErrorPeriod()
    {
        float swipeDistanceX = Mathf.Abs(endTouchPosition.x - startTouchPosition.x);
        float swipeDistanceY = Mathf.Abs(endTouchPosition.y - startTouchPosition.y);

        if (swipeDistanceX > swipeThreshold && swipeDistanceX > swipeDistanceY)
        {
            print("swipe Detected ");
            if (endTouchPosition.x < startTouchPosition.x)
            {
                // Swipe Left
                print("in left");
                return CanSwipeLeft();
            }
            else if (endTouchPosition.x > startTouchPosition.x)
            {
                print("in right");

                // Swipe Right
                return CanSwipeRight();
            }
        }
        print("swipe not Detected ");


        return false; // No valid swipe detected
    }

    private bool CanSwipeLeft()
    {
        // If the player is in the leftmost lane, they can't swipe left
        if (_playerLineState == PlayerLineState.leftLine)
        {
            print("left false ");

            return false; // Invalid swipe
        }
        print("Return true from left");

        // Otherwise, they can swipe left
        return true;
    }

    private bool CanSwipeRight()
    {
        // If the player is in the rightmost lane, they can't swipe right
        if (_playerLineState == PlayerLineState.rightLine && _playerPrevLineState==PlayerLineState.rightLine)
        {
            print("right false ");
            return false; // Invalid swipe
        }

        print("Return true from right");
        // Otherwise, they can swipe right
        return true;
    }


    #endregion

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

        if (isInstantMoving) return;
             
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
    //private void HandleLines()
    //{
    //    //Turn Left on middle line
    //    if (_playerLineState == PlayerLineState.middleLine && Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        _lineStartPos = transform.position;

    //        _lineEndPos = new Vector3(leftLine.position.x, transform.position.y, transform.position.z);


    //        _playerPrevLineState = PlayerLineState.middleLine;
    //        _playerLineState = PlayerLineState.leftLine;
    //        //anim play

    //        StopRoll();
    //        HandleCharacterTurns(false);
    //        // _anim.SetTrigger("TurnLeft");

    //        if (isTurning)
    //        {
    //            rotateTimer = 0;
    //        }
    //    }

    //    //Turn Right on middle line
    //    else if (_playerLineState == PlayerLineState.middleLine && Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
    //    {


    //        _lineStartPos = transform.position;
    //        _lineEndPos = new Vector3(rightLine.position.x, transform.position.y, transform.position.z);



    //        _playerPrevLineState = PlayerLineState.middleLine;
    //        _playerLineState = PlayerLineState.rightLine;
    //        //anim play
    //        StopRoll();

    //        HandleCharacterTurns(true);
    //        //    _anim.SetTrigger("TurnRight");

    //        if (isTurning)
    //        {
    //            rotateTimer = 0;
    //        }

    //    }
    //    //Turn right on left line
    //    else if (_playerLineState == PlayerLineState.leftLine && Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
    //    {


    //        _lineStartPos = transform.position;
    //        _lineEndPos = new Vector3(middleLine.position.x, transform.position.y, transform.position.z);


    //        _playerPrevLineState = PlayerLineState.leftLine;
    //        _playerLineState = PlayerLineState.middleLine;
    //        //anim play
    //        StopRoll();

    //        HandleCharacterTurns(true);
    //        //    _anim.SetTrigger("TurnRight");
    //        if (isTurning)
    //        {
    //            rotateTimer = 0;
    //        }
    //    }
    //    //Turn left on right line
    //    else if (_playerLineState == PlayerLineState.rightLine && Input.GetKeyDown(KeyCode.A))
    //    {


    //        _lineStartPos = transform.position;
    //        _lineEndPos = new Vector3(middleLine.position.x, transform.position.y, transform.position.z);


    //        _playerPrevLineState = PlayerLineState.rightLine;
    //        _playerLineState = PlayerLineState.middleLine;
    //        //anim play
    //        StopRoll();

    //        HandleCharacterTurns(false);
    //        //_anim.SetTrigger("TurnLeft");

    //        if (isTurning)
    //        {
    //            rotateTimer = 0;
    //        }

    //    }


    //}
    private void HandleTouchInput()
    {
        if (controlWithKeyboard)
        {
            DetectSwipe();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    endTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPosition = touch.position;
                    DetectSwipe();
                    break;
            }
        }

        DetectDoubleTap();

    }

    private void DetectSwipe()
    {
        float swipeDistanceX = Mathf.Abs(endTouchPosition.x - startTouchPosition.x);
        float swipeDistanceY = Mathf.Abs(endTouchPosition.y - startTouchPosition.y);


        // Delete later
        if (controlWithKeyboard)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                jumpQueued = false;
                //turn left
                //Turn Left on middle line
                if (_playerLineState == PlayerLineState.middleLine)
                {
                    TurnLeftOnMiddle();
                }
                //Turn left on right line
                else if (_playerLineState == PlayerLineState.rightLine)
                {
                    TurnLeftOnRight();

                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                jumpQueued = false;

                //Turn Right on middle line
                if (_playerLineState == PlayerLineState.middleLine)
                {
                    TurnRightOnMiddle();

                }
                //Turn right on left line
                else if (_playerLineState == PlayerLineState.leftLine)
                {
                    TurnRightOnLeft();
                }

            }

            if (Input.GetKeyDown(KeyCode.Space))
            {

                if ((isGrounded || _coyoteTimeTimer <= coyoteTime) && !isJumping)
                {
                    isJumping = true;
                    StopRoll();
                    Jump();
                }
                else if (!isGrounded && !jumpQueued)
                {
                    jumpQueued = true;
                    Debug.Log("Jump queued.");
                }

            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                jumpQueued = false;

                if (!isRolling)
                {
                    isRolling = true;
                    _anim.SetTrigger("Roll");
                    rollCoroutine = StartCoroutine(RollSequence());

                }

                if (isRolling && !isGrounded)
                {
                    _rb.AddForce(Vector3.down * rollDownForce, ForceMode.Acceleration);
                }
            }

        }

        else if (swipeDistanceX > swipeThreshold && swipeDistanceX > swipeDistanceY)
        {
            if (endTouchPosition.x < startTouchPosition.x)
            {
                jumpQueued = false;
                //turn left
                //Turn Left on middle line
                if (_playerLineState == PlayerLineState.middleLine)
                {
                    TurnLeftOnMiddle();
                }
                //Turn left on right line
                else if (_playerLineState == PlayerLineState.rightLine)
                {
                    TurnLeftOnRight();

                }
            }
            else if (endTouchPosition.x > startTouchPosition.x)
            {
                jumpQueued = false;

                //Turn Right on middle line
                if (_playerLineState == PlayerLineState.middleLine)
                {
                    TurnRightOnMiddle();

                }
                //Turn right on left line
                else if (_playerLineState == PlayerLineState.leftLine)
                {
                    TurnRightOnLeft();
                }

            }
        }
        else if (swipeDistanceY > swipeThreshold)
        {
            if (endTouchPosition.y > startTouchPosition.y)
            {

                if ((isGrounded || _coyoteTimeTimer <= coyoteTime) && !isJumping)
                {
                    isJumping = true;
                    StopRoll();
                    Jump();
                }
                else if (!isGrounded && !jumpQueued)
                {
                    jumpQueued = true;
                    Debug.Log("Jump queued.");
                }

            }
            else if (endTouchPosition.y < startTouchPosition.y)
            {
                jumpQueued = false;

                if (!isRolling)
                {
                    isRolling = true;
                    _anim.SetTrigger("Roll");
                    rollCoroutine = StartCoroutine(RollSequence());

                }

                if (isRolling && !isGrounded)
                {
                    _rb.AddForce(Vector3.down * rollDownForce, ForceMode.Acceleration);
                }
            }
        }
    }

    #region TurnMethods
    private void TurnRightOnLeft()
    {
        _lineStartPos = transform.position;
        _lineEndPos = new Vector3(middleLine.position.x, transform.position.y, transform.position.z);


        _playerPrevLineState = PlayerLineState.leftLine;
        _playerLineState = PlayerLineState.middleLine;
        //anim play
        StopRoll();

        HandleCharacterTurns(true);
        //    _anim.SetTrigger("TurnRight");
        if (isTurning)
        {
            rotateTimer = 0;
        }
    }

    private void TurnRightOnMiddle()
    {
        _lineStartPos = transform.position;
        _lineEndPos = new Vector3(rightLine.position.x, transform.position.y, transform.position.z);



        _playerPrevLineState = PlayerLineState.middleLine;
        _playerLineState = PlayerLineState.rightLine;
        //anim play
        StopRoll();

        HandleCharacterTurns(true);
        //    _anim.SetTrigger("TurnRight");

        if (isTurning)
        {
            rotateTimer = 0;
        }
    }

    private void TurnLeftOnRight()
    {
        _lineStartPos = transform.position;
        _lineEndPos = new Vector3(middleLine.position.x, transform.position.y, transform.position.z);


        _playerPrevLineState = PlayerLineState.rightLine;
        _playerLineState = PlayerLineState.middleLine;
        //anim play
        StopRoll();

        HandleCharacterTurns(false);
        //_anim.SetTrigger("TurnLeft");

        if (isTurning)
        {
            rotateTimer = 0;
        }
    }

    private void TurnLeftOnMiddle()
    {
        _lineStartPos = transform.position;

        _lineEndPos = new Vector3(leftLine.position.x, transform.position.y, transform.position.z);


        _playerPrevLineState = PlayerLineState.middleLine;
        _playerLineState = PlayerLineState.leftLine;
        //anim play

        StopRoll();
        HandleCharacterTurns(false);
        // _anim.SetTrigger("TurnLeft");

        if (isTurning)
        {
            rotateTimer = 0;
        }
    }


    #endregion


    private void HandleCharacterTurns(bool isTurningRight)
    {
        if (isTurningRight)
        {
            targetRotation = Quaternion.Euler(0, turnAngle, 0);
            isTurning = true;

        }
        else
        {

            isTurning = true;
            targetRotation = Quaternion.Euler(0, -turnAngle, 0);
        }

    }
    private void RotateCharacter()
    {
        Quaternion startRot = playerModel.localRotation;
        playerModel.localRotation = Quaternion.Lerp(startRot, targetRotation, Time.deltaTime * rotationSpeed);

    }

    private void CheckRotation()
    {
        if (isTurning && rotateTimer <= turnTimer)
        {
            RotateCharacter();
        }



        if (rotateTimer > turnTimer)
        {
            Quaternion startRot = playerModel.localRotation;
            Quaternion endRot = Quaternion.Euler(0, 0, 0);



            playerModel.localRotation = Quaternion.Lerp(startRot, endRot, Time.deltaTime * rotationSpeed);



        }

        if (playerModel.eulerAngles.y >= 359f || playerModel.eulerAngles.y <= 1f)
        {
            Quaternion endRot = Quaternion.Euler(0, 0, 0);
            playerModel.rotation = endRot;
            isTurning = false;
        }

    }

    //Delete  later old skate script

    //public bool IsSkating()
    //{
    //    return _isSkating;
    //}
    //private void HandleSkate()
    //{
    //    if (_shouldUseSkate && !_isSkating)
    //    {
    //        if (!PlayerCollectibleManager.instance.CheckSkate())
    //        {
    //            return;
    //        }
    //        //isSkate
    //        skateTimer = 0;
    //        _shouldUseSkate = false;
    //        StartSkate();
    //    }


    //    if (skateTimer > totalSkateTime)
    //    {
    //        if (playerModel.localPosition != Vector3.zero)
    //        {
    //            playerModel.localPosition = Vector3.zero;
    //            skate.SetActive(false);
    //            _isSkating = false;
    //        }

    //    }
    //    skateTimer += Time.deltaTime;

    //}

    //private void StartSkate()
    //{
    //    PlayerCollectibleManager.instance.UseSkate();
    //    _isSkating = true;
    //    playerModel.localPosition = skatingModelOffset;
    //    skate.SetActive(true);
    //    PowerUpManager.instance.TakePowerUp(skateData);

    //}

    private void DetectDoubleTap()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;

                float distanceMoved = Vector2.Distance(startTouchPosition, endTouchPosition);

                if (distanceMoved <= swipeThreshold)
                {
                    tapCount++;

                    if (tapCount == 1)
                    {
                        lastTapTime = Time.time;
                    }
                    else if (tapCount == 2 && Time.time - lastTapTime <= doubleTapThreshold)
                    {
                        Debug.Log("Double Tap Detected");
                        OnDoubleTap();

                        // Reset tap count
                        tapCount = 0;
                    }
                }
                else
                {
                    // If it's a swipe, reset the tap count
                    tapCount = 0;
                }
            }

            // Reset if the second tap did not occur within the threshold time
            if (Time.time - lastTapTime > doubleTapThreshold)
            {
                tapCount = 0;
            }
        }
    }

    private void OnDoubleTap()
    {
       
        _skateHandler.TryToUseSkate();

    }

    //private IEnumerator RotateCharacter(float angle)
    //{
    //    Quaternion startRotation = playerModel.rotation;
    //    Quaternion endRotation = Quaternion.Euler(0, playerModel.eulerAngles.y + angle, 0);
    //    float elapsedTime = 0f;
    //    print("in rutin");

    //    while (elapsedTime < turnTimer)
    //    {
    //        print("in while");
    //        playerModel.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / turnTimer);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    playerModel.rotation = endRotation; // Ensure the rotation ends exactly at the target rotation

    //    StartCoroutine(EndTurn());
    //}
    //private IEnumerator EndTurn()
    //{
    //    if (rotateTimer > turnTimer)
    //    {
    //        Quaternion startRot = playerModel.rotation;
    //        Quaternion endRotation = Quaternion.Euler(0,0, 0);
    //        float elapsedTime = 0f;

    //        while (elapsedTime < turnTimer)
    //        {
    //            print("in while");
    //            playerModel.rotation = Quaternion.Lerp(startRot, endRotation, elapsedTime / turnTimer);
    //            elapsedTime += Time.deltaTime;
    //            yield return null;
    //        }

    //        playerModel.rotation = endRotation;
    //        isTurning = false;
    //        //print("Turning False");
    //    }
    //}


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
        Gizmos.DrawRay(transform.position, Vector3.down * slopeRaycastDist);
    }

    public float GetSpeed()
    {
        return playerForwardSpeed;
    }

    public void SetPlayerSpeed(float value)
    {
        playerForwardSpeed = value;

    }
    public void ResetVelocity()
    {
        _rb.velocity = Vector3.zero;
    }
    public void ResetPlayer()
    {
        playerForwardSpeed = defaultForwardSpeed;
        _rb.velocity = Vector3.zero;
        _anim.SetTrigger("ResetGame");
        print("Player speed resetted");
        _playerLineState = PlayerLineState.middleLine;
        _playerPrevLineState = PlayerLineState.middleLine;
        animationSpeedMultiplier = 1;
    }
    public PlayerLineState GetPlayerLineState() => _playerLineState;

}

public enum PlayerLineState
{
    leftLine = 0,
    middleLine = 1,
    rightLine = 2
}