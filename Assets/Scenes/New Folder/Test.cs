using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public float moveSpeed = 5f;
    Rigidbody rb;

    Vector3 _moveDir;

    public float jumpForce = 25f;

    bool isJumping = false;
    bool isGrounded = false;
    private void Awake()
    {
        print("Awake");

        rb = GetComponent<Rigidbody>();

    }


    private void OnEnable()
    {
        Debug.Log("Onenable");

    }

    private void OnDisable()
    {
        print("ondisable");

    }
    // Start is called before the first frame update


    void Start()
    {
        print("start");

    }

    // Update is called once per frame
    void Update()
    {
        print("update");

        CheckInput();
       // MoveTranslate();
        Jump();
        
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isJumping = true;
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Force);
            moveSpeed -= 3;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            moveSpeed = 50;
        }
    }

    

    private void MoveTranslate()
    {
        transform.Translate(_moveDir * moveSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!isJumping)
        {
            rb.velocity = _moveDir.normalized * moveSpeed * Time.deltaTime;

        }
    }

    void CheckInput()
    {
        _moveDir.x = Input.GetAxis("Horizontal"); // 1 0 -1
        _moveDir.z = Input.GetAxis("Vertical");
        
    }
}
