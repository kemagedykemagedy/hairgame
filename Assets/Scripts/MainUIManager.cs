using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.NiceVibrations;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using ES3Internal;



public class MainUIManager : MonoBehaviour
{
    public static MainUIManager Instance;


    [Header("Gameobject References")]
    [SerializeField] private GameObject StartUI = default;
    [SerializeField] private GameObject InGameUI = default;
    [SerializeField] private GameObject CommonUI = default;
    [SerializeField] private GameObject HouseMenusParent = default;
    [SerializeField] public List<GameObject> MapList = new();

    [Header("RectTransform References")]
    [SerializeField] private RectTransform coinIcon = default;
    [SerializeField] private RectTransform coinIconPrefab = default;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI levelText = default;
    [SerializeField] private TextMeshProUGUI gemText = default;
    [SerializeField] private TextMeshProUGUI coinText = default;

    [Header("Image References")]
    [SerializeField] private Image NewConceptParent = default;

    [Header("Button References")]
    [SerializeField] private Button NextMapButton;
    [SerializeField] private Button PerviousMapButton;
    [SerializeField] private Button CloseHouseMenuButton;


    [Header("Variables")]
    [SerializeField] public int currentCoinNumber = 0;
    public int currentGemCount = 10;
    public int ActiveMap = 0;



    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UpdateCurrentGemNumber();
    }

    private void OnEnable()
    {
        ActionController.OnHouseGroundSelected += HouseSelected;
        ActionController.OnShopGroundSelected += HouseSelected;
        ActionController.OnEarnCoin+=((Vector3 pos)=>{
            SpawnCoin(pos,1);
        });
    }
    private void OnDisable()
    {
        ActionController.OnHouseGroundSelected -= HouseSelected;
        ActionController.OnShopGroundSelected -= HouseSelected;
        ActionController.OnEarnCoin-=((Vector3 pos)=>{
            SpawnCoin(pos,1);
        });
    }









    #region Button Functions
    public void OnBackToMainButtonClicked()
    {
        StartUI.SetActive(true);
        InGameUI.SetActive(false);
    }
    public void OnIdlePlayButtonClicked()
    {
        StartUI.SetActive(false);
        InGameUI.SetActive(true);
    }
    public void OnNextMapButtonClicked()
    {
        if (ActiveMap < 3)
        {
            ActiveMap++;
            CameraContoller.Instance.ChangeCameraPosition(ActiveMap);
        }

    }
    public void OnPreviousMapButtonClicked()
    {
        if (ActiveMap > 0)
        {
            ActiveMap--;
            CameraContoller.Instance.ChangeCameraPosition(ActiveMap);
        }
    }

    public void OnBackButtonClicked()
    {

    }
    public void CloseHouseMenuButtonClicked()
    {
        CloseHouseMenus();
    }
    #endregion

    #region Gem and Coin Functions
    /// <param name="worldPos"></param>
    internal void SpawnCoin(Vector3 worldPos, bool spawnCoin)
    {
        Vector3 screenPos = Vector3.zero;
        if (worldPos != Vector3.zero)
        {
            screenPos = Camera.main.WorldToScreenPoint(worldPos);
        }
        else
        {
            screenPos = new Vector3(Screen.width / 2, Screen.height / 2, 0) + (Vector3)Random.insideUnitCircle * 300f;
            screenPos.z = 0;
        }
        RectTransform current = Instantiate(coinIconPrefab, screenPos, Quaternion.identity, transform);
        if (!spawnCoin) current.gameObject.SetActive(false);
        current.DOPunchScale(Vector3.one * 0.3f, 0.3f, 5).OnComplete(() =>
        {
            current.DOMove(coinIcon.position, 0.9f).OnComplete(() =>
                    {
                        Destroy(current.gameObject);
                        currentCoinNumber += 1;
                        UpdateText(coinText, currentCoinNumber.ToString());
                    });
        });

    }

    internal void SpawnCoin(Vector3 worldPos, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (i < 20) SpawnCoin(worldPos, true);
            else SpawnCoin(worldPos, false);
        }
    }

    #endregion

    void HouseSelected(GameObject selectedHouse)
    {
        CloseHouseMenuButton.gameObject.SetActive(true);
    }


    public void CloseHouseMenus()
    {
        foreach (Transform child in HouseMenusParent.transform)
        {
            if (child.CompareTag("HouseMenu")) child.gameObject.SetActive(false);
            CloseHouseMenuButton.gameObject.SetActive(false);
        }
    }

    public void UpdateCurrentGemNumber()
    {
        UpdateText(gemText, currentGemCount.ToString());
    }

    private void UpdateText(TextMeshProUGUI referenceText, string updatedText)
    {
        referenceText.SetText(updatedText);
    }


}

