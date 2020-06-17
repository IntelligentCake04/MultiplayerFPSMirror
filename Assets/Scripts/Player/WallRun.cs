using UnityEngine;

namespace IntelligentCake.Player
{
    public class WallRun : MonoBehaviour
    {
        public Animator anim;
        public Transform cameraEffect;
        public bool canJump;
        public PlayerMovement cc;
        private RaycastHit hitL;
        private RaycastHit hitR;
        public bool isWallL;
        public bool isWallR;
        private int jumpCount;
        public float jumpForce = 25;
        [SerializeField] private float rayCastMaxDistance = 2;
        public Rigidbody rb;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (cc.grounded)
            {
                jumpCount = 0;
                isWallL = false;
                isWallR = false;
            }

            if (isWallR && isWallL == false) anim.SetBool("Left", true);
            if (isWallR == false) anim.SetBool("Left", false);
            if (isWallL == false) anim.SetBool("Right", false);
            if (isWallR == false && isWallL) anim.SetBool("Right", true);

            if (canJump && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                if (isWallL)
                {
                    var force = transform.right * jumpForce;
                    rb.AddForceAtPosition(force, transform.position, ForceMode.Impulse);
                }

                if (isWallR)
                {
                    var force = -transform.right * jumpForce;
                    rb.AddForceAtPosition(force, transform.position, ForceMode.Impulse);
                }
            }

            if (!cc.grounded)
            {
                if (Physics.Raycast(transform.position, transform.right, out hitR, rayCastMaxDistance))
                    if (hitR.transform.tag == "Wall")
                    {
                        canJump = true;
                        isWallR = true;
                        isWallL = false;
                        jumpCount += 1;

                        rb.useGravity = false;
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
                    if (hitL.transform.tag == "Wall")
                    {
                        canJump = true;
                        isWallL = true;
                        jumpCount += 1;
                        rb.useGravity = false;
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
}