using System.Collections;
using UnityEngine;
using DG.Tweening;


public class Cubeca : MonoBehaviour
{
    [SerializeField] private Transform _leftEye, _rightEye;
    [SerializeField] private Transform _mouth;


    private void Start() {
        StartCoroutine(blink());
        StartCoroutine(talk());
    }

    private IEnumerator blink() {

        _rightEye.DOKill();
        _rightEye.DOScaleY(0, 0.4f).SetEase(Ease.InOutSine).SetDelay(0.1f).OnComplete(() => {
            _rightEye.DOScaleY(0.3f, 0.4f).SetEase(Ease.InOutSine).SetDelay(0.1f);
        });

        _leftEye.DOKill();
        _leftEye.DOScaleY(0, 0.5f).SetEase(Ease.InOutSine).OnComplete(() => {
            _leftEye.DOScaleY(0.3f, 0.5f).SetEase(Ease.InOutSine);
        });

        yield return new WaitForSeconds(5);

        yield return blink();
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
