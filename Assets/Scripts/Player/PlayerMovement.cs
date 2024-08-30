using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerForwardSpeed = 5f;
    [SerializeField] float maxPlayerSpeed = 15f;
    [SerializeField] float slopeSpeedMultiplier = 8f;
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



    [Header("PowerUp Variables")]
    [SerializeField] float _powerBootsJumpMultiplier = 2f;
    [SerializeField] bool _isSkating;

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
        skateData.Duration = totalSkateTime;

        rollCoroutine = StartCoroutine(RollSequence());
        StopRoll();
    }

    void Update()
    {
        if (GameStarter.instance.IsGameStarted())
        {
            HandleTouchInput();
            IncreaseSpeed();
            //CheckJump();
            CheckQueedJump();
            CheckRoll();
            CheckSlopeDir();
            CheckGrounded();
            //HandleLines();
            UpdateAnimator();
            CheckRotation();
            HandleSkate();
        }
        rotateTimer += Time.deltaTime;


    }



    private void FixedUpdate()
    {
        if (!GameStarter.instance.IsGameStarted()) return;
        if (_health.IsPlayerDead()) { return; }

        ChangeLine();

        moveDir.z = playerForwardSpeed;
        if (OnSlope())
        {
            float currentSpeed = _rb.velocity.magnitude;
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
        if (Input.GetKeyDown(rollKey) && !isRolling)
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
        _anim.SetBool("isGameStarted", GameStarter.instance.IsGameStarted());
        _anim.SetBool("isSkateboarding", _isSkating);

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
        }
        else
            moveDir.y = jumpForce;
        _anim.SetTrigger("Jump");
        _coyoteTimeTimer = coyoteTime;

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
            HandleCharacterTurns(false);
            // _anim.SetTrigger("TurnLeft");

            if (isTurning)
            {
                rotateTimer = 0;
            }
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

            HandleCharacterTurns(true);
            //    _anim.SetTrigger("TurnRight");

            if (isTurning)
            {
                rotateTimer = 0;
            }

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

            HandleCharacterTurns(true);
            //    _anim.SetTrigger("TurnRight");
            if (isTurning)
            {
                rotateTimer = 0;
            }
        }
        //Turn left on right line
        else if (_playerLineState == PlayerLineState.rightLine && Input.GetKeyDown(KeyCode.A))
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


    }
    private void HandleTouchInput()
    {

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

        if (swipeDistanceX > swipeThreshold && swipeDistanceX > swipeDistanceY)
        {
            if (endTouchPosition.x < startTouchPosition.x)
            {
                //turn left
                //Turn Left on middle line
                if (_playerLineState == PlayerLineState.middleLine)
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
                //Turn left on right line
                else if (_playerLineState == PlayerLineState.rightLine)
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
            }
            else if (endTouchPosition.x > startTouchPosition.x)
            {
                //Turn Right on middle line
                if (_playerLineState == PlayerLineState.middleLine)
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
                //Turn right on left line
                else if (_playerLineState == PlayerLineState.leftLine)
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

    private void HandleSkate()
    {
        if (_shouldUseSkate && !_isSkating)
        {
            if (!PlayerCollectibleManager.instance.CheckSkate())
            {
                return;
            }
            //isSkate
            skateTimer = 0;
            _shouldUseSkate = false;
            StartSkate();
        }


        if (skateTimer > totalSkateTime)
        {
            if (playerModel.localPosition != Vector3.zero)
            {
                playerModel.localPosition = Vector3.zero;
                skate.SetActive(false);
                _isSkating = false;
            }

        }
        skateTimer += Time.deltaTime;

    }

    private void StartSkate()
    {
        PlayerCollectibleManager.instance.UseSkate();
        _isSkating = true;
        playerModel.localPosition = skatingModelOffset;
        skate.SetActive(true);
        PowerUpManager.instance.TakePowerUp(skateData);

    }

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
        Debug.Log("Double tap action triggered!");
        if (!_isSkating)
            _shouldUseSkate = true;
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

}

public enum PlayerLineState
{
    leftLine = 0,
    middleLine = 1,
    rightLine = 2
}