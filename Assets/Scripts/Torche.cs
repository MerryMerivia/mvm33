using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torche : MonoBehaviour
{
    [SerializeField] private Transform porteur;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.porteur)
        {
;            this.transform.position = porteur.position;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.porteur = collision.GetComponent<Transform>();
            collision.transform.GetComponent<PlayerController>().SetTorche(this);
            //this.gameObject.SetActive(false);
        }
    }
}
