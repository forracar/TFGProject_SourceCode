using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundPoint : MonoBehaviour
{
    [Range(0.01f,0.08f)]public float movementSpeed = 0.04f;
    public int ID;

    // Update is called once per frame
    void Update()
    {
        // Update planet position
        transform.position = OOPDataManager.RotateAroundPoint(transform.position, Vector3.zero, Vector3.up, movementSpeed * Time.deltaTime); ;

        // Update planet position reference
        OOPDataManager.UpdatePlanetPosition(ID, transform.position);
    }
}
