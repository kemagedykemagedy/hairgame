using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhotoSlot : MonoBehaviour,IDropHandler
{
    [SerializeField] public Enums.DollJob dollJob;
    [SerializeField] GameObject DollSlotRemoveButton;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped=eventData.pointerDrag;
        DollPhotoScript dollPhotoScript=dropped.GetComponent<DollPhotoScript>();
        if(dollPhotoScript.dollJob==dollJob){
            dollPhotoScript.parentAfterDrag=transform;
        StartCoroutine(UpdateSlotCoroutine());
        DollSlotRemoveButton.SetActive(true);
        }
        
    }

    IEnumerator UpdateSlotCoroutine()
    {
        yield return new WaitForSeconds(0.01f);
        transform.parent.parent.parent.GetComponent<HouseMenuScript>().UpdateSlots();
    }

    public void OnClickRemoveButton()
    {
        DollSlotRemoveButton.SetActive(false);
        transform.GetChild(0).GetComponent<DollPhotoScript>().BackToStartParent();
        StartCoroutine(UpdateSlotCoroutine());
    }

    
}

