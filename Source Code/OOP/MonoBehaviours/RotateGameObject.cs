using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
    [Range(1f,5f)] public float rotationSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
    }
}
