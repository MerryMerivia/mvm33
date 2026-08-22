using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Peut être utilisé par des CameraDestination pour produire quelque chose quand la caméra a atteint la destination en question
 */
public abstract class CinematicEvent : MonoBehaviour    // On doit garder le MonoBehaviour pour permettre les Coroutines
{
    public abstract void Event();
}
