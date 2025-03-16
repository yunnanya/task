using UnityEngine;
using System.Collections.Generic;
using PathFinding;
using BezierUtils;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    [Header("目标")]
    public Transform target;
    public TileGrid tileGrid;

    [Header("移动参数")]
    public float moveSpeed = 5.0f;
    private List<Vector3> _currentPath;

    [Header("路径渲染")]
    public LineRenderer pathLineRenderer;

    private Coroutine _pathUpdateCoroutine;

    private void Start()
    {
        _pathUpdateCoroutine = StartCoroutine(UpdatePathRoutine());
    }

    private IEnumerator UpdatePathRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // 缩短更新间隔至0.1秒
            if (target == null || tileGrid == null) continue;

            // 检查目标点是否可行走
            Tile targetTile = tileGrid.GetTileFromWorldPos(target.position);
            if (targetTile == null || targetTile.Weight >= TileGrid.TileWeight_Wall)
            {
                Debug.LogWarning("目标点位于障碍物上，无法移动");
                _currentPath = null;
                continue;
            }

            List<Vector3> path = PathFinder.GetPathPoints(tileGrid, transform.position, target.position);
            if (path != null && path.Count > 0)
            {
                _currentPath = SmoothAndValidatePath(path);
            }
            else
            {
                _currentPath = null;
            }
        }
    }

    private List<Vector3> SmoothAndValidatePath(List<Vector3> rawPath)
    {
        List<Vector3> smoothedPath = SmoothPathWithBezier(rawPath);
        if (IsPathValid(smoothedPath)) return smoothedPath;

        // 如果平滑后的路径无效，回退到原始路径
        Debug.LogWarning("平滑路径包含障碍物，使用原始路径");
        return rawPath;
    }

    private bool IsPathValid(List<Vector3> path)
    {
        foreach (Vector3 point in path)
        {
            Tile tile = tileGrid.GetTileFromWorldPos(point);
            if (tile != null && tile.Weight >= TileGrid.TileWeight_Wall) return false;
        }
        return true;
    }

    private void Update()
    {
        // 更新路径渲染
        if (_currentPath != null && _currentPath.Count > 0)
        {
            pathLineRenderer.positionCount = _currentPath.Count;
            pathLineRenderer.SetPositions(_currentPath.ToArray());
        }
        else
        {
            pathLineRenderer.positionCount = 0;
        }

        // 移动逻辑
        if (_currentPath == null || _currentPath.Count == 0) return;

        Vector3 targetPoint = _currentPath[0];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            _currentPath.RemoveAt(0);
        }
    }

    private List<Vector3> SmoothPathWithBezier(List<Vector3> path)
    {
        if (path == null || path.Count < 2) return path;
        BezierManager.Instance.SetBezierSourcePoints(path, 15, 0.5f);
        return BezierManager.Instance.GenerateBezierCurvePoints() ?? path;
    }

    private void OnDestroy()
    {
        if (_pathUpdateCoroutine != null) StopCoroutine(_pathUpdateCoroutine);
    }
}