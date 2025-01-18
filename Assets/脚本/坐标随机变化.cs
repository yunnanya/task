using UnityEngine;

public class Spheregrid : MonoBehaviour
{
    public GameObject sphereprefab; // Sphere预制体
    public float space = 1.0f; // 球体之间的间隔

    private GameObject[,] spheres; // 用于存储所有Sphere实例的二维数组

    void Start()
    {
        generategrid();
        InvokeRepeating("UpdateSpheres", 0, 0.5f); // 每隔0.5秒调用一次UpdateSpheres方法
    }

    void generategrid()
    {
        spheres = new GameObject[5, 5]; // 创建一个5x5的数组来存储Sphere实例

        for (int x = 0; x < 5; x++)
        {
            for (int z = 0; z < 5; z++)
            {
                Vector3 position = new Vector3(x * space, 0, z * space);
                GameObject sphere = Instantiate(sphereprefab, position, Quaternion.identity);
                spheres[x, z] = sphere; // 将Sphere实例存储在数组中
            }
        }
    }

    void UpdateSpheres()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int z = 0; z < 5; z++)
            {
                if (spheres[x, z] != null)
                {
                    // 更新Sphere的名称
                    spheres[x, z].name = "Sphere_" + Random.Range(0, 1000);

                    // 更新Sphere的位置
                    Vector3 newPosition = new Vector3(
                        Random.Range(0, 5) * space,
                        0,
                        Random.Range(0, 5) * space
                    );
                    spheres[x, z].transform.position = newPosition;

                    // 输出Sphere的名称和位置
                    Debug.Log("Sphere Name: " + spheres[x, z].name + ", Position: " + newPosition);
                }
            }
        }
    }
}
