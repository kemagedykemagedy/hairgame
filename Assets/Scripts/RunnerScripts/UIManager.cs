using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.NiceVibrations;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using ES3Internal;
//using SupersonicWisdomSDK;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;



    [Header("Gameobject References")]
    [SerializeField] private GameObject startUI = default;
    [SerializeField] private GameObject inGameUI = default;
    [SerializeField] private GameObject endUI = default;
    [SerializeField] private GameObject winUI = default;
    [SerializeField] private GameObject failUI = default;
    [SerializeField] private GameObject moneyUI = default;
    [SerializeField] private GameObject dragToMove = default;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI moneyText = default;
    [SerializeField] private TextMeshProUGUI levelText = default;
    [SerializeField] private TextMeshProUGUI endUILevelText = default;
    [SerializeField] public TextMeshProUGUI HairNumber;

    [Header("RectTransform References")]
    [SerializeField] private RectTransform moneyIcon = default;
    [SerializeField] private RectTransform moneyIconPrefab = default;

    [SerializeField] public GameObject barObj = default;
    [SerializeField] public Image barParent = default;
    [SerializeField] public Image barFill = default;
    public float fillAmountPerHair = 0;
    public bool BarFillingActive = true;

    [SerializeField] public GameObject NewConcept = default;
    [SerializeField] public Image NewConceptParent = default;
    [SerializeField] public Image NewConceptFill = default;
    [SerializeField] public List<Sprite> ConceptSprites = new();
    public bool NewConceptFillActive = false;

    [SerializeField] Button StartWidthButton;
    [SerializeField] Button StartLenghtButton;
    [SerializeField] public Button RetryButton;
    [SerializeField] TextMeshProUGUI StartWidthButtonText;
    [SerializeField] TextMeshProUGUI StartLenghtButtonText;
    [SerializeField] int StartAdditionNumber = 10;
    internal bool isGameStarted = false;

    private string levelTextWriting = null;
    private int currentMoneyCount = 0;
    private int currentLevel = 0;
    [SerializeField] OPHairStackController oPHairStackController;

    public int CurrentConcept = 0;
    [SerializeField] int LevelStartWidthPrice = 10;
[SerializeField] int LevelStartLengthPrice = 10;
    [SerializeField] float newConceptFillAmount;
    bool onBarFilling = false;
    public int StartHairNumber;
    public int StartHairWidth;

    [SerializeField] GameObject SettingsPanel;
    [SerializeField] GameObject HapticButton;
    [SerializeField] GameObject SoundButton;
    [SerializeField] GameObject SettingsPanelCloseButton;

    [SerializeField] Sprite SoundOn;
    [SerializeField] Sprite SoundOff;
    [SerializeField] Sprite HapticOn;
    [SerializeField] Sprite HapticOff;

    [SerializeField] GameObject BackGroundSound;
    [SerializeField] public GameObject StartClickPrevent;
    [SerializeField] float LevelStartWidthPriceMultipler = 1.5f;
    [SerializeField] float LevelStartLengthPriceMultipler = 1.5f;
    [SerializeField] public int MultiplerChangeLevel;
    [SerializeField] public float WidthMultiplerChangeAmount;
    [SerializeField] public float WidthMaxStartPurchaseNumber;
    [SerializeField] public float LengthMultiplerChangeAmount;
    [SerializeField] public float LengthMaxStartPurchaseNumber;  
    float CurrentStartWidthPurchaseNumber = 0;
    float CurrentStartlengthPurchaseNumber = 0;
    public bool isSoundOn = true;
    public bool isHapticOn = true;

    public void OnSettingsButtonPressed()
    {
        if (!SettingsPanel.activeSelf){
            SettingsPanel.SetActive(true);
            SettingsPanelCloseButton.SetActive(true);
            
        }
            
        else SettingsPanel.SetActive(false);
    }

    public void OnSettingsPanelCloseButtonPressed()
    {
        SettingsPanel.SetActive(false);
        SettingsPanelCloseButton.SetActive(false);
    }


    public void OnSoundButtonPressed()
    {
        if (SoundButton.GetComponent<Image>().sprite == SoundOn)
        {

            SoundButton.GetComponent<Image>().sprite = SoundOff;
            ActionController.OnSoundSettingsChanged.Invoke(false);
            BackGroundSound.SetActive(false);
        }
        else
        {
            SoundButton.GetComponent<Image>().sprite = SoundOn;
            ActionController.OnSoundSettingsChanged.Invoke(true);
            BackGroundSound.SetActive(true);
            BackGroundSound.GetComponent<AudioSource>().Play();
        }

    }

    public void OnHapticButtonPressed()
    {
        if (HapticButton.GetComponent<Image>().sprite == HapticOn)
        {
            HapticButton.GetComponent<Image>().sprite = HapticOff;
            ActionController.OnHapticSettingsChanged.Invoke(false);
            isHapticOn = false;
        }
        else
        {
            HapticButton.GetComponent<Image>().sprite = HapticOn;
            ActionController.OnHapticSettingsChanged.Invoke(true);
            isHapticOn = true;
        }

    }
    private void Awake()
    {
        Instance = this;
        // Subscribe
        //SupersonicWisdom.Api.AddOnReadyListener(OnSupersonicWisdomReady);
        // Then initialize
        //SupersonicWisdom.Api.Initialize();


    }
    void OnSupersonicWisdomReady()
    {
        // Start your game from this point
    }

    private void OnEnable()
    {
        ActionController.OnLevelStarted += LevelStarted;
        ActionController.OnLevelCompleted += LevelCompleted;
        ActionController.OnLevelFailed += LevelFailed;
        ActionController.OnLevelEndReached += LevelEndReached;
        ActionController.OnNewLevelLoadCompleted += NewLevel;
        ActionController.OnHairReachToHead += FillBar;
        ActionController.OnHairDropCompleted += (() =>
        {
            barObj.SetActive(false);
        });

        ActionController.OnEarnGem+=((Vector3 pos)=>{
            SpawnMoney(pos,1);
        });

    }
    private void OnDisable()
    {
        ActionController.OnLevelStarted -= LevelStarted;
        ActionController.OnLevelCompleted -= LevelCompleted;
        ActionController.OnLevelFailed -= LevelFailed;
        ActionController.OnLevelEndReached -= LevelEndReached;
        ActionController.OnNewLevelLoadCompleted -= NewLevel;
        ActionController.OnHairReachToHead -= FillBar;
        ActionController.OnHairDropCompleted -= (() =>
        {
            barObj.SetActive(false);
        });
        ActionController.OnEarnGem-=((Vector3 pos)=>{
            SpawnMoney(pos,1);
        });

    }

    private void Start()
    {
        SetActivityOfUI(endUI, false);
        currentMoneyCount = PlayerPrefs.GetInt("Money", 5);
        currentLevel = PlayerPrefs.GetInt("Level", 0);
        levelTextWriting = "LEVEL " + (currentLevel + 1);
        LevelStartWidthPrice = PlayerPrefs.GetInt("LevelStartWidthPrice", 10);
        LevelStartLengthPrice = PlayerPrefs.GetInt("LevelStartLengthPrice", 10);
        StartHairNumber = PlayerPrefs.GetInt("OPStartHairNumber", StartHairNumber);
        StartHairWidth = PlayerPrefs.GetInt("OPStartHairWidth", StartHairWidth);
        // UpdateText(endUILevelText, levelTextWriting);
        UpdateText(levelText, levelTextWriting);
        UpdateText(moneyText, currentMoneyCount.ToString());
        StartWidthButtonText.text = LevelStartWidthPrice.ToString();
        StartLenghtButtonText.text = LevelStartLengthPrice.ToString();
    }

    private void Update()
    {
        //        Debug.Log("BarFillingActive "+BarFillingActive);
        if (BarFillingActive)
        {
            //   Debug.Log("BARFILL");
            // FillBar();
        }
        if (currentMoneyCount < LevelStartWidthPrice)
        {
            StartWidthButton.interactable = false;
        }
        else
        {
            StartWidthButton.interactable = true;
        }

        if (currentMoneyCount < LevelStartLengthPrice)
        {
            StartLenghtButton.interactable = false;
        }
        else
        {
            StartLenghtButton.interactable = true;
        }
        if (CurrentStartWidthPurchaseNumber > WidthMaxStartPurchaseNumber)
        {
            StartWidthButtonText.text = "MAX";
             StartWidthButton.interactable = false;
        }
if (CurrentStartlengthPurchaseNumber > LengthMaxStartPurchaseNumber)
        {
            StartLenghtButtonText.text = "MAX";
             StartLenghtButton.interactable = false;
        }
     if(StartWidthButtonText.text == "MAX")
     {
         StartWidthButton.interactable = false;
     }

    }


    public void LevelEndReached(List<GameObject> a)
    {
        barObj.SetActive(true);
        dragToMove.SetActive(true);
        StartCoroutine(SetActiveFalseDrag());
    }

    IEnumerator SetActiveFalseDrag()
    {
        yield return new WaitForSeconds(4f);
        dragToMove.SetActive(false);
    }


    #region Main Functions



    internal void ResetUI()
    {
        isGameStarted = false;

        SetActivityOfUI(startUI, true);
        SetActivityOfUI(inGameUI, false);
        SetActivityOfUI(endUI, false);
        SetActivityOfUI(failUI, false);
        
          
          
        SetActivityOfUI(winUI, false);
        // SetActivityOfUI(moneyUI, false);

    }







    internal void OnPlayerStartedLevel()
    {
        isGameStarted = true;
        PlayHaptics(HapticTypes.MediumImpact);
        SetActivityOfUI(startUI, false);
        SetActivityOfUI(inGameUI, true);
        // SetActivityOfUI(moneyUI, true);
        ActionController.OnLevelStarted.Invoke();
        RetryButton.gameObject.SetActive(false);
    }


    internal void OnPlayerCompletedLevel()
    {
        PlayHaptics(HapticTypes.MediumImpact);
        SetActivityOfUI(inGameUI, false);
        SetActivityOfUI(endUI, true);
        SetActivityOfUI(winUI.transform.parent.gameObject, true);
        SetActivityOfUI(winUI, true, true, 0.2f, 0.4f);
        SetActivityOfUI(moneyUI, true, true, 0.2f, 0.4f);
        ActionController.OnLevelCompleted.Invoke();
        Debug.Log("PlayerWin");
        currentLevel++;
        PlayerPrefs.SetInt("Level", currentLevel);
        //NewConcept.SetActive(true);
        FilNewConcept();
    }


    internal void OnPlayerFailedLevel()
    {
        PlayHaptics(HapticTypes.MediumImpact);
        SetActivityOfUI(inGameUI, false);
        SetActivityOfUI(endUI, true);
        //SetActivityOfUI(failUI.transform.parent.gameObject, true);
        SetActivityOfUI(failUI, true, false, 0.2f, 0.4f);
        
            Debug.Log("FAIL UI 2 OPEN");
           
        ActionController.OnLevelFailed.Invoke();
    }


    /// <param name="worldPos"></param>
    internal void SpawnMoney(Vector3 worldPos,bool spawnMoney)
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
        RectTransform current = Instantiate(moneyIconPrefab, screenPos, Quaternion.identity, transform);
        if(!spawnMoney) current.gameObject.SetActive(false);
        current.DOPunchScale(Vector3.one * 0.5f, 0.3f, 5).OnComplete(()=>{
current.DOMove(moneyIcon.position, 0.9f).OnComplete(() =>
        {
            Destroy(current.gameObject);
            currentMoneyCount += currentLevel;
            UpdateText(moneyText, currentMoneyCount.ToString());
        });
        });
        
    }

    internal void SpawnMoney(Vector3 worldPos, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if(i<20) SpawnMoney(worldPos,true);
            else SpawnMoney(worldPos,false);
        }
    }

    public void FillBar(bool o)
    {
        onBarFilling = true;
        // float angle = barFill.fillAmount;
        // DOTween.To(() => angle, x => angle = x, barFill.fillAmount+fillAmountPerHair, 0.08f)
        //     .OnUpdate(() =>
        //     {
        //         barFill.fillAmount = angle;

        //     });
        //        Debug.Log("FillBar BAR");
        if (o) barFill.fillAmount = barFill.fillAmount + fillAmountPerHair;
        onBarFilling = false;
        // barFill.fillAmount = Mathf.Lerp(barFill.fillAmount, barFill.fillAmount + fillAmountPerHair, 0.5f * Time.deltaTime);
    }
    public void FilNewConcept()
    {
        float q = 0;
        DOTween.To(() => q, x => q = x, 10, 5f).OnComplete(() =>
        {
            NewConcept.SetActive(true);
        });

        NewConceptFill.fillAmount = Mathf.Lerp(NewConceptFill.fillAmount, NewConceptFill.fillAmount + fillAmountPerHair, 0.5f * Time.deltaTime);
        float angle = NewConceptFill.fillAmount;
        DOTween.To(() => angle, x => angle = x, NewConceptFill.fillAmount + newConceptFillAmount, 1f)
            .OnUpdate(() =>
            {
                NewConceptFill.fillAmount = angle;

            }).OnComplete(() =>
            {
                // if(NewConceptFill.fillAmount>=1f) SetConceptPhotos();
            });

    }

    #endregion


    #region Utility Functions
    private void SetActivityOfUI(GameObject referenceGameobject, bool isActive, bool playPunchAnimation = false, float affectDegree = 0.2f, float animationTime = 0.4f)
    {
        if (playPunchAnimation)
        {
            PlayPunchAnimation(referenceGameobject, affectDegree, animationTime);
        }

        referenceGameobject.SetActive(isActive);
    }

    private void PlayPunchAnimation(GameObject referenceGameobject, float affectDegree = 0.2f, float animationTime = 0.4f, TweenCallback tweenCallback = null)
    {
        referenceGameobject.transform.DOPunchScale(Vector3.one * affectDegree, animationTime).SetEase(Ease.Linear).OnComplete(tweenCallback);
    }

    private void PlayScaleAnimation(GameObject referenceGameobject, float scaleDegree = 0.8f, float scaleTime = 0.5f, TweenCallback animationCallback = null)
    {
        referenceGameobject.transform.DOScale(scaleDegree, scaleTime).SetEase(Ease.Linear).OnComplete(animationCallback);
    }

    private void UpdateText(TextMeshProUGUI referenceText, string updatedText)
    {
        referenceText.SetText(updatedText);
    }

    private void PlayHaptics(HapticTypes hapticType = HapticTypes.LightImpact)
    {
        if (isHapticOn) MMVibrationManager.Haptic(hapticType);
    }

    #endregion
    #region Button Functions
    public void OnPlayButtonClicked()
    {
        OnPlayerStartedLevel();
    }
    public void OnRetryButtonClicked()
    {
        RetryButton.gameObject.SetActive(false);
        DOTween.KillAll(true);
        PlayerPrefs.SetInt("Money", currentMoneyCount);
        GameManager.Instance.RestartLevel();
        ResetUI();
    }

    public void OnNextButtonClicked()
    {
        barFill.fillAmount = 0f;
        DOTween.KillAll(true);
        PlayerPrefs.SetInt("Money", currentMoneyCount);
        GameManager.Instance.NextLevel();
        ResetUI();
    }

    public void NewLevel()
    {
        StartClickPrevent.SetActive(true);
        currentMoneyCount = PlayerPrefs.GetInt("Money", 0);
        currentLevel = PlayerPrefs.GetInt("Level", 0);
        levelTextWriting = "LEVEL " + (currentLevel + 1);
        //        UpdateText(endUILevelText, levelTextWriting);
        UpdateText(levelText, levelTextWriting);
        UpdateText(moneyText, currentMoneyCount.ToString());
        barFill.fillAmount = 0;
        if (NewConceptFill.fillAmount >= 1f) SetConceptPhotos();
        CurrentStartWidthPurchaseNumber = 0;
        CurrentStartlengthPurchaseNumber=0;
        StartLenghtButtonText.text = LevelStartWidthPrice.ToString();
            StartWidthButtonText.text = LevelStartWidthPrice.ToString();
            UpdateHairNumber();
    }

    public void SetConceptPhotos()
    {
        Debug.Log("SetNewConcept");
        if (CurrentConcept >= 3) return;
        CurrentConcept++;

        NewConceptParent.sprite = ConceptSprites[CurrentConcept];
        NewConceptFill.sprite = ConceptSprites[CurrentConcept];
        NewConceptFill.fillAmount = 0;


    }
    #endregion

    #region SDK Event Functions
    private void LevelStarted()
    {
        barObj.SetActive(false);

        int sdkSendLevel = currentLevel + 1;
        //Debug.Log("SDK LEVEL STARTED "+sdkSendLevel);
        //SupersonicWisdom.Api.NotifyLevelStarted(sdkSendLevel, null);
        if (currentLevel % MultiplerChangeLevel == 0)
        {
            LevelStartWidthPriceMultipler -= WidthMultiplerChangeAmount;
            if (LevelStartWidthPriceMultipler < 1) LevelStartWidthPriceMultipler = 1f;

            LevelStartLengthPriceMultipler -= LengthMultiplerChangeAmount;
            if (LevelStartLengthPriceMultipler < 1) LevelStartLengthPriceMultipler = 1f;
        }

        //HERE: SDK Level Started Event
    }

    private void LevelCompleted()
    {
        //HERE: SDK Level Completed Event
        int sdkSendLevel = currentLevel + 1;
        //Debug.Log("SDK LEVEL COMPLETED "+sdkSendLevel);
       // SupersonicWisdom.Api.NotifyLevelCompleted(sdkSendLevel, null);
    }

    private void LevelFailed()
    {
        //HERE: SDK Level Failed Event
        int sdkSendLevel = currentLevel + 1;
        //Debug.Log("SDK LEVEL FAILED "+sdkSendLevel);
        //SupersonicWisdom.Api.NotifyLevelFailed(sdkSendLevel, null);
    }
    #endregion

    public void OnBuyWidthStart()
    {
        if (CurrentStartWidthPurchaseNumber <= WidthMaxStartPurchaseNumber)
        {
            CurrentStartWidthPurchaseNumber++;
            if (currentMoneyCount >= LevelStartWidthPrice)
            {
                Debug.Log("currentMoneyCount "+currentMoneyCount);
                Debug.Log("LevelStartWidthPrice "+LevelStartWidthPrice);
                currentMoneyCount -= LevelStartWidthPrice;
                ActionController.OnRowColumnAddRemove(false, true, StartAdditionNumber,true);
                StartHairNumber = oPHairStackController.GetHairCellsGO().Count;
                StartHairWidth = oPHairStackController.GetWidth();
                UpdateText(moneyText, currentMoneyCount.ToString());
            }

            LevelStartWidthPrice = Mathf.CeilToInt(LevelStartWidthPrice * LevelStartWidthPriceMultipler);
          
            PlayerPrefs.SetInt("LevelStartWidthPrice", LevelStartWidthPrice);
            PlayerPrefs.SetInt("OPStartHairNumber", StartHairNumber);
            PlayerPrefs.SetInt("OPStartHairWidth", StartHairWidth);
            StartWidthButtonText.text = LevelStartWidthPrice.ToString();
     
        }
        else
        {
            StartWidthButtonText.text = "MAX";

            StartWidthButton.interactable = false;

        }
UpdateHairNumber();

Debug.Log("OPStartHairNumber"+PlayerPrefs.GetInt("OPStartHairNumber"));
Debug.Log("OPStartHairWidth"+PlayerPrefs.GetInt("OPStartHairWidth"));
    }
    public void OnBuyLenghtStart()
    {
        if (CurrentStartlengthPurchaseNumber <= LengthMaxStartPurchaseNumber)
        {
            CurrentStartlengthPurchaseNumber++;
            if (currentMoneyCount >= LevelStartLengthPrice)
            {
                currentMoneyCount -= LevelStartLengthPrice;
                ActionController.OnRowColumnAddRemove(true, true, StartAdditionNumber,true);
                StartHairNumber = oPHairStackController.GetHairCellsGO().Count;
                UpdateText(moneyText, currentMoneyCount.ToString());
            }
            
            LevelStartLengthPrice = Mathf.CeilToInt(LevelStartLengthPrice * LevelStartLengthPriceMultipler);
            PlayerPrefs.SetInt("LevelStartLengthPrice", LevelStartLengthPrice);
            PlayerPrefs.SetInt("OPStartHairNumber", StartHairNumber);
            PlayerPrefs.SetInt("OPStartHairWidth", StartHairWidth);
            StartLenghtButtonText.text = LevelStartLengthPrice.ToString();
  
        }
        else
        {
    
            StartLenghtButtonText.text = "MAX";
   
            StartLenghtButton.interactable = false;
        }

        UpdateHairNumber();
Debug.Log("OPStartHairNumber"+PlayerPrefs.GetInt("OPStartHairNumber"));
Debug.Log("OPStartHairWidth"+PlayerPrefs.GetInt("OPStartHairWidth"));
    }

    public void UpdateHairNumber()
    {
        HairNumber.text = oPHairStackController.GetHairCellsGO().Count.ToString();
        
    }
}
