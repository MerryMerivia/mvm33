using UnityEngine;

/**
 * Classe qui détecte si elle est dans un mur
 */
public class WallClipChecker : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;

    public bool IsInsideWall()
    {
        return (this.boxCollider.IsTouchingLayers(1 << 6));
    }
}
