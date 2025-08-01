using UnityEngine;
using DG.Tweening;

public class CubeBasic : MonoBehaviour
{
    [SerializeField] private Transform _rotatableMesh;
    [SerializeField] private GameObject _arrowGO;

    [SerializeField] private Vector3 _moveDir;

    private bool _isMoving = false;

    private Rigidbody _rigidBody;
    private Transform _transform;
    private AudioSource _audioSource;


    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _audioSource = GetComponent<AudioSource>();
    }
    

    public bool MoveInDir(Vector2 dir) {
        if (_isMoving) return false;

        _audioSource.DOKill();
        _audioSource.pitch = Random.Range(0.95f, 1.05f);
        _audioSource.Play();
        _audioSource.DOPlay();

        _isMoving = true;
        _rigidBody.useGravity = false;

        Vector3 rotationAxis = Vector3.zero;
        if (dir.x != 0) {
            rotationAxis = new Vector3(0, 0, dir.x * 90);
        } else if (dir.y != 0) {
            rotationAxis = new Vector3(-dir.y * 90, 0, 0);
        }

        _rotatableMesh.DORotate(rotationAxis, 0.2f, RotateMode.WorldAxisAdd);
        _transform.DOMove(new Vector3(_transform.position.x - dir.x, _transform.position.y, _transform.position.z - dir.y), 0.2f).OnComplete(() => {
            _transform.position = Vector3Int.RoundToInt(_transform.position);
            _isMoving = false;
            _rigidBody.useGravity = true;
        });

        return true;
    }

    public void Select(bool doSelect) {
        _arrowGO.SetActive(doSelect);

    }

    public void MakeKinematic(bool isKinematic) {
        _rigidBody.useGravity = !isKinematic;
        _rigidBody.isKinematic = isKinematic;
    }

}
