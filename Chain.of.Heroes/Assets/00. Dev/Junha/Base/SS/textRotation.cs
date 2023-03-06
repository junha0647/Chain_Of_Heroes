using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textRotation : MonoBehaviour
{
    private void Update()
    {
        this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}