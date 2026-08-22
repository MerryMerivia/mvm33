using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eau : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            Torche torche = playerController.GetTorche();
            if (torche)
            {
                torche.gameObject.SetActive(false);
                playerController.SetTorche(null);
            }
        }
    }
}
