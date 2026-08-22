using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeMecTueEnnemi : MonoBehaviour
{
    [SerializeField] private Transform mec;
    [SerializeField] private SpriteRenderer smiley;
    [SerializeField] private SpriteRenderer ennemi;
    [SerializeField] private float tempsAvantSmiley;
    [SerializeField] private float tempsDeSmiley;
    [SerializeField] private float tempsAvantTuerEnemmi;
    [SerializeField] private float mecSpeed;
    [SerializeField] private Transform departMec;
    [SerializeField] private Transform arriveeMec;

    private bool active = false;

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
        if (!active && collision.CompareTag("Player"))
        {
            this.mec.position = this.departMec.position;
            StartCoroutine(ILVALETUUUUUUUEEEEER());
            this.active = true;
        }
    }

    private IEnumerator ILVALETUUUUUUUEEEEER()
    {
        float temps = 0;

        // Le mec va pas être content
        while (temps < this.tempsAvantSmiley)
        {
            temps += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        temps = 0;
        this.smiley.enabled = true;

        // Le mec cache son smiley
        while (temps < this.tempsDeSmiley)
        {
            temps += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        this.smiley.enabled = false;
        temps = 0;

        // Et il va tuer la créature ! Le fourbe !
        while (temps < this.tempsAvantTuerEnemmi)
        {
            temps += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        this.ennemi.transform.Translate(0, -0.5f, 0);
        this.ennemi.transform.Rotate(0, 0, -90);

        // Puis bah il continue sa route
        while (this.mec.position !=  this.arriveeMec.position)
        {
            this.mec.position = Vector3.MoveTowards(this.mec.position, this.arriveeMec.position, mecSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
