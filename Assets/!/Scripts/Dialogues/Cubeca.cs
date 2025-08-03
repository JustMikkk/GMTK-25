using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using DG.Tweening;


public class Cubeca : MonoBehaviour
{
    [Serializable]
    public struct DialogueSequence {
        [Multiline]
        public List<string> dialogues;
        public int fontSize;
    }

    [SerializeField] private List<DialogueSequence> _sequences;
    [SerializeField] private float _talkSpeed = 0.2f;

    [SerializeField] private Animator _animator;
    [SerializeField] private RectTransform _bubbleRect;
    [SerializeField] private TextMeshProUGUI _bubbleTextFront;
    [SerializeField] private TextMeshProUGUI _bubbleTextBack;

    [SerializeField] private RectTransform _bgRect;


    private int _goNextHash = Animator.StringToHash("goNext");
    private int _animationIndexHash = Animator.StringToHash("animationIndex");
    private int _playAnimationHash = Animator.StringToHash("playAnimation");



    private bool _isTalking = false;
    private string _goalString;
    private int _dialogueIndex = 0;

    private float _goNextTimer = 0;

    private void Start() {
        // StartCoroutine(blink());
        // StartCoroutine(talk());
        // StartCoroutine(drink());
        // StartCoroutine(helloCutscene());
        // StartCoroutine(goNextLoop());
    }

    private void FixedUpdate() {
        _goNextTimer += Time.deltaTime;

        if (_goNextTimer >= 0.3f) {
            _goNextTimer = 0;
            _animator.SetTrigger(_goNextHash);
        }
    }


    public void ShowDialogue(int index) {
        _dialogueIndex = 0;
        _bubbleTextFront.text = string.Empty;
        _bubbleTextBack.text = string.Empty;

        _bgRect.DOScaleY(1f, 0.3f).OnComplete(() => {
            _bubbleRect.DOScaleY(1, 0.3f).SetEase(Ease.OutBack).OnComplete(() => {
                _goalString = _sequences[index].dialogues[_dialogueIndex];
                _dialogueIndex++;
                StartCoroutine(animateText());
            });
        });
    }


    public void OnClick() {
        if (_dialogueIndex == _sequences[0].dialogues.Count) {
            _bubbleRect.DOScaleY(0, 0.3f);
            _bgRect.DOScaleY(0f, 0.3f).SetDelay(0.3f);
            GameManager.instance.DialogueComplete();
            return;
        }
        if (_isTalking) {
            StopCoroutine(animateText());
            _bubbleTextFront.text = _goalString;
            _bubbleTextBack.text = _goalString;
            _isTalking = false;
        } else {
            _goalString = _sequences[0].dialogues[_dialogueIndex];
            _dialogueIndex++;
            StartCoroutine(animateText());
        }
    }


    private IEnumerator animateText() {
        _isTalking = true;
        _bubbleRect.anchoredPosition = new Vector3(-250, -150, 0);
        yield return new WaitForSeconds(0.1f);

        _bubbleRect.anchoredPosition = new Vector3(-250, -140, 0);

        _bubbleTextFront.text = string.Empty;
        _bubbleTextBack.text = string.Empty;
        yield return new WaitForSeconds(0.1f);
        
        _bubbleRect.anchoredPosition = new Vector3(-250, -150, 0);


        foreach (char letter in _goalString.ToCharArray()) {

            if (char.IsDigit(letter)) {
                playAnimation(int.Parse("" + letter));
                continue;
            }

            _bubbleTextFront.text += letter;
            _bubbleTextBack.text += letter;

            float _letterInterval;
            if (letter == ' ') {
                _letterInterval = 0.05f;
                
            }
            else if (letter == '.' || letter == ',' || letter == ';' || letter == ':') {
                _letterInterval = 0.2f;
            }
            // else if (letter == '\n') {
            //     _letterInterval = 0.3f;
            // }
            else {
                // AudioManager.Instance.PlayLetterSound(FMODEvents.Instance.TalkingSounds, letter, this.transform.position);
                _letterInterval = Mathf.Clamp(_talkSpeed / _goalString.Length, 0.02f, 0.04f);
            }

            yield return new WaitForSeconds(_letterInterval);
        }
        _isTalking = false;
    }

    private void playAnimation(int index) {
        Debug.Log(index);
        _goNextTimer = 0f;
        _animator.SetBool(_goNextHash, false);
        _animator.SetInteger(_animationIndexHash, index);
        _animator.SetTrigger(_playAnimationHash);
    }


    private IEnumerator ayNoWay() {
        yield return new WaitForSeconds(2);

        _animator.SetTrigger("goNext");

        yield return new WaitForSeconds(0.15f);

        _animator.SetTrigger("goNext");

        yield return new WaitForSeconds(0.15f);

        _animator.SetTrigger("goNext");

        yield return new WaitForSeconds(0.15f);

        // _animator.SetTrigger("goNext");
    }

    private IEnumerator helloCutscene(float delay = 2f) {
        Debug.Log(delay);
        yield return new WaitForSeconds(3f);
        _animator.SetInteger("animationIndex", 1);
        _animator.SetTrigger("playAnimation");
        yield return new WaitForSeconds(1f);
        
        for (int i = 0; i < 34; i++) {
            _animator.SetTrigger("goNext");
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(10f);

        yield return helloCutscene(delay + 0.01f);
    }

    
}
