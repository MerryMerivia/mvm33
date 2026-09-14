using UnityEngine;

[RequireComponent(typeof(EntityHealth))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[ExecuteInEditMode]
public class Entity : MonoBehaviour
{
    public EntityTypeSO EntityType;

    SpriteRenderer spriteRenderer;
    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(EntityType != null)
            spriteRenderer.sprite = EntityType.Sprite;
    }
    
    //TODO : Logique de comportement


    // Update is called once per frame
    void Update()
    {
        
    }
}
