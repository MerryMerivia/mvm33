using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Collider2D collider;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float jumpImpulse;

    private float currentHorizontalMovement = 0;
    private bool wantsToJump = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.ReadInputs();
    }

    private void FixedUpdate()
    {
        this.MovePatate();
    }


    private void ReadInputs()
    {
        this.currentHorizontalMovement = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.wantsToJump = true;
        }
    }


    /**
     * Déplace horizontalement notre patate en fonction de l'accélération lue dans ReadInputs()
     */
    private void MovePatate()
    {
        this.transform.Translate(new Vector3(this.currentHorizontalMovement, 0, 0) * this.horizontalSpeed * Time.deltaTime);
        if (this.wantsToJump)
        {
            //if (this.rigidbody.totalForce.y > 0)
            {
                Debug.Log(this.rigidbody.totalForce);
                //this.rigidbody.AddForce(new Vector2(0, -this.rigidbody.totalForce.y), ForceMode2D.Impulse); // Pour que le double saut se fasse sur une base de force verticale nulle
                this.rigidbody.velocity = new Vector2(this.rigidbody.velocity.x, 0);
            }
            this.rigidbody.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
        }

        this.wantsToJump = false;
    }
}
