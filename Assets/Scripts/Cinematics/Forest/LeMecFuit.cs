using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static TempCameraFocus;

public class LeMecFuit : CinematicEvent
{
    [SerializeField] private Transform mec;
    [SerializeField] private Transform destination;
    [SerializeField] private SpriteRenderer smiley;
    [SerializeField] private SpriteRenderer luneEmote;
    [SerializeField] private SpriteRenderer map;
    [SerializeField] private float mecSpeed;
    [SerializeField] private float tempsAvantSmiley;
    [SerializeField] private float tempsDeSmiley;
    [SerializeField] private float tempsAvantLune;
    [SerializeField] private float tempsDeLune;
    [SerializeField] private GameObject cinematicTuage;

    public override void Event()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {

        float temps = 0;

        // Le mec va sourire
        while (temps < this.tempsAvantSmiley)
        {
            temps += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        temps = 0;
        this.smiley.enabled = true;

        // Le mec arrête de sourire
        while (temps < this.tempsDeSmiley)
        {
            temps += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        this.smiley.enabled = false;
        temps = 0;

        // Le mec va remarquer que c'est la nuit ! Il faut fuir !
        while (temps < this.tempsAvantLune)
        {
            temps += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        temps = 0;
        this.luneEmote.enabled = true;

        while (temps < this.tempsDeLune)
        {
            temps += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        this.luneEmote.enabled = false;
        this.map.enabled = true;
        this.cinematicTuage.SetActive(true);

        // Donc il fuit
        while (this.mec.position != this.destination.position)
        {
            this.mec.position = Vector3.MoveTowards(this.mec.position, this.destination.position, this.mecSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
        
    }
}
