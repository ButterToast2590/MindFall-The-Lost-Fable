using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EssentialObjectsLoaderSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();

        if (existingObjects.Length == 0)
        {
            Instantiate(essentialObjectsPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex == 1) 
            {
                var portal = FindObjectOfType<Portal>();
                if (portal != null)
                {
                    portal.gameObject.SetActive(true);
                }
            }
        }
    }
}
