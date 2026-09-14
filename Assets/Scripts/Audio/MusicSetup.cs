using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicSetup", menuName = "Scriptable Objects/MusicSetup")]
public class MusicSetup : ScriptableObject
{
    public AudioClip Music;

    public float LoopStart;
}
