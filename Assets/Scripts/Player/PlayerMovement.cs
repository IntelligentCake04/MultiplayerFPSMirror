using System;
using Mirror;
using UnityEngine;

namespace IntelligentCake.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        private bool cancellingGrounded;

        public float counterMovement = 0.175f;

        //Crouch & Slide
        private readonly Vector3 crouchScale = new Vector3(1, 0.5f, 1);

        private float desiredX;
        public bool grounded;
        private readonly float jumpCooldown = 0.25f;
        public float jumpForce = 550f;
        private bool jumping, sprinting, crouching;
        public float maxSlopeAngle = 35f;
        public float maxSpeed = 20f;

        //Movement
        public float moveSpeed = 4500f;

        //Sliding
        private Vector3 normalVector = Vector3.up;
        public Transform orientation;

        //Assingables
        public Transform playerCam;
        private Vector3 playerScale;

        //Other
        private Rigidbody rb;

        //Jumping
        private bool readyToJump = true;
        private readonly float sensitivity = 50f;
        private readonly float sensMultiplier = 1f;
        public float slideCounterMovement = 0.2f;
        public float slideForce = 400;
        private readonly float threshold = 0.01f;
        private Vector3 wallNormalVector;
        public LayerMask whatIsGround;

        //Input
        private float x, y;

        //Rotation and look
        private float xRotation;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            playerScale = transform.localScale;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        [Client]
        private void FixedUpdate()
        {
            if (!hasAuthority) return;
            Movement();
        }

        [Client]
        private void Update()
        {
            if (!hasAuthority) return;
            MyInput();
            Look();
        }

        /// <summary>
        ///     Find user input. Should put this in its own class but im lazy
        /// </summary>
        private void MyInput()
        {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
            jumping = Input.GetButton("Jump");
            crouching = Input.GetKey(KeyCode.LeftControl);

            //Crouching
            if (Input.GetKeyDown(KeyCode.LeftControl))
                StartCrouch();
            if (Input.GetKeyUp(KeyCode.LeftControl))
                StopCrouch();
        }

        private void StartCrouch()
        {
            transform.localScale = crouchScale;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            if (rb.velocity.magnitude > 0.5f)
                if (grounded)
                    rb.AddForce(orientation.transform.forward * slideForce);
        }

        private void StopCrouch()
        {
            transform.localScale = playerScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }

        private void Movement()
        {
            //Extra gravity
            rb.AddForce(Vector3.down * Time.deltaTime * 10);

            //Find actual velocity relative to where player is looking
            var mag = FindVelRelativeToLook();
            float xMag = mag.x, yMag = mag.y;

            //Counteract sliding and sloppy movement
            CounterMovement(x, y, mag);

            //If holding jump && ready to jump, then jump
            if (readyToJump && jumping) Jump();

            //Set max speed
            var maxSpeed = this.maxSpeed;

            //If sliding down a ramp, add force down so player stays grounded and also builds speed
            if (crouching && grounded && readyToJump)
            {
                rb.AddForce(Vector3.down * Time.deltaTime * 3000);
                return;
            }

            //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (x > 0 && xMag > maxSpeed) x = 0;
            if (x < 0 && xMag < -maxSpeed) x = 0;
            if (y > 0 && yMag > maxSpeed) y = 0;
            if (y < 0 && yMag < -maxSpeed) y = 0;

            //Some multipliers
            float multiplier = 1f, multiplierV = 1f;

            // Movement in air
            if (!grounded)
            {
                multiplier = 0.5f;
                multiplierV = 0.5f;
            }

            // Movement while sliding
            if (grounded && crouching) multiplierV = 0f;

            //Apply forces to move player
            rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        }

        private void Jump()
        {
            if (grounded && readyToJump)
            {
                readyToJump = false;

                //Add jump forces
                rb.AddForce(Vector2.up * jumpForce * 1.5f);
                rb.AddForce(normalVector * jumpForce * 0.5f);

                //If jumping while falling, reset y velocity.
                var vel = rb.velocity;
                if (rb.velocity.y < 0.5f)
                    rb.velocity = new Vector3(vel.x, 0, vel.z);
                else if (rb.velocity.y > 0)
                    rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void ResetJump()
        {
            readyToJump = true;
        }

        private void Look()
        {
            playerCam = Camera.main.transform;
            var mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
            var mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

            //Find current look rotation
            var rot = playerCam.transform.localRotation.eulerAngles;
            desiredX = rot.y + mouseX;

            //Rotate, and also make sure we dont over- or under-rotate.
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //Perform the rotations
            playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
            orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
        }

        private void CounterMovement(float x, float y, Vector2 mag)
        {
            if (!grounded || jumping) return;

            //Slow down sliding
            if (crouching)
            {
                rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
                return;
            }

            //Counter movement
            if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || mag.x < -threshold && x > 0 ||
                mag.x > threshold && x < 0)
                rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
            if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || mag.y < -threshold && y > 0 ||
                mag.y > threshold && y < 0)
                rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);

            //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
            if (Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2)) > maxSpeed)
            {
                var fallspeed = rb.velocity.y;
                var n = rb.velocity.normalized * maxSpeed;
                rb.velocity = new Vector3(n.x, fallspeed, n.z);
            }
        }

        /// <summary>
        ///     Find the velocity relative to where the player is looking
        ///     Useful for vectors calculations regarding movement and limiting movement
        /// </summary>
        /// <returns></returns>
        public Vector2 FindVelRelativeToLook()
        {
            var lookAngle = orientation.transform.eulerAngles.y;
            var moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

            var u = Mathf.DeltaAngle(lookAngle, moveAngle);
            var v = 90 - u;

            var thing = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
            var magnitue = rb.velocity.magnitude;
            var yMag = thing * Mathf.Cos(u * Mathf.Deg2Rad);
            var xMag = thing * Mathf.Cos(v * Mathf.Deg2Rad);

            return new Vector2(xMag, yMag);
        }

        private bool IsFloor(Vector3 v)
        {
            var angle = Vector3.Angle(Vector3.up, v);
            return angle < maxSlopeAngle;
        }

        /// <summary>
        ///     Handle ground detection
        /// </summary>
        private void OnCollisionStay(Collision other)
        {
            //Make sure we are only checking for walkable layers
            var layer = other.gameObject.layer;
            if (whatIsGround != (whatIsGround | (1 << layer))) return;

            //Iterate through every collision in a physics update
            for (var i = 0; i < other.contactCount; i++)
            {
                var normal = other.contacts[i].normal;
                //FLOOR
                if (IsFloor(normal))
                {
                    grounded = true;
                    cancellingGrounded = false;
                    normalVector = normal;
                    CancelInvoke(nameof(StopGrounded));
                }
            }

            //Invoke ground/wall cancel, since we can't check normals with CollisionExit
            var delay = 3f;
            if (!cancellingGrounded)
            {
                cancellingGrounded = true;
                Invoke(nameof(StopGrounded), Time.deltaTime * delay);
            }
        }

        private void StopGrounded()
        {
            grounded = false;
        }
    }
}