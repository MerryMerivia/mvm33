using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public int MaxHealth;

    public int Health;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(Health == 0) Health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeHealth(int health)
    {
        Health = Mathf.Max(Health + health, 0);

        if(Health == 0)
        {
            // Déclenchement mort
        }
            
        // MAJ Affichage vie
    }
}
