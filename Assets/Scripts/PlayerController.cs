using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float controlSpeed = 0.1f;
    [SerializeField] float jumpSpeed = 50000f;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float sprintSpeed = 2f;
    
    Quaternion lastRotation;
    Transform animationSlave;
    Rigidbody rigidBody;
    Animator animator;
    float xThrow, zThrow;
    bool isGrounded = true;
    int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 16;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.mass = 10000f;
        // we do this once to enable our collider, since for some reason it isn't always enabled
        // prefab structure may not be consistent so do some enabling/ disabling of things here too
        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
        animator = GetComponentInChildren<Animator>();
        animationSlave = animator.transform;
        Collider badCollider = animationSlave.GetComponent<Collider>();
        badCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessRotation();
        
        xThrow = Input.GetAxisRaw("Horizontal");
        zThrow = Input.GetAxisRaw("Vertical");

        if (xThrow != 0 | zThrow != 0)
        {
            // we are moving
            if (isGrounded)
            {
                // let the animation play normally; we're on the ground and moving
                animator.enabled = true;
            }
            else
            {
                // don't play animation in the air
                // repeating code, yeah yeah i know
                // reset the animation, and then stop it
                animationSlave.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                animationSlave.localPosition =
                    new Vector3(animationSlave.localPosition.x, -0.454f, animationSlave.localPosition.z);
                animator.enabled = false;   
            }
        }
        else
        {
            // reset the animation, and then stop it
            animationSlave.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            animationSlave.localPosition =
                new Vector3(animationSlave.localPosition.x, -0.454f, animationSlave.localPosition.z);
            animator.enabled = false;   
        }
    }

    private void FixedUpdate()
    {
        Vector3 rayOrign = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        isGrounded = Physics.Raycast(rayOrign, Vector3.down, 1.2f, layerMask);
        Debug.DrawLine(rayOrign, rayOrign + Vector3.down);

        ProcessMovement();
        
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 8.0f, rigidBody.velocity.z);
        }
    }

    private void ProcessMovement()
    {
        float sprintModifier = Input.GetButton("Fire3") ? sprintSpeed : 1.0f;
        xThrow = Input.GetAxisRaw("Horizontal") * (controlSpeed * sprintModifier);
        zThrow = Input.GetAxisRaw("Vertical") * (controlSpeed * sprintModifier);

        if (xThrow != 0 | zThrow != 0)
        {
            Vector3 movementVector = new Vector3(xThrow, rigidBody.velocity.y, zThrow);
            rigidBody.velocity = movementVector;
            animator.enabled = true;
        }
        else
        {
            // no key is pressed, so cut movement
            Vector3 currentMovement = rigidBody.velocity;
            Vector3 inverseMovement = new Vector3(-currentMovement.x, 0f, -currentMovement.z);
            rigidBody.AddForce(inverseMovement, ForceMode.VelocityChange);
        }
    }

    private void ProcessRotation()
    {
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        }
    }
}
