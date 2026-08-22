using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAura : MonoBehaviour
{
    [SerializeField] private float changeAlphaSpeed;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float currentValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.currentValue += Time.deltaTime * this.changeAlphaSpeed;
        Color currentColor = this.spriteRenderer.color;
        this.spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Abs(Mathf.Sin(currentValue))-0.75f);
    }
}
