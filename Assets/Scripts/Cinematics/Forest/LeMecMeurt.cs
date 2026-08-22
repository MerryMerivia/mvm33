using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeMecMeurt : CinematicEvent
{
    [SerializeField] private SpriteRenderer beeeeeeete;
    [SerializeField] private SpriteRenderer mec;
    [SerializeField] private Sprite mecSansTorche;
    [SerializeField] private GameObject bloc;
    [SerializeField] private float vitesseAlpha;
    [SerializeField] private float tempsAvantYeuxRouges;
    [SerializeField] private float tempsAvantTuage;

    public override void Event()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        float temps = 0;

        // Apparition de la Bêêêêêêêêête
        while (this.beeeeeeete.color.a < 1)
        {
            float currentValue = this.beeeeeeete.color.a;
            currentValue += Time.deltaTime * this.vitesseAlpha;
            Color currentColor = this.beeeeeeete.color;
            this.beeeeeeete.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentValue);

            yield return new WaitForSeconds(0.01f);
        }

        // Yeux rouges
        while (temps < this.tempsAvantYeuxRouges)
        {
            temps += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        this.beeeeeeete.color = Color.red;
        temps = 0;

        // Le mec va mourir !
        while (temps < this.tempsAvantYeuxRouges)
        {
            temps += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        temps = 0;

        // Le mec meurt
        this.mec.sprite = this.mecSansTorche;
        this.mec.transform.Translate(0, -0.5f, 0);
        this.mec.transform.Rotate(0, 0, 90);
        this.bloc.SetActive(false);

        // Disparition de la Bêêêêêêêêête
        while (this.beeeeeeete.color.a > 0)
        {
            float currentValue = this.beeeeeeete.color.a;
            currentValue -= Time.deltaTime * this.vitesseAlpha;
            Color currentColor = this.beeeeeeete.color;
            this.beeeeeeete.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentValue);

            yield return new WaitForSeconds(0.01f);
        }
    }
}
