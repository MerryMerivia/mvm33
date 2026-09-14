using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicSetup", menuName = "Scriptable Objects/MusicSetup")]
public class SoundSetup : ScriptableObject
{
    [Header("Music")]

    public AudioClip Music;

    public float LoopStart;

    [Header("SFX")]
    public string SnapshotName;

}
