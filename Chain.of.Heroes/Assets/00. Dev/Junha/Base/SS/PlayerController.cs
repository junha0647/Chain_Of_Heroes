using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject[] target;

    public bool[] isCheck;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            isCheck[0] = true;

            isCheck[1] = false;
            isCheck[2] = false;
            isCheck[3] = false;
        }
        if (isCheck[0] == true)
        {
            transform.LookAt(target[0].transform);
            transform.position = Vector3.MoveTowards(transform.position, target[0].transform.position, 0.01f);
            if(transform.position == target[0].transform.position)
            {
                isCheck[0] = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            isCheck[1] = true;

            isCheck[0] = false;
            isCheck[2] = false;
            isCheck[3] = false;
        }
        if (isCheck[1] == true)
        {
            transform.LookAt(target[1].transform);
            transform.position = Vector3.MoveTowards(transform.position, target[1].transform.position, 0.01f);
            if (transform.position == target[1].transform.position)
            {
                isCheck[1] = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            isCheck[2] = true;

            isCheck[0] = false;
            isCheck[1] = false;
            isCheck[3] = false;
        }
        if (isCheck[2] == true)
        {
            transform.LookAt(target[2].transform);
            transform.position = Vector3.MoveTowards(transform.position, target[2].transform.position, 0.01f);
            if (transform.position == target[2].transform.position)
            {
                isCheck[2] = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            isCheck[3] = true;

            isCheck[0] = false;
            isCheck[1] = false;
            isCheck[2] = false;
        }
        if (isCheck[3] == true)
        {
            transform.LookAt(target[3].transform);
            transform.position = Vector3.MoveTowards(transform.position, target[3].transform.position, 0.01f);
            if (transform.position == target[3].transform.position)
            {
                isCheck[3] = false;
            }
        }
    }

    public GameObject textObj;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            textObj.SetActive(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Portal"))
        {
            if(Input.GetKey(KeyCode.G))
            {
                SceneManager.LoadScene("BaseCamp");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            textObj.SetActive(false);
        }
    }
}