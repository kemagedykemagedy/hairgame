using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DollPhotoScript : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [SerializeField] public string DollName;
    [SerializeField] public int Price;
    [SerializeField] public Enums.DollJob dollJob;

    [HideInInspector] public Image image;
     Canvas canvas;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Transform startParent;

    private void Start() {
        image=GetComponent<Image>();
        canvas=GameObject.FindGameObjectWithTag("MainGameUI").GetComponent<Canvas>();
        startParent=transform.parent;
    }
   public void OnBeginDrag(PointerEventData eventData)
   {
    parentAfterDrag=transform.parent;
    transform.SetParent(canvas.transform);
    transform.SetAsLastSibling();
    image.raycastTarget=false;

   }

   public void OnDrag(PointerEventData eventData)
   {
    transform.position=Input.mousePosition;
   }

   public void OnEndDrag(PointerEventData eventData)
   {
    transform.SetParent(parentAfterDrag);
    image.raycastTarget=true;

   }

   public void BackToStartParent()
   {
    transform.SetParent(startParent);
   }
}

