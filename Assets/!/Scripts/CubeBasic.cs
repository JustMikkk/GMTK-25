using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

public class CubeBasic : MonoBehaviour
{
    public UnityEvent sequenceCompleted = new();

    [SerializeField] private Transform _rotatableMesh;
    [SerializeField] private GameObject _arrowGO;

    [SerializeField] private Vector3 _moveDir;

    [SerializeField] private Vector3 _raycastOrigin = new Vector3(0, -0.4f, 0);
    [SerializeField] private float _raycastLenght = 0.1f;

    private List<Vector2> _movesSequence;

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
        return moveInDir(dir, 0.2f);
    }


    public void Select(bool doSelect) {
        _arrowGO.SetActive(doSelect);
    }


    public void MakeKinematic(bool isKinematic) {
        _rigidBody.useGravity = !isKinematic;
        _rigidBody.isKinematic = isKinematic;
    }


    public void MoveFromSequence(List<Vector2> moves) {
        _movesSequence = moves;
        StartCoroutine(moveFromSequence());
    }


    private bool moveInDir(Vector2 dir, float speed) {
        if (_isMoving) return false;
        if (!isGrounded()) return false;

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

        _rotatableMesh.DORotate(rotationAxis, speed, RotateMode.WorldAxisAdd);
        _transform.DOMove(new Vector3(_transform.position.x - dir.x, _transform.position.y, _transform.position.z - dir.y), speed).OnComplete(() => {
            _transform.position = Vector3Int.RoundToInt(_transform.position);
            _isMoving = false;
            _rigidBody.useGravity = true;
        });

        return true;
    }


    private IEnumerator moveFromSequence() {
        foreach (Vector2 move in _movesSequence) {
            bool moved = false;
            float timer = 0;
         
            while (!moved && timer < 3) {  // Changed || to && and > to <
                moved = moveInDir(move, 0.1f);
                timer += Time.deltaTime;
                yield return new WaitForSeconds(0.11f);
            }
        }

        sequenceCompleted?.Invoke();
        yield return null;
    }


    private bool isGrounded() {

        Ray downRay = new Ray(transform.position + _raycastOrigin, Vector3.down);

        if (Physics.Raycast(downRay, out RaycastHit hit, _raycastLenght)) {
            Debug.DrawRay(downRay.origin, downRay.direction * _raycastLenght, Color.green);
            return true;
        }

        Debug.DrawRay(downRay.origin, downRay.direction * _raycastLenght, Color.red);
        return false;
    }

}
