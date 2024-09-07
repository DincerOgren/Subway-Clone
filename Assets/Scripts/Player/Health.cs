using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] Transform healthContainer;
    [SerializeField] GameObject healthImagePrefab;

    [Header("Health Variables")]
    [SerializeField] float maxHealth = 3;
    float _currentHealth;

    [Header("UI")]
    [SerializeField] Canvas inGameCanvas;
    [SerializeField] Canvas endGameCanvas;
    [SerializeField] TextMeshProUGUI endScore;
    [SerializeField] TextMeshProUGUI endHighScore;

    [Header("Damage Flash")]
    [SerializeField] float flashTime;
    [SerializeField] Transform playerMaterialRef;
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float invincibleTimeAfterDamage = 1f;
    [SerializeField] float flashAmount = 1f;
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


    ChasingEnemy enemyRef;
    bool _chaseStarted;
    //[Header("Knockback")]
    //[SerializeField] float knockBackForce = 10f;
    //[SerializeField] float knockBackSpeed = 20f;


    bool startClean;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        enemyRef = GameObject.FindWithTag("Enemy").GetComponent<ChasingEnemy>();
    }

    private void Start()
    {
        _currentHealth = maxHealth;

        for (int i = 0; i < maxHealth; i++)
        {
            Instantiate(healthImagePrefab, healthContainer);
        }
        _playerMat = playerMaterialRef.GetComponent<SkinnedMeshRenderer>().material;
    }

    private void Update()
    {
        //if (Flags.isTakingDamage)
        //    startClean = true;

        //if (invincibleTimer > invincibleTimeAfterDamage && startClean)
        //{
        //    startClean = false;
        //    GetComponent<DamageCollisions>().ClearArray();
        //}
        //Flags.isTakingDamage = !(invincibleTimer > invincibleTimeAfterDamage);
        UpdateTimers();

        CheckError();
        CheckPlayerState();
        CheckInvincible();

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
            enemyRef.gameObject.SetActive(true);

            enemyRef.StartChase(transform.position);
            _chaseStarted = true;
        
        }
        if (_twistedTimer >= twistedTime)
        {
            isPlayerTwisted = false;
            // enemyRef.StartChase(false);
        }
       

        //print("PLAYER TWISTEDDDDDD");
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

        if (collision.collider.CompareTag("TrainFront"))
        {
            // If the player hits the front, they die immediately
            if (!isInErrorPeriod)
            {
                StartErrorPeriod();
            }
            else
            {
                Die();
            }
        }
        else if (collision.collider.CompareTag("TrainSide"))
        {
            // If the player hits the side, enter the twisted state
            if (!isPlayerTwisted)
            {
                EnterTwistedState();
            }
            else
            {
                // If already twisted, the player dies
                Die();
            }
        }

    }
    private void EnterTwistedState()
    {
        isPlayerTwisted = true;
        _twistedTimer = 0;
        // Start shaking the player
        //StartCoroutine(ShakePlayer());

        // Move the player back to the previous lane after the shake
        playerMovement.MoveToPreviousLane();
        //isPlayerTwisted = false;
        //print("PLAYER TWISTEDDDDDD");
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
                RecoverFromHit();
                return;
            }
        }

        if (_errorTimer < 0)
        {
            EndErrorPeriod();
        }
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
        playerMovement.SetPlayerSpeed(oldSpeed); // Restore player speed
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
        Debug.Log("End Of Error Period");
        playerMovement.EnableSwipeDetectionDuringErrorPeriod(false); // Disable error period swipe detection
        playerMovement.SetPlayerSpeed(oldSpeed); // Restore player speed
        oldSpeed = 0f;
        Die();
    }

    private void TakeDamage()
    {
        //startClean = true;
        //_currentHealth--;
        //invincibleTimer = 0;
        ////StartCoroutine(KnockBack());
        ////Knockback
        //UpdateUI();
        //if (_currentHealth < 1)
        //{
        //    Die();
        //    return;
        //}
        //StartCoroutine(DamageFlash());

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


    private IEnumerator DamageFlash()
    {
        SetFlashColor();
        while (invincibleTimer < invincibleTimeAfterDamage)
        {

            float currentFlashAmount = (Mathf.Sin(flashAmount * Time.time) + 1) / 2;
            //print(currentFlashAmount);
            _playerMat.SetFloat("_FlashAmount", currentFlashAmount);
            yield return null;
        }

        _playerMat.SetFloat("_FlashAmount", 0);

    }

    private void SetFlashColor()
    {
        _playerMat.SetColor("_FlashColor", flashColor);
    }


    #endregion
    private void Die()
    {
        if (isDead) { return; }

        isDead = true;

        ScoreManager.instance.SaveScore();
        //Play death anim

        endScore.text = ScoreManager.instance.GetScore().ToString();
        endHighScore.text = ScoreManager.instance.GetHighScore().ToString();

        inGameCanvas.gameObject.SetActive(false);
        endGameCanvas.gameObject.SetActive(true);


        ScoreManager.instance.ResetScore();
        PlayerCollectibleManager.instance.AddCurrentCoinsToCoins();
        PlayerCollectibleManager.instance.ResetCoins();


        gameObject.SetActive(false);
    }

    public void ResetChase()
    {
        _chaseStarted = false;
    }

    private void UpdateUI()
    {
        foreach (Transform child in healthContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _currentHealth; i++)
        {
            Instantiate(healthImagePrefab, healthContainer);

        }
    }
    public bool IsPlayerDead()
    {
        return isDead;
    }
}
