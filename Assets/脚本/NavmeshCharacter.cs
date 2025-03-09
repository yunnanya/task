using UnityEngine;
using UnityEngine.AI;

public class NavmeshCharacter : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Camera _camera;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ������
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _agent.SetDestination(hit.point); // ����Ŀ���
            }
        }
    }
}