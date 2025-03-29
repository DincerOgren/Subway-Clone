using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{

    [Header("Health Variables")]
    [SerializeField] float maxHealth = 3;
    float _currentHealth;

    [Header("UI")]
    [SerializeField] Canvas inGameCanvas;
    [SerializeField] Canvas endGameCanvas;
    [SerializeField] TextMeshProUGUI endScore;
    [SerializeField] TextMeshProUGUI endHighScore;


    [SerializeField] float invincibleTimeAfterDamage = 1f;
    float invincibleTimer = Mathf.Infinity;
    Material _playerMat;
    bool isDead = false;

    bool isInvincible;
    [Header("Player Movement")]
    PlayerMovement playerMovement;
    float oldSpeed;

    [Header("Error Handler")]
    public bool isInErrorPeriod = false;
    public float errorPeriodTime = .5f;
    public float _errorTimer = 0;
    public bool isPlayerTwisted;
    public float twistedTime = 2f;
    public float _twistedTimer = Mathf.Infinity;

    [Header("Hit Values")]
    public float backwardsThreshold = 2f;
    public float backwardMoveSpeed = 10f;
    Vector3 hitPos;

    [Header("Train Raycasts")]
    [SerializeField] Transform footRaycastTransform;
    [SerializeField] Transform headRaycastTransform;
    [SerializeField] float footRaycastDist;
    [SerializeField] float headRaycastDist;
    [SerializeField] LayerMask trainLayer;
    [SerializeField] float trainTopY = 3.1f;
    [SerializeField] float frontBuffer = 1.5f;

    bool isRunFinished;

    Animator _anim;
    ChasingEnemy _enemyRef;
    bool _chaseStarted;
    SkateHandler _skateHandler;
    
    //[Header("Knockback")]
    //[SerializeField] float knockBackForce = 10f;
    //[SerializeField] float knockBackSpeed = 20f;


    bool startClean;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        _enemyRef = GameObject.FindWithTag("Enemy").GetComponent<ChasingEnemy>();
        _anim = GetComponent<Animator>();
        _skateHandler = GetComponent<SkateHandler>();
    }

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    private void Update()
    {
        Debug.DrawRay(footRaycastTransform.position, footRaycastDist * transform.forward, Color.red, 10000f);

        Debug.DrawRay(headRaycastTransform.position, headRaycastDist * transform.forward, Color.blue, 10000f);

        //if (Flags.isTakingDamage)
        //    startClean = true;

        //if (invincibleTimer > invincibleTimeAfterDamage && startClean)
        //{
        //    startClean = false;
        //    GetComponent<DamageCollisions>().ClearArray();
        //}
        //Flags.isTakingDamage = !(invincibleTimer > invincibleTimeAfterDamage);

        TryJumpRecoverOnTrainFrontHit();
        UpdateTimers();

        CheckError();
        CheckPlayerState();
        CheckInvincible();

    }

    private void TryJumpRecoverOnTrainFrontHit()
    {
        if (CheckFootRaycast() && CheckHeadRaycast())
        {
            Debug.LogWarning("Both true");
        }
        else if (CheckFootRaycast() && !CheckHeadRaycast())
        {
            Debug.LogWarning("Yukarý ýýsýnla abimi");
            Vector3 newPos = transform.position;
            newPos.y = trainTopY;
            newPos.z += frontBuffer;
            transform.position = newPos;
            EnterTwistedState();
        }
    }

    bool CheckFootRaycast()
    {
        if (Physics.Raycast(footRaycastTransform.position,transform.forward,out RaycastHit hitInfo, footRaycastDist, trainLayer))
        {
            var hittedObject = hitInfo.transform;
            if (hittedObject.CompareTag("TrainFront") || hittedObject.CompareTag("ObstacleGround"))
            {
                print("hit train" + hittedObject.name);
                return true;
            }
        }

        return false;
    }
    bool CheckHeadRaycast()
    {
        if (Physics.Raycast(headRaycastTransform.position,transform.forward,out RaycastHit hitInfo, headRaycastDist, trainLayer))
        {
            
                //if hit then no jump
            return true;
            
        }

        return false;
    }

    private void UpdateTimers()
    {
        invincibleTimer += Time.deltaTime;
        _twistedTimer += Time.deltaTime;
    }
    private void CheckPlayerState()
    {
        if (!isPlayerTwisted) return;

        if (!_chaseStarted)
        {
            _enemyRef.gameObject.SetActive(true);

            _enemyRef.StartChase(transform.position);
            _chaseStarted = true;

        }
        if (_twistedTimer >= twistedTime)
        {
            isPlayerTwisted = false;
            // enemyRef.StartChase(false);
        }


       
    }

    private void CheckInvincible()
    {
        if (!isInvincible) return;

        if (invincibleTimer >= invincibleTimeAfterDamage)
        {
            isInvincible = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isInvincible)
        {
            return;
        }

        Debug.LogWarning("---------------------------------");
        if (collision.collider.CompareTag("TrainFront") || collision.collider.CompareTag("Obstacle"))
        {
            // If the player hits the front, they die immediately



            if (!isInErrorPeriod)
            {
                StartErrorPeriod();
            }
            else
            {
                StartCoroutine(Die());
            }
        }
        else if (collision.collider.CompareTag("TrainSide"))
        {

            //if (_skateHandler.IsSkating())
            //{
            //    //Blow and return;
            //    //Explosion
            //}



            // If the player hits the side, enter the twisted state
            if (!isPlayerTwisted)
            {
                EnterTwistedState();
            }
            else
            {
                if (_skateHandler.IsSkating())
                {
                    _skateHandler.OnSkateContact();
                    return;
                }
                // If already twisted, the player dies

                StartCoroutine(Die(true));
            }
        }

    }





    private void CheckError()
    {
        if (!isInErrorPeriod) return;

        _errorTimer -= Time.deltaTime;

        if (_errorTimer >= 0)
        {
            // Continuously check for swipe during error period
            if (playerMovement.CheckSwipeDuringErrorPeriod())
            {
                print("Player swipe true return");

                RecoverFromHit();
                return;
            }
            else
                print("Player not recover from hit");
        }

        if (_errorTimer < 0)
        {
            EndErrorPeriod();
        }
    }
    private void EnterTwistedState()
    {
        isPlayerTwisted = true;
        _twistedTimer = 0;
        // Start shaking the player
        //StartCoroutine(ShakePlayer());

        // Move the player back to the previous lane after the shake
        playerMovement.MoveToPreviousLine();
        //isPlayerTwisted = false;
        //print("PLAYER TWISTEDDDDDD");
    }






    private void StartErrorPeriod()
    {
        isInErrorPeriod = true;
        _errorTimer = errorPeriodTime;


        oldSpeed = playerMovement.GetSpeed();
        // Optionally slow down the player or provide visual feedback
        playerMovement.SetPlayerSpeed(0); // Stop player movement during the error period
        playerMovement.EnableSwipeDetectionDuringErrorPeriod(true); // Start detecting recovery swipes

        Debug.Log("Error period started. Swipe to recover!");
    }
    private void RecoverFromHit()
    {
        isInErrorPeriod = false;
        _errorTimer = 0f;

        Debug.Log("Player recovered from hit!");

        playerMovement.EnableSwipeDetectionDuringErrorPeriod(false); // Disable error period swipe detection
        playerMovement.SetPlayerSpeed(oldSpeed);
        oldSpeed = 0f;
        isPlayerTwisted = true;

        EnterInvincibleState();
    }

    private void EnterInvincibleState()
    {
        invincibleTimer = 0;
        isInvincible = true;
    }

    private void EndErrorPeriod()
    {
        isInErrorPeriod = false;
        Debug.Log("End Of Error Period. Player dies");
        playerMovement.EnableSwipeDetectionDuringErrorPeriod(false); // Disable error period swipe detection
        playerMovement.SetPlayerSpeed(oldSpeed); // Restore player speed
        
        oldSpeed = 0f;
        StartCoroutine(Die());
    }


    //IEnumerator KnockBack()
    //{
    //    Flags.isTakingDamage = true;
    //    while (invincibleTimer < invincibleTimeAfterDamage)
    //    {
    //        //Change so only moves z 
    //        Vector3 kbForce = Vector3.back * knockBackForce;
    //        _rb.velocity = kbForce;

    //        yield return null;
    //    }
    //    Flags.isTakingDamage = false;
    //}



    #region Damage Flash


    //private IEnumerator DamageFlash()
    //{
    //    SetFlashColor();
    //    while (invincibleTimer < invincibleTimeAfterDamage)
    //    {

    //        float currentFlashAmount = (Mathf.Sin(flashAmount * Time.time) + 1) / 2;
    //        //print(currentFlashAmount);
    //        _playerMat.SetFloat("_FlashAmount", currentFlashAmount);
    //        yield return null;
    //    }

    //    _playerMat.SetFloat("_FlashAmount", 0);

    //}

    //private void SetFlashColor()
    //{
    //    _playerMat.SetColor("_FlashColor", flashColor);
    //}


    #endregion
    //private void Die(bool deathFromTwisted=false)
    //{
    //    if (isDead) { return; }

    //    if (deathFromTwisted)
    //    {
    //        Debug.LogWarning("-----------------------------------------------");
    //        playerMovement.InstantMoveToPreviousLine();
    //    }

    //    isDead = true;
    //    playerMovement.SetPlayerSpeed(0);
    //    playerMovement.ResetVelocity();

    //    ScoreManager.instance.SaveScore();
    //    //Play death anim


    //    if (!isRunFinished && !deathFromTwisted)
    //    {
    //        isRunFinished = true;
    //        _anim.SetTrigger("Die");
    //        StartCoroutine(GoBackwards());
    //    }
    //    else
    //        _anim.SetTrigger("Die");


    //    endScore.text = ScoreManager.instance.GetScore().ToString();
    //    endHighScore.text = ScoreManager.instance.GetHighScore().ToString();

    //    //Catch Sequence


    //    _enemyRef.PlayCathAnim();

    //    print("YOU DEAD");

    //    Invoke(nameof(EndGameStuffs),2f);

    //    //Invoke(nameof(DieWithTime),1f); ?
    //}
    private IEnumerator Die(bool deathFromTwisted = false)
    {
        if (isDead) yield break;

        if (deathFromTwisted)
        {
            Debug.LogWarning("Twisted ölüm – önce geri git");
            yield return StartCoroutine(playerMovement.InstantMoveToPreviousLine());
        }

        isDead = true;
        playerMovement.SetPlayerSpeed(0);
        playerMovement.ResetVelocity();

        ScoreManager.instance.SaveScore();

        if (!isRunFinished && !deathFromTwisted)
        {
            isRunFinished = true;
            _anim.SetTrigger("Die");
            yield return StartCoroutine(GoBackwards()); // varsa
        }
        else
        {
            _anim.SetTrigger("Die");
        }

        endScore.text = ScoreManager.instance.GetScore().ToString();
        endHighScore.text = ScoreManager.instance.GetHighScore().ToString();

        _enemyRef.PlayCathAnim();

        Debug.Log("YOU DEAD");

        Invoke(nameof(EndGameStuffs), 2f);
    }
    void EndGameStuffs()
    {
        inGameCanvas.gameObject.SetActive(false);
        endGameCanvas.gameObject.SetActive(true);


        ScoreManager.instance.ResetScore();
        PlayerCollectibleManager.instance.AddCurrentCoinsToCoins();
        PlayerCollectibleManager.instance.ResetCoins();

        GameStarter.Instance.ChangeCamera();

        // Save mission progress
        // save coins
        // save scores 
        // save collectibles
        // maybe with actions so it doesnt depend on anything?
    }
    IEnumerator GoBackwards()
    {
        hitPos = transform.position;
        while (true)
        {
            if (Mathf.Abs(hitPos.z) - Mathf.Abs(transform.position.z) > backwardsThreshold)
            {
                break;
            }


            transform.position = Vector3.MoveTowards(transform.position, hitPos + Vector3.back * backwardsThreshold * 2, Time.deltaTime * backwardMoveSpeed);

            yield return null;
        }

    }

    private void DieWithTime()
    {
        gameObject.SetActive(false);
    }

    public void ResetChase()
    {
        _chaseStarted = false;
    }


    public bool IsPlayerDead()
    {
        return isDead;
    }



    public void ResetDeadState()
    {
        isPlayerTwisted = false;
        isInvincible = false;
        isDead = false;
        isRunFinished = false;
    }

}
