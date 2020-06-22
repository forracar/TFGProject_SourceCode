using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float movementSpeed = 10;

    // Update is called once per frame
    void Update()
    {
        transform.position += movementSpeed * (Vector3)math.forward(transform.rotation) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlanetTag")
        {
            Destroy(this);
        }
    }
}
