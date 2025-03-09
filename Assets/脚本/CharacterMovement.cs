using PathFinding; // ȷ������ PathFinding �����ռ�
using UnityEngine;
using System.Collections.Generic;

public class CharacterMovement : MonoBehaviour
{
    public float Speed = 5.0f;
    private List<Vector3> _path;
    private int _currentIndex;

    public void SetPath(List<Tile> path)
    {
        _path = new List<Vector3>();
        foreach (var tile in path)
        {
            _path.Add(tile.ToWorldPosition()); // �� Tile ����ת��Ϊ��������
        }
        _currentIndex = 0;
    }

    private void Update()
    {
        if (_path == null || _currentIndex >= _path.Count) return;

        Vector3 target = _path[_currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, Speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            _currentIndex++;
        }
    }
}