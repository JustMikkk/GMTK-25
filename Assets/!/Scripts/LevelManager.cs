using System.Collections;
using System.Collections.Generic;
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


    public struct CubeMove {
        public int cubeIndex;
        public Vector2 direction; 
    }


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
    [SerializeField] private List<Vector3> _nextPositions;
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



    private Dictionary<int, List<Vector2>> movesDict = new Dictionary<int, List<Vector2>>() {
        { 0, new List<Vector2>() },
        { 1, new List<Vector2>() },
        { 2, new List<Vector2>() },
        { 3, new List<Vector2>() },
        { 4, new List<Vector2>() },
        { 5, new List<Vector2>() },
        { 6, new List<Vector2>() },
        { 7, new List<Vector2>() },
    };

    private List<CubeMove> _moves = new();
    
    
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
                direction = direction.x > direction.y ? new Vector2(direction.x, 0).normalized : new Vector2(0, direction.y).normalized;
            }
            if (_currentCube.MoveInDir(direction)) {                
                movesDict[_currentCubeIndex].Add(direction);
                CubeMove move = new() {
                    cubeIndex = _currentCubeIndex,
                    direction = direction
                };
                _moves.Add(move);
                GameManager.instance.UpdateMovesText(_moves.Count());
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
            UndoMove();
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
            ch.transform.DOMove(_nextPositions[index], 2);
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

        // foreach (CubeMove move in _moves) {
        //     _cubes[move.cubeIndex].MoveInDir(move.direction);
        //     yield return new WaitForSeconds(0.21f);
        // }

    // moving with all at once
        // for (int i = 0; i < getLongestMove(); i++) {
        //     for (int j = 0; j < _cubes.Count(); j++) {
        //         if (movesDict[j].Count() > i) {
        //             _cubes[j].MoveInDir(movesDict[j][i]);
        //         }
        //     }

        //     yield return new WaitForSeconds(0.21f);
        // }
        int cubesReadyCounter = 0;
        for (int i = 0; i < _cubes.Count(); i++) {
            _cubes[i].sequenceCompleted.AddListener(() => {
                cubesReadyCounter++;
            });
            _cubes[i].MoveFromSequence(movesDict[i], 0.1f * i);
        }


        yield return new WaitUntil(() => cubesReadyCounter == _cubes.Count());
        yield return new WaitForSeconds(2f);

        if (_isZooming)
            yield return zoomIn();
    }


    private int getLongestMove() {
        int biggest = movesDict[0].Count();

        for (int i = 0; i < _moves.Count(); i++) {
            if (movesDict[i].Count() > biggest) {
                biggest = movesDict[i].Count();
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


    public void UndoMove() {
        if (_moves.Count() == 0) {
            foreach (CubeBasic cube in _cubes) {
                cube.transform.position += Vector3.up;
                return;
            }
        }
        CubeMove lastMove = _moves.Last();
        if (Mathf.RoundToInt(_cubes[lastMove.cubeIndex].gameObject.transform.position.y) == 0) {
            _cubes[lastMove.cubeIndex].gameObject.transform.position += Vector3.up; 
        }
        if (_cubes[lastMove.cubeIndex].MoveInDir(lastMove.direction * -1)) {
            _moves.Remove(lastMove);
            GameManager.instance.UpdateMovesText(_moves.Count());
        }
    }

#endregion

}
