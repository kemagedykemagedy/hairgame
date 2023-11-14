using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class SpecialBottomDollSlot : MonoBehaviour
{


    [SerializeField] Image SpecialDollFillImage;


    [Header("Prefab References")]
    [SerializeField] private GameObject DollPhotoPrefab;


    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI DollNameText;
    [SerializeField] private TextMeshProUGUI PercentText;


    [Header("Gameobject References")]
    [SerializeField] private GameObject PhotoSlot;
    [SerializeField] private GameObject AffordableImage;

    
    [Header("Variables")]
    [HideInInspector] public int Price;
    [HideInInspector] public string Name;
    // Start is called before the first frame update
    void Start()
    {
SetNewSpecialDoll();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
    
    }
    private void OnDisable()
    {


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
        UpdateText(PercentText, Price.ToString());
    }
    public void SetName(string newName)
    {
        Name = newName;
        UpdateText(DollNameText, Name);
    }

     private void UpdateText(TextMeshProUGUI referenceText, string updatedText)
    {
        referenceText.SetText(updatedText);
    }

    public void SetNewSpecialDoll()
    {
        if (!IsChildExist())
        {
            AffordableImage.SetActive(true);
            
            GameObject newDollPhoto = Instantiate(DollPhotoPrefab, new Vector3(0, 0, 0), Quaternion.identity);
  
            newDollPhoto.transform.SetParent(PhotoSlot.transform);
            newDollPhoto.transform.SetAsFirstSibling();
            newDollPhoto.GetComponent<DollPhotoScript>().dollJob = MainUIManager.Instance.MapList[MainUIManager.Instance.ActiveMap].GetComponent<MapController>().SpecialDollJobs[0];
            newDollPhoto.GetComponent<DollPhotoScript>().startParent = PhotoSlot.transform;
            newDollPhoto.GetComponent<DollPhotoScript>().DollName = newDollPhoto.GetComponent<DollPhotoScript>().dollJob.ToString();
            newDollPhoto.GetComponent<DollPhotoScript>().Price = 0;
            SetName(newDollPhoto.GetComponent<DollPhotoScript>().dollJob.ToString());
            UpdateSpecialDollFillAmount(0f);
            SetPrice(0);
        }
            
    }
    public void UpdateSpecialDollFillAmount(float amount)
    {
        SpecialDollFillImage.fillAmount=amount;
        PercentText.text=SpecialDollFillImage.fillAmount.ToString();
        if(SpecialDollFillImage.fillAmount<=0) EnableSpecialDoll();
    }

    public void EnableSpecialDoll()
    {
         AffordableImage.SetActive(false);
    }
}

