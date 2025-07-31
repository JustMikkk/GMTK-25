using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubesManager : MonoBehaviour
{
    private struct Move {
        public int cubeIndex;
        public List<Vector2> direction;
    }


    // input
    [SerializeField] private InputActionAsset _inputActions;
    private InputAction _moveAction;
    private InputAction _switchCubeAction;
    private InputAction _switchCameraAction;
    private InputAction _zoomInAction;
    
    
    // cameras
    [SerializeField] private CinemachineCamera _videoCamera;
    [SerializeField] private CinemachineCamera _overviewCamera;
    private bool _isVideoCamera = false;
    
    
    // cubes
    [SerializeField] private GameObject _lvl1prefab;
    [SerializeField] private List<CubesHolder> _cubesHolders;
    private List<CubeBasic> _cubes;
    private int _currentCubeIndex = 0;
    private CubeBasic _currentCube;


    private Dictionary<int, List<Vector2>> _moves = new Dictionary<int, List<Vector2>>() {
        { 0, new List<Vector2>() },
        { 1, new List<Vector2>() },
        { 2, new List<Vector2>() },
        { 3, new List<Vector2>() },
        { 4, new List<Vector2>() },
        { 5, new List<Vector2>() },
        { 6, new List<Vector2>() },
        { 7, new List<Vector2>() },
    };

    private bool _isZooming = false;
    

    private void OnEnable() {
        _inputActions.FindActionMap("Player").Enable();
    }


    private void OnDisable() {
        _inputActions.FindActionMap("Player").Disable();
    }


    private void Awake() {
        _moveAction = InputSystem.actions.FindAction("Move");
        _switchCubeAction = InputSystem.actions.FindAction("Jump");
        _switchCameraAction = InputSystem.actions.FindAction("Crouch");
        _zoomInAction = InputSystem.actions.FindAction("Interact");
    }


    private void Start() {
        _cubes = _cubesHolders.Last().cubes;

        _currentCube = _cubes[_currentCubeIndex];
        _currentCube.Select(true);
    }


    void Update() {
        if (_moveAction.IsPressed()) {
            Vector2 direction = _moveAction.ReadValue<Vector2>();
            if (_currentCube.MoveInDir(direction)) {
                Move move = new Move();
                move.cubeIndex = _currentCubeIndex;
                
                _moves[_currentCubeIndex].Add(direction);
            }
        }

        if (_switchCubeAction.WasPressedThisFrame()) {
            _isZooming = false;

            _currentCube.Select(false);
            _currentCubeIndex++;
            if (_currentCubeIndex == _cubes.Count) _currentCubeIndex = 0;

            _currentCube = _cubes[_currentCubeIndex];
            _currentCube.Select(true);
        }

        if (_switchCameraAction.WasPressedThisFrame()) {
            _isVideoCamera = !_isVideoCamera;

            if (_isVideoCamera) {
                _videoCamera.Priority = 20;
                _overviewCamera.Priority = 10;
                _currentCube.Select(false);
            } else {
                _overviewCamera.Priority = 20;
                _videoCamera.Priority = 10;
                _currentCube.Select(true);
            }
        }

        if (_zoomInAction.WasPressedThisFrame()) {
            _isZooming = true;
            StartCoroutine(ZoomIn());
        }
    }


    private IEnumerator ZoomIn()
    {
        foreach (CubesHolder ch in _cubesHolders) {
            ch.transform.DOScale(new Vector3(ch.transform.localScale.x * 3, 1, ch.transform.localScale.z * 3), 2);
        }

        yield return new WaitForSeconds(2);

        CubesHolder newCubesHolder = Instantiate(_lvl1prefab).GetComponent<CubesHolder>();
        _cubesHolders.Add(newCubesHolder);
        _cubes = newCubesHolder.cubes;

        _currentCube.Select(false);
        
        _currentCubeIndex = 0;
        _currentCube = _cubes[_currentCubeIndex];

        if (!_isVideoCamera) _currentCube.Select(true);

        CubesHolder oldCubesHolder = _cubesHolders.First();
        _cubesHolders.Remove(_cubesHolders.First());
        Destroy(oldCubesHolder.gameObject);


        for (int i = 0; i < getLongestMove(); i++) {
            for (int j = 0; j < _cubes.Count(); j++) {
                if (_moves[j].Count() > i) {
                    _cubes[j].MoveInDir(_moves[j][i]);
                }
            }

            yield return new WaitForSeconds(0.21f);
        }

        yield return new WaitForSeconds(1f);

        if (_isZooming)
            yield return ZoomIn();

    }

    private int getLongestMove() {
        int biggest = _moves[0].Count();

        for (int i = 0; i < _moves.Count(); i++) {
            if (_moves[i].Count() > biggest) {
                biggest = _moves[i].Count();
            }
        }

        return biggest;
    }
}
