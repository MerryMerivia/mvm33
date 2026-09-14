using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCameraFocus : MonoBehaviour
{
    [Serializable]
    public struct CameraDestination
    {
        public Transform destination;
        public float duration;
        public float speed;
        public CinematicEvent cinematicEvent;

        public CameraDestination(Transform destination, float duration, float speed, CinematicEvent cinematicEvent)
        {
            this.destination = destination;
            this.duration = duration;
            this.speed = speed;
            this.cinematicEvent = cinematicEvent;
        }
    }

    [SerializeField] private Transform cameraFocus;
    public CameraDestination[] cameraDestinations;
    [SerializeField] private PlayerController playerController;

    private float timeSinceArrivedAtDestination = 0f;

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
        if (collision.CompareTag("Player") && !this.active)
        {
            StartCoroutine(ProgressTowardDestination());
            this.active = true;
        }
    }

    private IEnumerator ProgressTowardDestination()
    {
        this.playerController.SetCanMove(false);
        for (int i = 0 ; i < cameraDestinations.Length ; i++)
        {
            // Déplacement de la caméra
            while (this.cameraFocus.position != this.cameraDestinations[i].destination.position)
            {
                this.cameraFocus.position = Vector3.MoveTowards(this.cameraFocus.position, cameraDestinations[i].destination.position, cameraDestinations[i].speed * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }

            if (this.cameraDestinations[i].cinematicEvent)
            {
                this.cameraDestinations[i].cinematicEvent.Event();  // Permet qu'il se passe des choses pendant l'arrêt de la caméra à destination
            }

            // Attente
            while (this.timeSinceArrivedAtDestination < this.cameraDestinations[i].duration)
            {
                this.timeSinceArrivedAtDestination += Time.deltaTime;
                yield return new WaitForSeconds(0.01f);
            }
        }
        this.playerController.SetCanMove(true);
        //GameObject.Destroy(this.gameObject);
    }
}
