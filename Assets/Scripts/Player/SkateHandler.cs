using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateHandler : MonoBehaviour
{

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
    [SerializeField] bool _isSkating;

    [Header("Skate Contact")]
    [SerializeField] float disableSphrereRange = 5;
    [SerializeField] float disableLength= 10f;
    [SerializeField] LayerMask disablingLayer;

    public Transform playerModel;
    
    public bool onSkateCollision;
    Animator _anim;
    private void Awake()
    {
        _anim=GetComponent<Animator>();
    }
    private void Start()
    {

        skateData.Duration = totalSkateTime;
    }

    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.F))
        {
            TryToUseSkate();
        }
        HandleSkate();
        UpdateAnimator();

        //Delete later
        if (onSkateCollision)
        {
            onSkateCollision = false;
            OnSkateContact();
        }
    }

    public void OnSkateContact()
    {

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, disableSphrereRange, transform.forward, disableLength, disablingLayer);
        List<GameObject> processedObjects = new();


        foreach (var hit in hits)
        {
            Transform currentObject = hit.collider.transform;

            GameObject targetObject;

            // Parent varsa, onu kontrol et; yoksa objenin kendisini kontrol et
            //if (currentObject.parent != null && currentObject.parent.gameObject.layer == disablingLayer)
            //{

            //    targetObject = currentObject.parent.gameObject;
            //}
            //else if (currentObject.parent == null  && currentObject.gameObject.layer == disablingLayer)
            //{
            //    targetObject = currentObject.gameObject;
            //}
            //else
            //{
            //    continue;
            //}
            if (((1 << currentObject.gameObject.layer) & disablingLayer) != 0)
            {
                print("First if ");
                if (currentObject.parent.gameObject.layer == disablingLayer)
                {
                    print("parent if");

                    targetObject = currentObject.parent.gameObject;
                }
                else
                {
                    targetObject = currentObject.gameObject;
                    print("else if");

                }
            }
            else
            {
                print("Layer Uymuyor else ");
                continue;
            }

            if (!processedObjects.Contains(targetObject))
            {
                processedObjects.Add(targetObject);

                Debug.Log("Processing and deactivating: " + targetObject.name);
                targetObject.SetActive(false);
            }

            // disable Object on radius 
            //get rid of police
            //vfx
            // - skate
            // end skate state

        }


        // Stop Skate Using
        PowerUpManager.instance.StopPowerupImmediate();
    }

    public void TryToUseSkate()
    {
        if(!_isSkating)
            _shouldUseSkate = true;
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
            StartSkate();
            _shouldUseSkate = false;
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
    private void UpdateAnimator()
    {
        _anim.SetBool("isSkateboarding", _isSkating);

    }

    public bool IsSkating()
    {
        return _isSkating;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, disableSphrereRange);

        Gizmos.DrawRay(transform.position, transform.forward * disableLength);

    }
}
