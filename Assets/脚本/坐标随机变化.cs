using UnityEngine;
using UnityTimer;

/*
 * 脚本命名注意：
 * 1.采用驼峰命名法，如：ExampleClass
 * 2.脚本名称与类名一致，如：ExampleClass.cs
 */

public class SphereGrid : MonoBehaviour
{
    public GameObject spherePrefab;
    public float space = 1.0f;
    public Vector2 gridSize = new Vector2(5, 5);
    public Vector2 randomRange = new Vector2(0f, 5f);

    private GameObject[,] spheres;
    private Timer positionTimer;
    private Timer logTimer;

    void Awake()
    {
        GenerateGrid();

        /*
         * 两处遍历的时间间隔，应提前声明好
         * Timer注册完成时，应添加日志作为标记
         */
        positionTimer = Timer.Register(1f, UpdatePositions, isLooped: true);
        logTimer = Timer.Register(0.5f, LogPositions, isLooped: true);

        /*
         * 在Awake或其他初始化逻辑中，如果资源文件是空的，应添加错误日志
         */
        if (spherePrefab == null)
        {
            Debug.LogError("spherePrefab Is Null.");
        }
    }

    void OnDestroy()
    {
        Timer.Cancel(positionTimer);
        Timer.Cancel(logTimer);

        /*
         * 在OnDestroy中，非必要的数据容器全部清空
         * 非必要的变量也可以置为空（=null）
         */
    }

    private void GenerateGrid()
    {
        /*
         * 每次执行GenerateGrid()时，都会重新声明两个int变量及一个二维数组
         * 这样做耗费性能，且会引入内存泄漏风险
         * 应把高频用到的变量提前声明好
         */

        int width = (int)gridSize.x;
        int height = (int)gridSize.y;
        spheres = new GameObject[width, height];

        /*
         * 遍历通常使用i,j,k作为下标
         * 或PosX,PosZ等命名
         */
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * space, 0, z * space);
                GameObject sphere = Instantiate(spherePrefab, position, Quaternion.identity);
                sphere.name = $"Sphere_{x}_{z}";
                sphere.transform.SetParent(transform);
                spheres[x, z] = sphere;
            }
        }
    }

    private void UpdatePositions()
    {
        /*
        * 遍历通常使用i,j,k作为下标
        * 或PosX,PosZ等命名
        */
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                if (spheres[x, z] == null) continue;

                float newX = Random.Range(randomRange.x, randomRange.y) * space;
                float newZ = Random.Range(randomRange.x, randomRange.y) * space;
                spheres[x, z].transform.position = new Vector3(newX, 0, newZ);
            }
        }
    }

    private void LogPositions()
    {
        /*
        * 遍历通常使用i,j,k作为下标
        * 或PosX,PosZ等命名
        */
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                if (spheres[x, z] == null) continue;
                //大量、高频拼接字符串非常耗费性能
                //后续文本Log输出应尽量谨慎
                Debug.Log($"{spheres[x, z].name} Position: {spheres[x, z].transform.position.ToString("F2")}");
            }
        }
    }
}
