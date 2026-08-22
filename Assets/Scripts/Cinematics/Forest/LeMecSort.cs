using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TempCameraFocus;

public class LeMecSort : CinematicEvent
{
    [SerializeField] private Transform mec;
    [SerializeField] private Transform[] destinations;
    [SerializeField] private GameObject attache;
    [SerializeField] private float mecSpeed;

    public override void Event()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {

        for (int i = 0; i < destinations.Length; i++)
        {
            // Déplacement de la caméra
            while (this.mec.position != this.destinations[i].position)
            {
                this.mec.position = Vector3.MoveTowards(this.mec.position, destinations[i].position, mecSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

            // Le demi tour en incendiant l'attache
            if (attache.activeInHierarchy)
            {
                attache.SetActive(false);
                this.mec.transform.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
    }
}
