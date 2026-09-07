using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealth : MonoBehaviour
{
    public int MaxHealth;

    public int Health;

    [NonSerialized] public UnityEvent HealthChangeEvent = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Health == 0) Health = MaxHealth;
    }

    public void ChangeHealth(int health)
    {
        Health = Mathf.Clamp(Health + health, 0, MaxHealth);

        if(Health == 0)
        {
            // Déclenchement mort

        }

        // MAJ Affichage vie
        HealthChangeEvent.Invoke();
    }


}

#if UNITY_EDITOR
[CustomEditor(typeof(EntityHealth))]
public class EntityHealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        
        var entityHealth = target as EntityHealth;

        if (GUILayout.Button("Hurt"))
        {
            entityHealth.ChangeHealth(-1);
        }

        if (GUILayout.Button("Heal"))
        {
            entityHealth.ChangeHealth(1);
        }
    }
}
#endif