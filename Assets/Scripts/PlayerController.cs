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

    Quaternion lookAtDir;
    Quaternion lastRotation;
    Rigidbody rigidBody;
    float xThrow, zThrow;
    bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        lookAtDir = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded == true)
        {
            ProcessJump();
        }
        ProcessRotation();
        ProcessTranslation();
        SetGravity();

    }


    private void ProcessHopping()
    {
        rigidBody.AddForce(Vector3.up * hopAmount * Time.deltaTime);
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
            rigidBody.AddForce(Vector3.up * jumpSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
