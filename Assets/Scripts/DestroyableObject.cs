using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

[RequireComponent(typeof(SphereCollider))]
public class DestroyableObject : MonoBehaviour
{
    private SphereCollider _trigger;
    public UIKeyPanel uiKeyPanel;
    public bool isKeyPressed = false;
    public bool isCancel = false;
    public Action pressAction = null;
    private bool isActive = false;
    public GameObject player;
    public enum Destroyable
    {
        batDestroyable,
        knifeDestroyable
    }
    public Destroyable destroyable;

    public IEnumerator IEnumKeyShow(Vector2 pos, Action pressAction, float showTime = 4.0f)
    {

        isKeyPressed = false;
        isCancel = false;
        uiKeyPanel.keyText.text = "Q";
        uiKeyPanel.DOKill();
        uiKeyPanel.GetComponent<RectTransform>().anchoredPosition = pos;
        uiKeyPanel.transform.localScale = Vector3.one;
        uiKeyPanel.keyRect.DOKill();
        uiKeyPanel.keyRect.anchoredPosition = Vector2.zero;
        uiKeyPanel.keyRect.DOLocalMoveY(-12f, 0.2f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);

        while (!isKeyPressed && !isCancel)
        {
            yield return null;
        }

        uiKeyPanel.transform.DORewind();
        uiKeyPanel.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).SetUpdate(true);


        if (pressAction != null && isKeyPressed) { pressAction(); }
    }

    private void Awake()
    {
        _trigger = this.GetComponent<SphereCollider>();
        _trigger.isTrigger = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Q))
        {
            isKeyPressed = true;
            player.GetComponent<PlayerController>().m_animator.SetTrigger("Smash");
            transform.DOShakePosition(1f, 0.5f, 5, 0, false, true).OnComplete(() =>
            {
                transform.DORotate(new Vector3(-90, 0, 0), 1).SetRelative(true).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(this.gameObject);
                });
            });


        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player"&&destroyable==Destroyable.batDestroyable&& other.GetComponent<PlayerItem>().batMushroom==true)
        {
            isActive = true;
            StartCoroutine(
            IEnumKeyShow(
                            new Vector2(UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(-30, 30)), pressAction, 4.0f
                        ));
        } else if(other.gameObject.tag == "Player" && destroyable == Destroyable.knifeDestroyable && other.GetComponent<PlayerItem>().knifeItem == true)
        {
            isActive = true;
            StartCoroutine(
            IEnumKeyShow(
                            new Vector2(UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(-30, 30)), pressAction, 4.0f
                        ));
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != this && other.gameObject.tag == "Player")
        {
            isCancel = true;
            isActive = false;
        }
    }
}
