using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Transition : MonoBehaviour
{
    [SerializeField] GameObject currentRoom;
    [SerializeField] Transition leadingRoom;
    [SerializeField] PolygonCollider2D cameraConfiner;
    private List<Transition> adjacentRooms;
    private CinemachineConfiner2D cinemachineConfiner;

    private void Start()
    {
        this.adjacentRooms = new List<Transition>();
        this.cinemachineConfiner = FindAnyObjectByType<CinemachineConfiner2D>();

        Transform transitionsTransform = this.transform.parent;
        foreach (Transition transition in transitionsTransform.GetComponentsInChildren<Transition>())
        {
            this.adjacentRooms.Add(transition.leadingRoom);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().ChangeRoom(this.transform.localEulerAngles.z);
            this.Exit();
        }
    }

    public void Exit()
    {
        foreach (Transition transition in this.adjacentRooms)   // On désactive toutes les salles jointes à celle qu'on quitte, mais on réactive celles nécessaires dans Enter() de la salle dans laquelle on entre
        {
            transition.currentRoom.SetActive(false);
        }
        this.leadingRoom.Enter();
    }

    public void Enter()
    {
        this.currentRoom.SetActive(true);   // Normalement pas besoin, mais dans le doute...
        this.cinemachineConfiner.m_BoundingShape2D = this.cameraConfiner;

        foreach (Transition transition in this.adjacentRooms)   // On désactive toutes les salles jointes à celle qu'on quitte, mais on réactive celles nécessaires dans Enter() de la salle dans laquelle on entre
        {
            transition.currentRoom.SetActive(true);
        }
        this.SetRoom();
    }

    /**
     * Initialiser transform et état des ennemis ici
     */
    private void SetRoom()
    {

    }
}
