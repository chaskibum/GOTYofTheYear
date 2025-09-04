using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private Transform target;
    private float _yOffset = 1f;

    private void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y + _yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
    }
}
