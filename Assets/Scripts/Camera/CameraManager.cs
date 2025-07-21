using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Character _playerToFollow;
    [SerializeField] private Vector3 _offsetToCharacter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _camera.transform.position = _playerToFollow.transform.position + _offsetToCharacter;
    }
}
