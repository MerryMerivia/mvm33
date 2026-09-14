using UnityEngine;

[CreateAssetMenu(fileName = "SFXSetup", menuName = "Scriptable Objects/SFXSetup")]
public class SFXSetup : ScriptableObject
{
    [Range(-10000, 0)]
    public float DryLevel;

    [Range(-10000, 0)]
    public float RoomLevel;

    [Range(0.1f, 20)]
    public float DecayTime;

    [Range(-10000, 1000)]
    public float Reflections;
}
