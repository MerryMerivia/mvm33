using UnityEngine;

[CreateAssetMenu(fileName = "EntityType", menuName = "Scriptable Objects/EntityType")]
public class EntityTypeSO : ScriptableObject
{
    public EntityType Type;

    public Sprite Sprite;
}

public enum EntityType
{
    PLAYER = 0,
    QUEUE = 1,
    ROULANT = 2,
    VOLANT = 3,
}
