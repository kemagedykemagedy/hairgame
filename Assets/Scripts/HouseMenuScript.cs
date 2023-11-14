using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HouseMenuScript : MonoBehaviour
{
    [SerializeField] List<GameObject> DollPhotoSlots = new();
    [SerializeField] Button BuildButton;
       


    private void Start() {
      
    }


    public void UpdateSlots()
    {
            ActivateBuildButton(CheckCompleteness());
    }

    public bool CheckCompleteness()
    {
        bool isCompleted=true;
        foreach (var photoSlot in DollPhotoSlots)
        {
            if(photoSlot.transform.childCount<=0) isCompleted=false;
        }

        return isCompleted;
    }

    void ActivateBuildButton(bool isActive)
    {
        BuildButton.gameObject.SetActive(isActive);
    }

    public void OnBuildButtonClicked()
    {
        ActionController.OnBuildButtonClicked.Invoke(this.gameObject);
         MainUIManager.Instance.CloseHouseMenus();
    }

    
}

