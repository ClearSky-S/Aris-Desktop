using System;
using System.Collections;
using System.Collections.Generic;
using Assistant;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityWeld;
using UnityWeld.Binding;

[Binding]
public class DraggableImage : ViewModel, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler 
{
    private Canvas _canvas;
    [SerializeField] private GameObject _menu;
    [SerializeField] private Animator _animator;
    [SerializeField] private ActorData _actorData;
    [Header("대사")]
    private AudioSource _voiceSource;
    [SerializeField] private CanvasGroup _dialogCanvasGroup;
    [SerializeField] private TextMeshProUGUI _dialogText;

    
    
    private bool _isDragging = false;
    private TransparentWindow _transparentWindow;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _voiceSource = GetComponent<AudioSource>();
        _transparentWindow = GetComponentInParent<TransparentWindow>();
        _dialogCanvasGroup.alpha = 0;
        _menu.SetActive(false);
    }

    private void Start()
    {
        // 기동 대사
        PlayVoice(_actorData.GetLoginVoice());
    }

    public void NextDisplay()
    {
        _transparentWindow.NextMonitor();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Follow the mouse
        // convert mouse position to canvas position
        var mousePos = Input.mousePosition;
        // clamp to screen
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0, Screen.height);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, mousePos, _canvas.worldCamera, out Vector2 pos);
        transform.position = _canvas.transform.TransformPoint(pos);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        SetAnimState(1);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
        SetAnimState(0);
    }
    
    private void SetAnimState(int state)
    {
        if (_animator != null)
        {
            _animator.SetInteger("AnimState", state);
        }
    }
    
    private void PlayVoice(VoiceData voiceData)
    {
        _dialogText.text = voiceData.Text;
        _voiceSource.clip = voiceData.Clip;
        _voiceSource.Play();
        _dialogCanvasGroup.alpha = 1;
        _dialogCanvasGroup.DOKill();
        _dialogCanvasGroup.DOFade(0, 1).SetDelay(voiceData.Clip.length);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            _menu.SetActive(true);
        }
        if(_isDragging)
            return;
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        PlayVoice(_actorData.GetTouchVoice());
    }
}
