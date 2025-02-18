using UnityEngine;
using UnityTimer;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

public class SphereGrid : MonoBehaviour
{
    public GameObject spherePrefab; // Sphere预制体
    public float space = 1.0f; // 球体之间的间隔
    public Vector2 gridSize = new Vector2(5, 5); // 网格尺寸
    public Vector2 randomRange = new Vector2(0f, 5f); // 随机位置范围

    private GameObject[,] spheres; // 存储所有Sphere实例
    private Timer positionTimer;   // 位置更新计时器
    private Timer logTimer;        // 日志输出计时器
    private GameObject selectedSphere; // 选定的目标Sphere

    private List<GameObject> nearestUsingSqr = new List<GameObject>(); // 存储平方方法的结果
    private List<GameObject> nearestUsingDistance = new List<GameObject>(); // 存储平方根方法的结果

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

        // 选择中间的Sphere作为目标
        int midX = (int)(gridSize.x / 2);
        int midZ = (int)(gridSize.y / 2);
        selectedSphere = spheres[midX, midZ];
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

        // 更新位置后计算最近邻居
        CalculateNearestSpheres();
    }

    void LogPositions()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                if (spheres[x, z] == null) continue;

                //UnityEngine.Debug.Log($"{spheres[x, z].name} - " +
                //          $"Position: {spheres[x, z].transform.position.ToString("F2")}");
            }
        }
    }

    private void CalculateNearestSpheres()
    {
        if (selectedSphere == null)
            return;

        Stopwatch stopwatch = new Stopwatch();

        // 方法b1：使用平方和比较
        stopwatch.Start();
        nearestUsingSqr = CalculateNearestUsingSqr(selectedSphere);
        stopwatch.Stop();
        long ticks1 = stopwatch.ElapsedTicks;
        double timeB1 = 0.1 * ticks1;

        stopwatch.Reset();

        //// 方法b2：使用平方根比较
        //stopwatch.Start();
        //nearestUsingDistance = CalculateNearestUsingDistance(selectedSphere);
        //stopwatch.Stop();
        //long ticks2 = stopwatch.ElapsedTicks;
        //double timeB2 = 0.1*ticks2;

        UnityEngine.Debug.Log($"平方和方法计算最近邻居耗时: {timeB1} ms");
        //UnityEngine.Debug.Log($"平方根方法计算最近邻居耗时: {timeB2} ms");
    }

    private List<GameObject> CalculateNearestUsingSqr(GameObject targetSphere)
    {
        List<(GameObject sphere, float sqrDistance)> distances = new List<(GameObject, float)>();

        Vector3 targetPos = targetSphere.transform.position;

        foreach (var sphere in spheres)
        {
            if (sphere == targetSphere)
                continue;

            Vector3 pos = sphere.transform.position;
            float dx = pos.x - targetPos.x;
            float dz = pos.z - targetPos.z;
            float sqrDist = dx * dx + dz * dz;

            distances.Add((sphere, sqrDist));
        }

        distances.Sort((a, b) => a.sqrDistance.CompareTo(b.sqrDistance));
        return distances.Take(5).Select(d => d.sphere).ToList();
    }

    private List<GameObject> CalculateNearestUsingDistance(GameObject targetSphere)
    {
        List<(GameObject sphere, float distance)> distances = new List<(GameObject, float)>();

        Vector3 targetPos = targetSphere.transform.position;

        foreach (var sphere in spheres)
        {
            if (sphere == targetSphere)
                continue;

            Vector3 pos = sphere.transform.position;
            float dx = pos.x - targetPos.x;
            float dz = pos.z - targetPos.z;
            float dist = Mathf.Sqrt(dx * dx + dz * dz);

            distances.Add((sphere, dist));
        }

        distances.Sort((a, b) => a.distance.CompareTo(b.distance));
        return distances.Take(5).Select(d => d.sphere).ToList();
    }
}