using System.Collections;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [SerializeField] private float rayCastMaxDistance = 2;
    public bool isWallR = false;
    public bool isWallL = false;
    private RaycastHit hitR;
    private RaycastHit hitL;
    private int jumpCount = 0;
    public PlayerMovement cc;
    public Rigidbody rb;
    public Transform cameraEffect;
    public Animator anim;
    public bool canJump;
    public float jumpForce = 25;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cc.grounded)
        {
            jumpCount = 0;
            isWallL = false;
            isWallR = false;
        }
        if (isWallR == true && isWallL == false)
        {

            anim.SetBool("Left", true);
        }
        if (isWallR == false)
        {

            anim.SetBool("Left", false);
        }
        if (isWallL == false)
        {

            anim.SetBool("Right", false);
        }
        if (isWallR == false && isWallL == true)
        {

            anim.SetBool("Right", true);
        }

        if (canJump == true && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (isWallL == true)
            {
                Vector3 force = this.transform.right * jumpForce;
                rb.AddForceAtPosition(force, this.transform.position, ForceMode.Impulse);
            }
            if (isWallR == true)
            {
                Vector3 force = -this.transform.right * jumpForce;
                rb.AddForceAtPosition(force, this.transform.position, ForceMode.Impulse);
            }
        }

        if (!cc.grounded)
        {
            if (Physics.Raycast(transform.position, transform.right, out hitR, rayCastMaxDistance))
            {
                if (hitR.transform.tag == "Wall")
                {
                    canJump = true;
                    isWallR = true;
                    isWallL = false;
                    jumpCount += 1;

                    rb.useGravity = false;
                }
            }
            if (!Physics.Raycast(transform.position, transform.right, out hitR, rayCastMaxDistance))
            {

                isWallR = false;
                jumpCount += 1;
                if (isWallL == false)
                {
                    canJump = false;
                    rb.useGravity = true;
                }
            }
            if (Physics.Raycast(transform.position, -transform.right, out hitL, rayCastMaxDistance))
            {
                if (hitL.transform.tag == "Wall")
                {
                    canJump = true;
                    isWallL = true;
                    jumpCount += 1;
                    rb.useGravity = false;
                }
            }
            if (!Physics.Raycast(transform.position, -transform.right, out hitL, rayCastMaxDistance))
            {

                isWallL = false;
                jumpCount += 1;
                if (isWallR == false)
                {
                    canJump = false;
                    rb.useGravity = true;
                }
            }
        }
    }
}