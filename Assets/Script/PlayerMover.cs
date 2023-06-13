using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] bool debug;

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float walkStepRange;
    [SerializeField] float runStepRange;

    private CharacterController controller;
    private Animator anim;
    private Vector3 moveDir;
    private float curSpeed;
    private float ySpeed;
    private bool walk;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        Fall();
    }
    float lastStepTime = 0.5f;
    private void Move()
    {
        if (moveDir.magnitude == 0)
        {
            curSpeed = Mathf.Lerp(curSpeed, 0, 0.1f);
            anim.SetFloat("MoveSpeed", curSpeed);
            return;
        }

        Vector3 forwardVec = new Vector3(Camera.main.transform.forward.x,0,Camera.main.transform.forward.z).normalized;
        Vector3 rightVec = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;

        if (walk)
        {
            curSpeed = Mathf.Lerp(curSpeed, walkSpeed, 0.1f);
        }
        else
        {
            curSpeed = Mathf.Lerp(curSpeed, runSpeed, 0.1f);
        }

        controller.Move(forwardVec * moveDir.z * curSpeed * Time.deltaTime);
        controller.Move(rightVec * moveDir.x * curSpeed * Time.deltaTime);
        anim.SetFloat("MoveSpeed", curSpeed);

        Quaternion lookRotation = Quaternion.LookRotation(forwardVec * moveDir.z + rightVec * moveDir.x);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);

        lastStepTime -= Time.deltaTime;
        if(lastStepTime < 0)
        {
            lastStepTime = 0.5f;
            GenerateFootStepSound();
        }
    }

    private void GenerateFootStepSound()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, walk ? walkStepRange: runStepRange);
        foreach(Collider collider in colliders)
        {
            IListenable listnable = collider.GetComponent<IListenable>();
            listnable?.Listen(transform);
        }
    }
    private void OnMove(InputValue value)
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;
    }

    private void OnWalk(InputValue value)
    {
        walk = value.isPressed;
    }

    private void Fall()
    {
        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (controller.isGrounded && ySpeed < 0)
            ySpeed = 0;

        controller.Move(Vector3.up * ySpeed * Time.deltaTime);
    }

    private void Jump()
    {
        ySpeed = jumpSpeed;
    }

    private void OnJump(InputValue value)
    {
        Jump();
    }

    private void OnDrawGizmosSelected()
    {
        if(!debug)
            return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, walkStepRange);
        Gizmos.DrawWireSphere (transform.position, runStepRange);
    }
}
