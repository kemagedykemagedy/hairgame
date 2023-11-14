using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HouseController : MonoBehaviour
{
    [Header("Gameobject References")]
    [SerializeField] GameObject HouseGround;
    [SerializeField] GameObject HouseModel;
    [SerializeField] GameObject GroundSprite;
    [SerializeField] GameObject HouseMenu;
    [SerializeField] public List<GameObject> DollCreationPoints;
    [SerializeField] List<GameObject> PatrolPoints=new();
    
    bool isConstructed=false;


    private void OnEnable()
    {
        ActionController.OnHouseGroundSelected+=HouseSelected;
        ActionController.OnBuildButtonClicked+=ActivateHouse;
    }
    private void OnDisable()
    {
        ActionController.OnHouseGroundSelected-=HouseSelected;
        ActionController.OnBuildButtonClicked-=ActivateHouse;
    }

    public void HouseSelected(GameObject selectedHouse)
    {
        if(selectedHouse==gameObject)
        {
            if(!isConstructed)
            {
                HouseMenu.SetActive(true);
            }
        }
    }

    public void ActivateHouse(GameObject houseMenu)
    {
        if(houseMenu==HouseMenu.gameObject)
        {
            if(!isConstructed)
            {
                HouseModel.SetActive(true);
                HouseGround.GetComponent<BoxCollider>().enabled=false;
                Debug.Log(DollCreationPoints.Count);
               ActionController.OnSpawnDolls.Invoke(DollCreationPoints,PatrolPoints);
            }
        }
    }
}

