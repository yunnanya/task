using UnityEngine;

public class SphereGrid : MonoBehaviour
{
    public GameObject spherePrefab; // 从Project面板拖拽Sphere预制体到这里
    public float spacing = 1.0f; // 球体之间的间隔

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int z = 0; z < 5; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                Instantiate(spherePrefab, position, Quaternion.identity);
            }
        }
    }
}
