using UnityEngine;

public class NightManager : MonoBehaviour
{
    [SerializeField] private Material nightMaterial;
    [SerializeField] private Color color;
    [SerializeField] private float timeBeforeNight;

    [SerializeField] private SpriteRenderer nightMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.nightMaterial.color = this.color;
        this.color.a = -1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (this.color.a < 1f)
        {
            this.color.a += 2 * (Time.deltaTime / this.timeBeforeNight);
            //this.nightMaterial.color = this.color;
            //this.nightMaterial.SetColor("_NightColor", this.color);
            //this.nightMaterial.SetFloat("_Transparency", Mathf.Max(this.color.a, 0f));

            this.nightMask.color = this.color;
        }

    }
}
