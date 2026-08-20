using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Serializable]
    public struct PhysicsVariables
    {
        public float walkSpeed;
        public float delayBeforeNormalGravity;
        public float jumpImpulse;
        public float normalGravity;
        public float jumpingGravity;
        public float maxFallingSpeed;

        public PhysicsVariables(float walkSpeed, float delayBeforeNormalGravity, float jumpImpulse, float normalGravity, float jumpingGravity, float maxFallingSpeed)
        {
            this.walkSpeed = walkSpeed;
            this.delayBeforeNormalGravity = delayBeforeNormalGravity;
            this.jumpImpulse = jumpImpulse;
            this.normalGravity = normalGravity;
            this.jumpingGravity = jumpingGravity;
            this.maxFallingSpeed = maxFallingSpeed;
        }
    }

    [SerializeField] PhysicsVariables currentPhysic;
    [SerializeField] PhysicsVariables normalPhysic;
    [SerializeField] PhysicsVariables underWaterPhysic;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private AnimationClip deathAnimationClip;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Collider2D collider;

    private float lastTimeSinceJumped = 0;
    private bool wantsToJump = false;
    private float timeSinceGrounded = 0f;
    private float coyoteJumpWindow = 0.1f;
    public bool[] closeToWalls = new bool[2] { false, false };

    private bool canMove = true;


    void Awake()
    {
        Application.targetFrameRate = 60;
        this.currentPhysic = this.normalPhysic;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.canMove)
        {
            CheckMovement();
        }
    }

    private void FixedUpdate()
    {
        this.MovePatate();
    }

    private void CheckMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            wantsToJump = true;
        }

        if (rigidbody.gravityScale == this.currentPhysic.jumpingGravity)
        {
            lastTimeSinceJumped += Time.deltaTime;

            // Arrêt de gravité de saut
            if (Input.GetKeyUp(KeyCode.Space) || lastTimeSinceJumped > this.currentPhysic.delayBeforeNormalGravity)
            {
                rigidbody.gravityScale = this.currentPhysic.normalGravity;
                lastTimeSinceJumped = 0f;
            }
        }
    }

    /**
     * Déplace notre patate en fonction de l'accélération lue dans CheckMovements()
     */
    private void MovePatate()
    {
        // Le saut
        if ((IsGrounded() || this.timeSinceGrounded < this.coyoteJumpWindow) && wantsToJump && this.rigidbody.velocity.y < 0.1f)
        {
            //animator.SetBool("IsJumping", true);
            //audioManager.PlayClip(audioManager.jumpClip);
            this.rigidbody.velocity = new Vector2(this.rigidbody.velocity.x, 0);
            rigidbody.AddForce(Vector2.up * this.currentPhysic.jumpImpulse, ForceMode2D.Impulse);
            rigidbody.gravityScale = this.currentPhysic.jumpingGravity;
        }
        wantsToJump = false;

        // Les directions
        if ((Input.GetAxis("Horizontal") < -0.01f && !this.closeToWalls[0]) || (Input.GetAxis("Horizontal") > 0.01f && !this.closeToWalls[1]))   // Pour corriger le bug d'entrée partielle dans le mur
        {
            transform.Translate(this.currentPhysic.walkSpeed * Input.GetAxis("Horizontal") * Time.deltaTime * Vector3.right);
        }

        if (Input.GetAxis("Horizontal") > 0.01f)
        {
            //animator.SetInteger("Direction", 1);
            //animator.SetBool("IsMoving", true);
        }
        else if (Input.GetAxis("Horizontal") < -0.01f)
        {
            //animator.SetInteger("Direction", -1);
            //animator.SetBool("IsMoving", true);
        }
        else
        {
            //animator.SetBool("IsMoving", false);
        }

        // Vitesse max de chute. Pas possible d'éditer directement les composants de rigidbody.velocity
        if (this.rigidbody.velocity.y < this.currentPhysic.maxFallingSpeed)
        {
            this.rigidbody.velocity = new Vector2(this.rigidbody.velocity.x, this.currentPhysic.maxFallingSpeed);
        }
    }

    private bool IsGrounded()
    {
        float checkExtent = 0.03f;
        RaycastHit2D castHit = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size - new Vector3(0.1f, 0, 0), 0f, Vector2.down, checkExtent, groundMask);
        bool isGrounded = castHit.collider != null;

        // Atterissage
        //if (isGrounded && animator.GetBool("IsAirborne"))
        {
            //audioManager.PlayClip(audioManager.landingClip);
            //animator.SetBool("IsJumping", false);
        }

        //animator.SetBool("IsAirborne", !isGrounded);

        if (isGrounded)
        {
            this.rigidbody.gravityScale = this.currentPhysic.normalGravity;
            this.timeSinceGrounded = 0f;
            this.lastTimeSinceJumped = 0f;
        }

        if (!isGrounded && Math.Abs(rigidbody.velocity.y) > 0.1f)
        {
            //animator.SetBool("IsAirborne", true);
            this.timeSinceGrounded += Time.deltaTime;
        }

        Color rayColor = (isGrounded ? Color.green : Color.red);

        Debug.DrawRay(collider.bounds.center + new Vector3(collider.bounds.extents.x, 0), Vector2.down * (collider.bounds.extents.y + checkExtent), rayColor);
        Debug.DrawRay(collider.bounds.center - new Vector3(collider.bounds.extents.x, 0), Vector2.down * (collider.bounds.extents.y + checkExtent), rayColor);
        Debug.DrawRay(collider.bounds.center - new Vector3(collider.bounds.extents.x, collider.bounds.extents.y + checkExtent), Vector2.right * (collider.bounds.extents.x * 2f), rayColor);

        return isGrounded;
    }

    public void SetActive(bool activate)
    {
        canMove = activate;
        rigidbody.gravityScale = (activate ? (Input.GetKey(KeyCode.Space) ? this.currentPhysic.jumpingGravity : this.currentPhysic.normalGravity) : 0);
    }

    public void SetWaterPhysics(bool activate)
    {
        this.currentPhysic = (activate ? this.underWaterPhysic : this.normalPhysic);
        this.rigidbody.gravityScale = this.currentPhysic.jumpingGravity;
        this.rigidbody.AddForce(Vector2.up * this.currentPhysic.jumpImpulse / 4, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Permet de déclarer que le joueur est collé aux murs. Utilisé notamment pour éviter de clip dans les bords
    /// </summary>
    /// <param name="direction">Direction : -1 pour gauche, 0 pour aucune, 1 pour droite</param>
    public void SetWallsProximity(int direction)
    {
        switch (direction)
        {
            case -1:    // Gauche
                this.closeToWalls = new bool[] { true, false };
                break;
            case 0:     // Aucune direction
                this.closeToWalls = new bool[] { false, false };
                break;
            case 1:     // Droite
                this.closeToWalls = new bool[] { false, true };
                break;
        }
    }


}
