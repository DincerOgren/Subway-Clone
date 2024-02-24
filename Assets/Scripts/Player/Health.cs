using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    //[Header("Knockback")]
    //[SerializeField] float knockBackForce = 10f;
    //[SerializeField] float knockBackSpeed = 20f;


    bool startClean;

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
        if (Flags.isTakingDamage)
            startClean = true;

        if (invincibleTimer > invincibleTimeAfterDamage && startClean)
        {
            startClean = false;
            GetComponent<DamageCollisions>().ClearArray();
        }
        Flags.isTakingDamage = !(invincibleTimer > invincibleTimeAfterDamage);
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        invincibleTimer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        startClean = true;
        _currentHealth--;
        invincibleTimer = 0;
        //StartCoroutine(KnockBack());
        //Knockback
        UpdateUI();
        if (_currentHealth < 1)
        {
            Die();
            return;
        }
        StartCoroutine(DamageFlash());
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
        PlayerCollectibleManager.instance.ResetCoins();


        gameObject.SetActive(false);
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
