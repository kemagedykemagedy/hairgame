using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Runtime.CompilerServices;



public class BottomDollSlot : MonoBehaviour
{
   




    [Header("Prefab References")]
    [SerializeField] private GameObject DollPhotoPrefab;


    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI DollNameText;
    [SerializeField] private TextMeshProUGUI PriceText;


    [Header("Gameobject References")]
    [SerializeField] private GameObject PhotoSlot;
    [SerializeField] private GameObject DollBuyButton;
    [SerializeField] private GameObject AffordableImage;

    
    [Header("Variables")]
    [HideInInspector] public int Price;
    [HideInInspector] public string Name;

    string[] randomNames = new string[]
        {
            "Lily", "Bella", "Daisy", "Rosie", "Amber", "Ruby", "Crystal",
            "Sunny", "Poppy", "Jasmine", "Zoey", "Luna", "Hazel", "Mia",
            "Harper", "Grace", "Chloe", "Ava", "Ivy", "Willow"
        };

    // Start is called before the first frame update
    void Start()
    {
        SetNewDoll(null);

    }

    // Update is called once per frame
    void Update()
    {
      UpdateAffordablenessStuation();
    }

    private void OnEnable()
    {
        ActionController.OnBuildButtonClicked += SetNewDoll;
        ActionController.OnShopBuildButtonClicked += SetNewDoll;
    }
    private void OnDisable()
    {

        ActionController.OnBuildButtonClicked -= SetNewDoll;
        ActionController.OnShopBuildButtonClicked -= SetNewDoll;
    }

    public void UpdateAffordablenessStuation()
    {
        if (CheckIfAffordable())
        {
            
            DollBuyButton.GetComponent<Button>().interactable = true;
        }
        else
        {
         
            DollBuyButton.GetComponent<Button>().interactable = false;
        }
    }

    public bool CheckIfAffordable()
    {
        bool isAffordable = true;
        if (Price > MainUIManager.Instance.currentGemCount) isAffordable = false;
        return isAffordable;
    }

    public bool IsChildExist()
    {
        bool isChildExist = true;
        if (PhotoSlot.transform.childCount <= 0) isChildExist = false;
        return isChildExist;
    }

    public void SetPrice(int newPrice)
    {
        Price = newPrice;
        UpdateText(PriceText, Price.ToString());
    }
    public void SetName(string newName)
    {
        Name = newName;
        UpdateText(DollNameText, Name);
    }

    public void SetNewDoll(GameObject uselessGO)
    {
        if (!IsChildExist())
        {
            AffordableImage.SetActive(true);
            DollBuyButton.gameObject.SetActive(true);
            GameObject newDollPhoto = Instantiate(DollPhotoPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            newDollPhoto.transform.SetParent(PhotoSlot.transform);
            newDollPhoto.GetComponent<DollPhotoScript>().dollJob = Enums.DollJob.None;
            newDollPhoto.GetComponent<DollPhotoScript>().startParent = PhotoSlot.transform;
            newDollPhoto.GetComponent<DollPhotoScript>().DollName = "NewDoll";
            newDollPhoto.GetComponent<DollPhotoScript>().Price = Random.Range(1, 100);
            SetName(randomNames[Random.Range(0, randomNames.Length)]);
            SetPrice(Random.Range(0, 300));

        }
    }


    private void UpdateText(TextMeshProUGUI referenceText, string updatedText)
    {
        referenceText.SetText(updatedText);
    }

    public void OnBuyButtonClicked()
    {
        
            AffordableImage.SetActive(false);
            DollBuyButton.gameObject.SetActive(false);
            MainUIManager.Instance.currentGemCount-=Price;
            MainUIManager.Instance.UpdateCurrentGemNumber();
        
        
        
    }




}

