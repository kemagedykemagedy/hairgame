using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

    public class CoinSafeController : MonoBehaviour
    {
        [SerializeField] GameObject ShopModel;
        [SerializeField] GameObject WarningSprite;
        public List<Vector3> posList=new();
        public GameObject CoinParent;
         public int rows = 3; // Number of rows in the grid.
    public int columns = 3; // Number of columns in the grid.
    public float padding = 0.1f; // Padding value to add around each grid element.
    public GameObject prefab; // Prefab to be instantiated as children of the GameObject.

    void Start()
    {
        CalculateAndPositionGrid();
    }

    private void OnEnable()
    {
        ActionController.OnCoinSafeSelected+=CollectCoin;

    }

    private void OnDisable()
    {
       ActionController.OnCoinSafeSelected+=CollectCoin;

    }

    void CalculateAndPositionGrid()
    {
        // Get the size of the prefab.
        Vector3 prefabSize = Vector3.zero;

        if (prefab != null)
        {
            Renderer prefabRenderer = prefab.GetComponent<Renderer>();
            if (prefabRenderer != null)
            {
                prefabSize = prefabRenderer.bounds.size;
            }
            else
            {
                Debug.LogError("Prefab does not have a Renderer component.");
                return;
            }
        }
        else
        {
            Debug.LogError("Prefab is not assigned.");
            return;
        }

        // Calculate the total padding.
        float totalPaddingX = (columns - 1) * padding;
        float totalPaddingZ = (rows - 1) * padding;

        // Calculate the available space for the grid.
        Vector3 availableSpace = CoinParent.GetComponent<Renderer>().bounds.size - new Vector3(totalPaddingX, 0f, totalPaddingZ);

        // Calculate the spacing between grid elements.
        float spacingX = availableSpace.x / columns;
        float spacingZ = availableSpace.z / rows;

        // Calculate and position the grid of prefabs.
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 localPosition = new Vector3(
                    col * (spacingX + padding) + (prefabSize.x / 2f),
                    0f,
                    row * (spacingZ + padding) + (prefabSize.z / 2f)
                );

               posList.Add(localPosition);
            }
        }
    }

    public void AddNewChild(GameObject newCoin)
    {
        if(ShopModel.activeSelf)
        {
            if(CoinParent.transform.childCount<posList.Count)
            {
                newCoin.transform.SetParent(CoinParent.transform);
        newCoin.transform.DOLocalJump(posList[CoinParent.transform.childCount-1],2f,1,0.2f,false);
            }else{
                WarningSprite.SetActive(true);
            }
             
        }
       
       // newCoin.transform.localPosition = posList[CoinParent.transform.childCount-1];

    }

    public void CollectCoin(GameObject CoinSafe)
    {
    
       // if(CoinSafe==this.gameObject)
        
          StartCoroutine(CollectCoinCoroutine());
          WarningSprite.SetActive(false);
        
        
    }

    IEnumerator CollectCoinCoroutine()
    {
        foreach (Transform child in CoinParent.transform)
        {
            ActionController.OnEarnCoin.Invoke(child.position);
            child.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.02f);
        }  
        int tempInt=CoinParent.transform.childCount;
        for (int i = 0; i < tempInt; i++)
        {
            Debug.Log("destroy coin");
            Destroy(CoinParent.transform.GetChild(i).gameObject);
        }

    }
    }

