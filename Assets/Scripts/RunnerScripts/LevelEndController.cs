using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;
using ToonyColorsPro;
using System.Linq.Expressions;

public class LevelEndController : MonoBehaviour
{
    public List<GameObject> HairCellGrid = new();
    [SerializeField] GameObject HairGun;
    [SerializeField] GameObject HairClip;
    [SerializeField] GameObject DollHeadGO;
    [SerializeField] GameObject DollHeadFinalParentGO;
    public bool isTap = false;
    public bool isFiring = false;
    public int TotalHairNumberOnHead = 0;
    public int SuccessHairNumber;
    [SerializeField] GameObject PackageGO;
    [SerializeField] int currentActiveBodyNumber = 0;
    [SerializeField] GameObject PlayerObj;
    [SerializeField] GameObject HairParent;
    [SerializeField] ParticleSystem HairPerfectionParticle;
    [SerializeField] GameObject BodiesGO;
    [SerializeField] GameObject HairCellFinalGO;
    [SerializeField] GameObject ProjectionObj;
    [SerializeField] GameObject stickableArea;
    bool isSuccess = false;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip SuccessSound;
    bool Haptic = true;
    bool Sound = true;
    [SerializeField] float barSuccessAmount;
    bool gameEnd=false;


    private void OnEnable()
    {
        ActionController.OnLevelEndReached += SetHairDict;
        ActionController.OnHeadSpinning += StartFiringHair;
        ActionController.OnHairReachToHead += UpdateTotalHairNumber;
        ActionController.OnSoundSettingsChanged += ((bool a) =>
        {
            Sound = a;
        });
        ActionController.OnHapticSettingsChanged += ((bool a) =>
        {
            Haptic = a;
        });
        ActionController.OnResetForNewLevel+=NewLevel;

    }
    private void OnDisable()
    {
        ActionController.OnLevelEndReached -= SetHairDict;
        ActionController.OnHeadSpinning -= StartFiringHair;
        ActionController.OnHairReachToHead -= UpdateTotalHairNumber;
        ActionController.OnSoundSettingsChanged -= ((bool a) =>
        {
            Sound = a;
        });
        ActionController.OnHapticSettingsChanged -= ((bool a) =>
        {
            Haptic = a;
        });
        ActionController.OnResetForNewLevel-=NewLevel;

    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        StartCoroutine(SetSpline());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SetSpline()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<SplinePositioner>().spline = GameObject.FindGameObjectWithTag("PlayerSpline").GetComponent<SplineComputer>();
        if (GameObject.FindGameObjectWithTag("PlayerSpline").GetComponent<SplineComputer>() == null) Debug.Log("Playersplinenull");
        GetComponent<SplinePositioner>().SetPercent(1);
        StartCoroutine(ActivateDollHeadParent());

    }

    IEnumerator ActivateDollHeadParent()
    {
        yield return new WaitForSeconds(0.5f);
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void PlaySound(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        if (Sound) audioSource.Play();
    }
    void SetHairDict(List<GameObject> hairDict)
    {
        Debug.Log("LEVEL END");
        HairCellGrid = hairDict;
gameEnd=false;
        StartCoroutine(OrderHairsToClip());
    }

    IEnumerator OrderHairsToClip()
    {
        Vector3 newHairPos = Vector3.zero;
        float moveDelay = 0;

        for (int i = 0; i < HairCellGrid.Count; i++)
        {

            if (i % 15 == 0)
            {
                newHairPos.x = 0;
                newHairPos += new Vector3(0, 0.3f, 0);
                moveDelay = 0.1f;
            }
            newHairPos += new Vector3(0.3f, 0, 0);



            HairCellGrid[i].transform.SetParent(HairClip.transform);
            //HairCellGrid[i].transform.DOLocalMove(newHairPos, 0.5f);
            HairCellGrid[i].transform.DOLocalJump(newHairPos, 5f, 1, 1f, false);
            HairCellGrid[i].transform.localEulerAngles = new Vector3(-90, 0, 0);
            HairCellGrid[i].transform.GetChild(0).GetComponent<DynamicBone>().m_Force = Vector3.zero;
            // hairCell.GetComponent<Rigidbody>().useGravity = true;
            // hairCell.GetComponent<HairCell>().isFalling = true;
            //yield return new WaitForSeconds(moveDelay);
            //if(moveDelay!=0) moveDelay=0;

        }
        yield return new WaitForSeconds(0.001f);

    }



    IEnumerator FireHair()
    {

        int num = HairCellGrid.Count;
        for (int i = num - 1; i >= 0; i--)
        {
            if (!isFiring) break;
            if (i % 15 == 0)
            {
                //HairClip.transform.DOLocalMoveY(HairClip.transform.localPosition.y - 0.1f, 0.1f);
            }

            try
            {
                HairCellGrid[i].transform.SetParent(HairGun.transform);
                HairCellGrid[i].transform.DOLocalMove(Vector3.zero, 0.05f);
                HairCellGrid[i].transform.localEulerAngles = new Vector3(0, 0, 0);
                HairCellGrid[i].GetComponent<Rigidbody>().useGravity = true;

                HairCellGrid[i].GetComponent<HairCell>().isFalling = true;
                HairCellGrid.Remove(HairCellGrid[i]);

            }
            catch
            {

            }
            yield return new WaitForSeconds(0.2f);
        }

        if (HairCellGrid.Count == 0 && !gameEnd)
        {

            if (!isSuccess && TotalHairNumberOnHead > SuccessHairNumber)
            {
                isSuccess = true;
        gameEnd=true;
                StartCoroutine(EndGame());

                //Run packageAnim
            }
        }
    }

    void ChangeProjectionMat(bool isFire)
    {
        Color tempColor = new();
        if (isFire) tempColor = Color.green;
        else tempColor = Color.red;
        tempColor.a = 0.35f;
        ProjectionObj.GetComponent<MeshRenderer>().material.color = tempColor;
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1f);
        PrepareHairColor();

        GetComponent<SplinePositioner>().spline.transform.parent.gameObject.SetActive(false);

        DollHeadGO.GetComponent<DollHeadController>().ResetHeadPos();
        DollHeadGO.GetComponent<DollHeadController>().enabled = false;

        DollHeadGO.GetComponent<DollHeadController>().isDollHeadActive = false;
        GetComponent<SplinePositioner>().enabled = false;
        if (UIManager.Instance.barFill.fillAmount > barSuccessAmount)
        {
            ActionController.OnHairDropCompleted.Invoke();




            //transform.DOMove(new Vector3(transform.position.x,transform.position.y+7.91f,transform.position.z+39.72f),1f);
            yield return new WaitForSeconds(1f);
            PackageGO.SetActive(true);
            DollHeadGO.transform.SetParent(DollHeadFinalParentGO.transform);
            yield return new WaitForSeconds(1f);
            //OpenHairColor();
           


            yield return new WaitForSeconds(2f);

            GameManager.Instance.EndGame(true, TotalHairNumberOnHead);
        }
        else
        {
            GameManager.Instance.EndGame(false, 0);
        }
        //transform.GetChild(0).gameObject.SetActive(false);

        UIManager.Instance.barObj.SetActive(false);
    }

    void StartFiringHair(bool firing, bool onface)
    {

        if (firing)
        {
            isFiring = true;

            StartCoroutine(FireHair());
            ChangeProjectionMat(true);
        }
        else
        {
            isFiring = false;
            StopCoroutine(FireHair());
            ChangeProjectionMat(false);
        }



    }

    public void UpdateTotalHairNumber(bool onHead)
    {
        if (onHead)
        {
            TotalHairNumberOnHead++;

        }

    }


    public void OpenHairColor()
    {
        foreach (Transform child in HairCellFinalGO.transform)
        {
            if (child.CompareTag("HairCell")) Destroy(child.gameObject);
        }
        PlaySound(SuccessSound);
        HairPerfectionParticle.Play();

        HairParent.transform.GetChild(PlayerObj.GetComponent<PlayerController>().ActiveHairNum).gameObject.SetActive(true);
        Color tempCol = Color.blue;
        tempCol.a = 255;
        HairParent.transform.GetChild(PlayerObj.GetComponent<PlayerController>().ActiveHairNum).gameObject.GetComponent<MeshRenderer>().material.color = PlayerObj.GetComponent<PlayerController>().ActiveColor;

    }
    public void PrepareHairColor()
    {
        foreach (Transform child in HairParent.transform)
        {
            child.gameObject.SetActive(false);
        }
        foreach (Transform child in BodiesGO.transform)
        {
            child.gameObject.SetActive(false);
        }
        BodiesGO.transform.GetChild(UIManager.Instance.CurrentConcept).gameObject.SetActive(true);
        //        Debug.Log(PlayerObj.GetComponent<PlayerController>().ActiveHairNum);


    }

    public void NewLevel()
    {
       
        foreach (Transform child in stickableArea.transform)
        {
           
            if (child.CompareTag("HairCell")) Destroy(child.gameObject);
        }

        PackageGO.SetActive(false);
        GetComponent<SplinePositioner>().enabled = true;
        HairCellGrid.Clear();

        DollHeadGO.GetComponent<DollHeadController>().enabled = true;
        DollHeadGO.GetComponent<SphereCollider>().enabled = true;
        DollHeadGO.transform.SetParent(this.transform);
        StartCoroutine(SetSpline());
        DollHeadGO.GetComponent<DollHeadController>().enabled = true;
        DollHeadGO.GetComponent<DollHeadController>().isDollHeadActive = true;
        isSuccess = false;
        StartCoroutine(ActivateDollHeadParent());
    }
}

