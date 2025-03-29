using System.Collections;
using UnityEngine;

public class Testttte : MonoBehaviour
{
    [Header("Ground & Slope")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckDistance = 0.5f;
    [SerializeField] float gravityForce = -20f;
    [SerializeField] float slopeRaycastDist = 0.1f;
    [SerializeField] float maxSlopeAngle = 45f;
    [SerializeField] float slopeSpeedMultiplier = 1.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("Jump & Roll")]
    [SerializeField] float jumpForce = 9f;
    [SerializeField] float rollDownForce = 30f;
    [SerializeField] float actionDelay = 0.5f;

    [Header("Chase Settings")]
    [SerializeField] float catchUpSpeedMultiplier = 1.5f;
    [SerializeField] float slowingSpeedMultiplier = 0.5f;
    [SerializeField] float playerChaseThreshold = 3f;
    [SerializeField] float chaseDuration = 4f;
    [SerializeField] float disappearThreshold = 10f;
    [SerializeField] float teleportDistance = 10f;

    [Header("Line Movement")]
    [SerializeField] float lineChangeSpeed = 15f;
    [SerializeField] float snapThreshold = 0.1f;
    private const float LEFTLINE = -3f, MIDDLELINE = 0f, RIGHTLINE = 3f;

    [Header("Catch Logic")]
    [SerializeField] float backwardsMoveSpeed = 10f;
    [SerializeField] float catchThreshold = 1f;

    private EnemyChaseState chaseState = EnemyChaseState.Disabled;

    private PlayerMovement playerRef;
    private Rigidbody _rb;
    private Animator anim;

    private Vector3 moveDir;
    private RaycastHit slopeHit;
    private Vector3 startPos;

    private float _catchUpSpeed;
    private float _slowingSpeed;
    private float _speed;

    private float chaseTimer;
    private bool isGrounded;
    private bool isJumping;
    private bool isRolling;
    private bool shouldStop;

    private PlayerLineState playerLineState;
    private PlayerLineState enemyLineState;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        playerRef = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        SetSpeed();
        enemyLineState = PlayerLineState.middleLine;
        startPos = transform.position;
    }

    private void Update()
    {
        if (!GameStarter.Instance.IsGameStarted()) return;

        SetSpeed();
        UpdateTimers();
        UpdateAnimator();
        CheckGrounded();
        HandleChaseLogic();
        CheckLineSwitch();
    }

    private void FixedUpdate()
    {
        if (!GameStarter.Instance.IsGameStarted()) return;

        HandleMovement();
        SwitchLineTo(playerLineState);
    }

    public void StartChase(Vector3 playerPos)
    {
        transform.position = playerPos + Vector3.back * teleportDistance;
        chaseState = EnemyChaseState.CatchingUp;
    }

    private void HandleChaseLogic()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerRef.transform.position);

        switch (chaseState)
        {
            case EnemyChaseState.CatchingUp:
                if (distanceToPlayer <= playerChaseThreshold)
                {
                    chaseTimer = 0f;
                    chaseState = EnemyChaseState.Chasing;
                }
                break;

            case EnemyChaseState.Chasing:
                if (chaseTimer >= chaseDuration)
                {
                    chaseState = EnemyChaseState.Slowing;
                }
                break;

            case EnemyChaseState.Slowing:
                if (distanceToPlayer >= disappearThreshold)
                {
                    chaseState = EnemyChaseState.Disabled;
                    playerRef.GetComponent<Health>().ResetChase();
                    gameObject.SetActive(false);
                }
                break;
        }
    }

    private void SetSpeed()
    {
        _speed = playerRef.GetSpeed();
        _catchUpSpeed = _speed * catchUpSpeedMultiplier;
        _slowingSpeed = _speed * slowingSpeedMultiplier;
    }

    private void HandleMovement()
    {
        if (shouldStop || chaseState == EnemyChaseState.Disabled) return;

        float currentSpeed = chaseState switch
        {
            EnemyChaseState.CatchingUp => _catchUpSpeed,
            EnemyChaseState.Chasing => _speed,
            EnemyChaseState.Slowing => _slowingSpeed,
            _ => 0f
        };

        moveDir.z = currentSpeed;

        if (OnSlope())
        {
            moveDir.y = 0f;
            Vector3 slopeDir = GetSlopeMoveDirection() * currentSpeed * slopeSpeedMultiplier;
            _rb.velocity = slopeDir;
        }
        else
        {
            _rb.velocity = moveDir;
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundLayer);

        if (!isGrounded)
        {
            ApplyGravity();
        }
        else
        {
            moveDir.y = 0f;
        }
    }

    private void ApplyGravity()
    {
        moveDir.y += gravityForce * Time.deltaTime;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, slopeRaycastDist, groundLayer))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle > 0 && angle <= maxSlopeAngle;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    private void UpdateTimers()
    {
        chaseTimer += Time.deltaTime;
    }

    private void UpdateAnimator()
    {
        anim.SetBool("isGrounded", isGrounded);
    }

    private void CheckLineSwitch()
    {
        playerLineState = playerRef.GetPlayerLineState();
    }

    private void SwitchLineTo(PlayerLineState targetLine)
    {
        float targetX = targetLine switch
        {
            PlayerLineState.leftLine => LEFTLINE,
            PlayerLineState.middleLine => MIDDLELINE,
            PlayerLineState.rightLine => RIGHTLINE,
            _ => MIDDLELINE
        };

        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.MoveTowards(currentPos.x, targetX, lineChangeSpeed * Time.deltaTime);
        transform.position = currentPos;

        if (Mathf.Abs(transform.position.x - targetX) < snapThreshold)
        {
            transform.position = new Vector3(targetX, currentPos.y, currentPos.z);
            enemyLineState = targetLine;
        }
    }

    public void StartJump(float force)
    {
        if (!isGrounded || isJumping) return;
        StartCoroutine(PerformJump(force));
    }

    private IEnumerator PerformJump(float force)
    {
        isJumping = true;
        yield return new WaitForSeconds(actionDelay);
        moveDir.y = force;
        anim.SetTrigger("Jump");
        isJumping = false;
    }

    public void Roll()
    {
        if (isRolling) return;
        anim.SetTrigger("Roll");
        isRolling = true;
        StartCoroutine(PerformRoll());
    }

    private IEnumerator PerformRoll()
    {
        float timer = 0f;
        while (!isGrounded && timer < 1f)
        {
            _rb.AddForce(Vector3.down * rollDownForce, ForceMode.Acceleration);
            timer += Time.deltaTime;
            yield return null;
        }
        isRolling = false;
    }

    public void PlayCatchAnim()
    {
        anim.SetTrigger("Catch");
        shouldStop = true;
        _rb.velocity = Vector3.zero;
        StartCoroutine(GoBackwards(transform.position));
    }

    private IEnumerator GoBackwards(Vector3 hit)
    {
        Vector3 hitPos = transform.position;
        while (true)
        {
            if (Mathf.Abs(hitPos.z - transform.position.z) > catchThreshold)
                break;

            transform.position = Vector3.MoveTowards(transform.position, hitPos + Vector3.back * catchThreshold * 2, Time.deltaTime * backwardsMoveSpeed);
            yield return null;
        }
    }
}

public enum EnemyChaseState
{
    Disabled,
    CatchingUp,
    Chasing,
    Slowing
}
