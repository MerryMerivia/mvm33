using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlayClipFromSource()
    {
        if(audioSource != null)
            audioSource.Play();
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(SFXManager))]
public class SFXManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();

        var entityHealth = target as SFXManager;

        if (GUILayout.Button("Play Clip"))
        {
            entityHealth.PlayClipFromSource();
        }
    }
}
#endif
