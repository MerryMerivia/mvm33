using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public bool isThereWall { get; private set; } = false;
    public bool jumpAvailable = true;

    [SerializeField] BoxCollider2D boxCollider;

    private void Update()
    {
        this.CanWallJump();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.lay ("Ground"))
        {
            LayerMask tmp = (this.boxCollider.contactCaptureLayers);
            this.isThereWall = true;
        }*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground")) // TODO: Dans les faits, peut causer un faux négatif si on quitte un mur mais qu'il y en a un autre. Mais c'est un problème pour le moi de plus tard
        {
            this.isThereWall = false;
        }
    }

    public bool CanWallJump()
    {
        bool canJump = false;

        if (this.jumpAvailable && this.IsThereWall())
        {
            canJump = true;
        }

        return canJump;
    }

    public bool IsThereWall()
    {
        return (this.boxCollider.IsTouchingLayers(1 << 6));
    }

    public void Flip(bool left)
    {
        int direction = (left ? -1 : 1);
        //this.boxCollider.offset = new Vector2(direction * Mathf.Abs(this.boxCollider.offset.x), this.boxCollider.offset.y);
        this.transform.localScale = new Vector3(direction, 1, 1);
    }
}
