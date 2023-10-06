using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;
using System;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
[RequireComponent(typeof(SphereCollider))]
public class Collectable : MonoBehaviour
{
    public enum Item
    {
        power,
        bat,
        knife,
        Elf,
    }
    public Item item;
    private SphereCollider _trigger;
    public UIKeyPanel uiKeyPanel;
    public bool isKeyPressed = false;
    public bool isCancel = false;
    public Action pressAction = null;
    private bool isActive = false;
    public GameObject player;
    public GameObject clearUI;

    public IEnumerator IEnumKeyShow(Vector2 pos, Action pressAction, float showTime = 4.0f)
    {

        isKeyPressed = false;
        isCancel = false;
        uiKeyPanel.keyText.text = "E";
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
        if (isActive && Input.GetKeyDown(KeyCode.E))
        {
            isKeyPressed = true;
            transform.DOShakePosition(1f, 0.5f, 5, 0, false, true).OnComplete(() =>
            {
                if (item == Item.bat)
                {

                    player.GetComponent<PlayerItem>().batMushroom = true;
                    player.GetComponent<PlayerItem>().knifeItem = false;
                    player.GetComponent<PlayerItem>().knife.SetActive(false);
                    player.GetComponent<PlayerItem>().bat.SetActive(true);


                }

                else if (item == Item.power)
                {
                    player.GetComponent<PlayerItem>().powerMushRoom = true;
                    player.GetComponent<PlayerController>().m_jumpForce += 9.0f;
                    player.GetComponent<PlayerController>().m_walkScale += 3.0f;
                }
                else if (item == Item.knife)
                {
                    player.GetComponent<PlayerItem>().knifeItem = true;
                    player.GetComponent<PlayerItem>().batMushroom = false;
                    player.GetComponent<PlayerItem>().bat.SetActive(false);
                    player.GetComponent<PlayerItem>().knife.SetActive(true);

                }
                else if (item == Item.Elf)
                {
                    clearUI.SetActive(true);
                }
                Destroy(this.gameObject);
            });
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        isActive = true;
        if (other.gameObject.tag == "Player")
        {
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
