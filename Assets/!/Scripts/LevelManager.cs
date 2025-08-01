using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public UnityEvent levelReadyEvent = new();

    // input
    [SerializeField] private InputActionAsset _inputActions;
    private InputActionMap _inputActionMap;
    private InputAction _moveAction;
    private InputAction _switchCameraAction;
    private InputAction _nextCubeAction;
    private InputAction _prevCubeAction;
    private InputAction _resetAction;
    private InputAction _undoAction;
    
    
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _videoCamera;
    [SerializeField] private CinemachineCamera _overviewCamera;


    [Header("Zoom")]
    [SerializeField] private float _zoomAmount;
    [SerializeField] private List<float> _nextYPositions;
    private bool _isVideoCamera = false;
    
    
    [Header("Cubes")]
    [SerializeField] private Transform _gameHolder;
    [SerializeField] private GameObject _newPrefab;
    [SerializeField] private List<CubesHolder> _cubesHolders;
    private List<CubeBasic> _cubes;
    private int _currentCubeIndex = 0;
    private CubeBasic _currentCube;


    // reset timer
    private float _resetHoldTime = 2;
    private float _resetTimer = 0;



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

#region Lifetime Methods

    private void Awake() {
        _inputActionMap = _inputActions.FindActionMap("Player");

        _moveAction = InputSystem.actions.FindAction("Move");
        _switchCameraAction = InputSystem.actions.FindAction("SwitchCamera");
        _nextCubeAction = InputSystem.actions.FindAction("NextCube");
        _prevCubeAction = InputSystem.actions.FindAction("PreviousCube");
        _undoAction = InputSystem.actions.FindAction("Undo");
        _resetAction = InputSystem.actions.FindAction("Reset");

        _gameHolder.gameObject.SetActive(false);
    }


    private void Start() {
        _cubes = _cubesHolders.Last().cubes;

        _currentCube = _cubes[_currentCubeIndex];
        _currentCube.Select(true);

    }


    void Update() {

        if (!_inputActionMap.enabled) return;

        if (_moveAction.IsPressed()) {
            Vector2 direction = _moveAction.ReadValue<Vector2>();
            if (direction.x != 0 && direction.y != 0) {
                direction = direction.x > direction.y ? new Vector2(direction.x, 0) : new Vector2(0, direction.y);
            }
            if (_currentCube.MoveInDir(direction.normalized)) {                
                _moves[_currentCubeIndex].Add(direction.normalized);
            }
        }

        if (_switchCameraAction.WasPressedThisFrame()) {
            SwitchCamera(!_isVideoCamera);
        }

        if (_nextCubeAction.WasPressedThisFrame()) {
            SwitchCube(true);
        }

        if (_prevCubeAction.WasPressedThisFrame()) {
            SwitchCube(false);
        }

        if (_resetAction.IsPressed()) {
            GameManager.instance.SetTransitionCamera(true);
            _resetTimer += Time.deltaTime;

            if (_resetTimer >= _resetHoldTime) {
                _resetTimer = 0;
                GameManager.instance.ResetLevel();
            }

        } else {
            if (_resetTimer > 0) {
                GameManager.instance.SetTransitionCamera(false);
                _resetTimer -= Time.deltaTime;
            }
        }

        if (_undoAction.WasPressedThisFrame()) {

        }
    }

#endregion
#region Starting / Ending

    public void AppearNormal(float delay) {
        _gameHolder.gameObject.SetActive(true);
        _gameHolder.localScale = Vector3.one * 0.01f;
        _gameHolder.rotation = Quaternion.Euler(new Vector3(0, -180, 0));

        _inputActionMap.Enable();
        _gameHolder.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        _gameHolder.DOScale(Vector3.one, 2).SetEase(Ease.InOutSine).SetDelay(delay);
        _gameHolder.DORotate(Vector3.zero, 2, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).SetDelay(delay).OnComplete(() => {
            levelReadyEvent?.Invoke();
            Invoke(nameof(makeCubesNotKinematic), 1);
        });
    }


    public void AppearReset(float delay) {
        _gameHolder.gameObject.SetActive(true);
        _gameHolder.position = new Vector3(0, -50, 0);

        _inputActionMap.Enable();
        _gameHolder.DOMoveY(0, 1f).SetEase(Ease.InOutSine).SetDelay(delay).OnComplete(() => {
            levelReadyEvent?.Invoke();
            Invoke(nameof(makeCubesNotKinematic), 1);
        });
    }


    public void DisappearNormal() {
        _inputActionMap.Disable();
        _isZooming = false;
        StopCoroutine(zoomIn());

        _gameHolder.DOScale(Vector3.one * 0.01f, 1).SetEase(Ease.InOutSine);
        _gameHolder.DORotate(new Vector3(0, -180, 0), 1, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).OnComplete(() => {
            Destroy(gameObject);
        });
    }

    public void DisappearReset() {
        _inputActionMap.Disable();
        _isZooming = false;
        StopCoroutine(zoomIn());

        _gameHolder.DOMoveY(-50, 1f).SetEase(Ease.InOutSine).OnComplete(() => {
            Destroy(gameObject);
        });
    }


#endregion
#region Switching


    public void SwitchCamera(bool isVideo) {
        _isVideoCamera = isVideo;

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


    public void SwitchCube(bool isNext) {
        _currentCube.Select(false);

        if (isNext) {
            _currentCubeIndex++;
            if (_currentCubeIndex == _cubes.Count) _currentCubeIndex = 0;
        } else {
            _currentCubeIndex--;
            if (_currentCubeIndex == -1) _currentCubeIndex = _cubes.Count() -1;
        }

        _currentCube = _cubes[_currentCubeIndex];
        _currentCube.Select(true);
    }


#endregion
#region Zooming

    public void Zoom(bool doZoomIn) {
        _isZooming = doZoomIn;

        if (_isZooming) StartCoroutine(zoomIn());
    }


    private IEnumerator zoomIn() {
        makeCubesKinematic();

        int index = 0;
        foreach (CubesHolder ch in _cubesHolders) {
            ch.ResetCubesPositions();
            ch.transform.DOMoveY(_nextYPositions[index], 2);
            ch.transform.DOScale(ch.transform.localScale * _zoomAmount, 2).OnComplete(() => {
                ch.ResetCubesPositions();
            });
            index++;
        }

        yield return new WaitForSeconds(2f);

        CubesHolder newCubesHolder = Instantiate(_newPrefab, _gameHolder).GetComponent<CubesHolder>();
        _cubesHolders.Add(newCubesHolder);
        _cubes = newCubesHolder.cubes;
        makeCubesNotKinematic();

        _currentCube.Select(false);
        
        _currentCubeIndex = 0;
        _currentCube = _cubes[_currentCubeIndex];

        if (!_isVideoCamera) _currentCube.Select(true);

        CubesHolder oldCubesHolder = _cubesHolders.First();
        _cubesHolders.Remove(_cubesHolders.First());
        Destroy(oldCubesHolder.gameObject);

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < getLongestMove(); i++) {
            for (int j = 0; j < _cubes.Count(); j++) {
                if (_moves[j].Count() > i) {
                    _cubes[j].MoveInDir(_moves[j][i]);
                }
            }

            yield return new WaitForSeconds(0.21f);
        }

        yield return new WaitForSeconds(2f);

        if (_isZooming)
            yield return zoomIn();
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

#endregion
#region Help Methods

    private void makeCubesKinematic() {
        foreach (CubeBasic cube in _cubes) {
            cube.MakeKinematic(true);
        }
    }

    private void makeCubesNotKinematic() {
        foreach (CubeBasic cube in _cubes) {
            cube.MakeKinematic(false);
        }
    }

#endregion

}
