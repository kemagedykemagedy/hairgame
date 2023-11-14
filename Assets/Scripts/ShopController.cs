using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopController : MonoBehaviour
{
    [Header("Gameobject References")]
    [SerializeField] GameObject ShopGround;
    [SerializeField] GameObject ShopModel;
    [SerializeField] GameObject GroundSprite;
    [SerializeField] GameObject ShopMenu;
    [SerializeField] GameObject CoinPrefab;
    [SerializeField] CoinSafeController coinSafeController;
    [SerializeField] public List<GameObject> DollCreationPoints;
    [SerializeField] List<GameObject> PatrolPoints=new();

    public bool isConstructed = false;


    private void OnEnable()
    {
        ActionController.OnShopGroundSelected += ShopSelected;
   
    }
    private void OnDisable()
    {
        ActionController.OnShopGroundSelected -= ShopSelected;
      
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Doll"))
        {
            SpawnCoin();//triggerda değil spline triggerda cagırılacak dollar shoptan cıkarken ya da girince çalışacak
        }
    }

    public void SpawnCoin()
    {
        GameObject newCoin = Instantiate(CoinPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newCoin.transform.SetParent(transform);
        newCoin.transform.localPosition=Vector3.zero;
        coinSafeController.AddNewChild(newCoin);

    }

    public void ShopSelected(GameObject selectedShop)
    {
        if (selectedShop == gameObject)
        {
            if (!isConstructed)
            {
                ShopMenu.SetActive(true);
            }
        }
    }

    public void ActivateShop(Button activateButton)
    {
        
            if (!isConstructed)
            {
                isConstructed = true;
                ShopModel.SetActive(true);
                ShopGround.GetComponent<BoxCollider>().enabled = false;
                activateButton.transform.parent.gameObject.SetActive(false);

            }

            ActionController.OnShopBuildButtonClicked.Invoke(activateButton.gameObject);
             ActionController.OnSpawnDolls.Invoke(DollCreationPoints,PatrolPoints);
        
    }
}

