using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public Vector3 startPosition;
    
    public void OnGameStart()
    {
        transform.position = startPosition;
    }
}
