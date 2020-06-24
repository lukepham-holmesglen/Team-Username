using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float controlSpeed = 5f;
    [SerializeField] float jumpSpeed = 50000f;
    [SerializeField] float gravity = -100f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float sprintSpeed = 2f;
    [SerializeField] float hopAmount = 10f;

    Quaternion lastRotation;
    Rigidbody rigidBody;
    float xThrow, zThrow;
    bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        // we do this once to enable our collider, since for some reason it isn't always enabled 
        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        // comparison with true is redundant
        if (isGrounded)
        {
            ProcessJump();
        }
        
        ProcessRotation();
        ProcessTranslation();
        // why is gravity being set per frame? this seems extremely inefficient
        SetGravity();

    }
    
    private void ProcessTranslation()
    {
        xThrow = Input.GetAxis("Horizontal");
        zThrow = Input.GetAxis("Vertical");

        if (Input.GetButton("Fire3"))
        {
            ProcessSprint();
        }
        else
        {
            float xSpeed = xThrow * controlSpeed * Time.deltaTime;
            float zSpeed = zThrow * controlSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x + xSpeed, transform.position.y, transform.position.z + zSpeed);
        }
    }

    private void ProcessSprint()
    {
        float xSpeed = xThrow * controlSpeed * Time.deltaTime;
        float zSpeed = zThrow * controlSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x + xSpeed * sprintSpeed, transform.position.y, transform.position.z + zSpeed * sprintSpeed);
    }

    private void SetGravity()
    {
        Physics.gravity = new Vector3(0, gravity, 0);
    }

    private void ProcessRotation()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        }
    }

    private void ProcessJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            isGrounded = false;
            rigidBody.AddForce(Vector3.up * (jumpSpeed * Time.deltaTime));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // explicit string comparison is inefficient - use CompareTag instead
        // additionally, there's no need for a conditional here at all
        isGrounded = collision.gameObject.CompareTag("Ground");
    }
}
