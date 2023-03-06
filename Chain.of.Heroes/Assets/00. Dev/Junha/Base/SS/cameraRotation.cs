using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRotation : MonoBehaviour
{
    private void Update()
    {
        this.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
    }
}
