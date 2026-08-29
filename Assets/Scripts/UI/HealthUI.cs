using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HealthUI : MonoBehaviour
{
    public EntityHealth Player;

    [Header("Sprites")]
    [SerializeField] private Sprite healthFull;
    [SerializeField] private Sprite healthEmpty;

    private Image[] healthUnits;
    
    void Awake()
    {
        healthUnits = GetComponentsInChildren<Image>(true);
    }

    private void Start()
    {
        // 0 is parent sprite because GetComponentsInChildren also gets self
        for (int i = 1; i < healthUnits.Length; i++)
        {
            if(i <= Player.MaxHealth)
            {
                healthUnits[i].gameObject.SetActive(true);

                if (i <= Player.Health)
                    healthUnits[i].sprite = healthFull;
                else
                    healthUnits[i].sprite = healthEmpty;
            }
            else
            {
                healthUnits[i].gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
