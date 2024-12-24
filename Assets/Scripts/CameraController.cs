using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float Speed = 5;
    private Transform _transform;
    public void Start()
    {
        _transform = Camera.main.transform;
    }
    private void FixedUpdate()
    {
        Vector3 target = new Vector3(transform.position.x, transform.position.y, -10);
        _transform.position = Vector3.Lerp(_transform.position, target, Speed * Time.deltaTime);
    }

}
