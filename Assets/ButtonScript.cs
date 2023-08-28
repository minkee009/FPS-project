using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class ButtonScript : MonoBehaviour
{
    public GameObject bridge;
    public NavMeshSurface navMeshSurface;

    // Start is called before the first frame update
    void Start()
    {
        bridge.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bridge.SetActive(true);
            navMeshSurface.BuildNavMesh();
        }
    }
}
