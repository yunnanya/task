using UnityEngine;

public class PrefabReplacer : MonoBehaviour
{
    public GameObject characterPrefab;

    void Start()
    {
        GameObject sphere = GameObject.Find("Sphere");
        if (sphere != null)
        {
            Vector3 pos = sphere.transform.position;
            Quaternion rot = sphere.transform.rotation;
            Destroy(sphere);
            Instantiate(characterPrefab, pos, rot);
        }
    }
}