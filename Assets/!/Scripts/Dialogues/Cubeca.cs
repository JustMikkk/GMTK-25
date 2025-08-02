using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;


public class Cubeca : MonoBehaviour
{
    [SerializeField] private Transform _leftEye, _rightEye;
    [SerializeField] private Transform _mouth;
    [SerializeField] private Transform _leftElbow;
    [SerializeField] private List<Vector3> _drinkRotations;


    private void Start() {
        StartCoroutine(blink());
        // StartCoroutine(talk());
        StartCoroutine(drink());
    }

    private IEnumerator blink() {

        // _rightEye.DOKill();
        // _rightEye.DOScaleY(0, 0.4f).SetEase(Ease.InOutSine).SetDelay(0.1f).OnComplete(() => {
        //     _rightEye.DOScaleY(0.3f, 0.4f).SetEase(Ease.InOutSine).SetDelay(0.1f);
        // });

        // _leftEye.DOKill();
        // _leftEye.DOScaleY(0, 0.5f).SetEase(Ease.InOutSine).OnComplete(() => {
        //     _leftEye.DOScaleY(0.3f, 0.5f).SetEase(Ease.InOutSine);
        // });

        _leftEye.localScale = new Vector3(_leftEye.localScale.x, 0.15f, _leftEye.localScale.z);
        _rightEye.localScale = new Vector3(_rightEye.localScale.x, 0.15f, _rightEye.localScale.z);
        yield return new WaitForSeconds(0.05f);

        _leftEye.localScale = new Vector3(_leftEye.localScale.x, 0f, _leftEye.localScale.z);
        _rightEye.localScale = new Vector3(_rightEye.localScale.x, 0f, _rightEye.localScale.z);
        yield return new WaitForSeconds(0.1f);
        
        _leftEye.localScale = new Vector3(_leftEye.localScale.x, 0.15f, _leftEye.localScale.z);
        _rightEye.localScale = new Vector3(_rightEye.localScale.x, 0.15f, _rightEye.localScale.z);
        yield return new WaitForSeconds(0.05f);


        _leftEye.localScale = new Vector3(_leftEye.localScale.x, 0.3f, _leftEye.localScale.z);
        _rightEye.localScale = new Vector3(_rightEye.localScale.x, 0.3f, _rightEye.localScale.z);
        yield return new WaitForSeconds(Random.Range(3, 5));

        yield return blink();
    }


    private IEnumerator drink() {

        _leftElbow.localRotation = Quaternion.Euler(_drinkRotations[1]);
        yield return new WaitForSeconds(0.1f);

        _leftElbow.localRotation = Quaternion.Euler(_drinkRotations[2]);
        yield return new WaitForSeconds(2f);

        _leftElbow.localRotation = Quaternion.Euler(_drinkRotations[1]);
        yield return new WaitForSeconds(0.1f);

        _leftElbow.localRotation = Quaternion.Euler(_drinkRotations[0]);
        yield return new WaitForSeconds(0.1f);


        yield return new WaitForSeconds(Random.Range(7, 15));

        yield return drink();
    }



    private IEnumerator talk() {

        _mouth.DOLocalMoveX(0, 0.3f).OnComplete(() => {
            _mouth.DOScaleX(2, 0.3f);
        });


        yield return new WaitForSeconds(0.7f);

        for (int i = 0; i < 3; i++) {
            _mouth.DOScaleY(2, 0.2f).OnComplete(() => {
                _mouth.DOScaleY(0.72f, 0.1f).SetDelay(0.2f);
            });

            yield return new WaitForSeconds(1f);
        }

        _mouth.DOScaleX(0.67f, 0.3f).OnComplete(() => {
            _mouth.DOLocalMoveX(-0.2f, 0.3f);
        });



        yield return new WaitForSeconds(5);

        yield return talk();
    }

    
}
