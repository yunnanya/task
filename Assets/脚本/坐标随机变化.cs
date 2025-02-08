using UnityEngine;
using UnityTimer;

public class SphereGrid : MonoBehaviour
{
    public GameObject spherePrefab; // Sphere预制体
    public float space = 1.0f; // 球体之间的间隔
    public Vector2 gridSize = new Vector2(5, 5); // 网格尺寸
    public Vector2 randomRange = new Vector2(0f, 5f); // 随机位置范围

    private GameObject[,] spheres; // 存储所有Sphere实例
    private Timer positionTimer;   // 位置更新计时器
    private Timer logTimer;        // 日志输出计时器

    void Awake()
    {
        GenerateGrid();

        // 注册位置更新计时器（每秒执行）
        positionTimer = Timer.Register(1f, UpdatePositions,
            isLooped: true);

        // 注册日志输出计时器（每0.5秒执行）
        logTimer = Timer.Register(0.5f, LogPositions,
            isLooped: true);
    }

    void OnDestroy()
    {
        Timer.Cancel(positionTimer);
        Timer.Cancel(logTimer);
    }

    void GenerateGrid()
    {
        int width = (int)gridSize.x;
        int height = (int)gridSize.y;
        spheres = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * space, 0, z * space);
                GameObject sphere = Instantiate(spherePrefab, position, Quaternion.identity);
                sphere.name = $"Sphere_{x}_{z}"; // 设置唯一名称
                sphere.transform.SetParent(transform); // 设为子物体方便管理
                spheres[x, z] = sphere;
            }
        }
    }

    void UpdatePositions()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                if (spheres[x, z] == null) continue;

                // 生成连续随机位置
                float newX = Random.Range(randomRange.x, randomRange.y) * space;
                float newZ = Random.Range(randomRange.x, randomRange.y) * space;
                spheres[x, z].transform.position = new Vector3(newX, 0, newZ);
            }
        }
    }

    void LogPositions()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                if (spheres[x, z] == null) continue;

                Debug.Log($"{spheres[x, z].name} - " +
                          $"Position: {spheres[x, z].transform.position.ToString("F2")}");
            }
        }
    }
}