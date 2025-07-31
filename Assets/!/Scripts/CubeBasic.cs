using UnityEngine;
using DG.Tweening;

public class CubeBasic : MonoBehaviour
{
    [SerializeField] private Transform _rotatableMesh;
    [SerializeField] private GameObject _arrowGO;

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

        _rotatableMesh.DORotate(new Vector3(dir.y * -90, 0, dir.x * 90), 0.2f, RotateMode.Fast);

        _transform.DOMove(new Vector3(_transform.position.x - dir.x, _transform.position.y, _transform.position.z - dir.y), 0.2f).OnComplete(() => {
            _transform.position = Vector3Int.RoundToInt(_transform.position);
            _isMoving = false;
            _rigidBody.useGravity = true;
            _rotatableMesh.rotation = Quaternion.identity;
        });
        return true;
    }

    public void Select(bool doSelect) {
        _arrowGO.SetActive(doSelect);

    }

    public void MakeKinematic() {
        _rigidBody.useGravity = false;
        _rigidBody.isKinematic = true;
    }

}
