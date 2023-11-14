using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using System.Runtime.Remoting.Channels;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif


public class Gate : MonoBehaviour
{

    public enum GateType
    {
        Length,
        Width,
        Color,
        Shape
    }

    public GateType gateType;

    public enum GateShape
    {
        Straight,
        Curly,
        Wavy,
        Rasta
    }





    [ShowIf("gateType", GateType.Color)] public Color GateColor;
    [ShowIf("gateType", GateType.Shape)] public GateShape gateShape;
    [SerializeField] bool isAddition;
    bool isLength;
    [SerializeField] public int Amount;

    [SerializeField] TextMeshPro GateTypeText;
    [SerializeField] public TextMeshPro AmountText;

    [SerializeField] Material LengthMat;
    [SerializeField] Material WidthMat;
    [SerializeField] Material ColorMat;
    [SerializeField] Material ShapeMat;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] List<Sprite> SpriteList = new();
    [SerializeField] Sprite emptySprite;
    [SerializeField] List<ParticleSystem> particles = new();
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject ParticleFlowObj;
    [SerializeField] GameObject CycleObj;
    [SerializeField] int GateSplineOrder;
    [SerializeField] float DynamicGateAmountConstant;
    [SerializeField] int MaxHairNum;
    [SerializeField] int MinHairNum;
    [SerializeField] int MaxWidth;





    private void Start()
    {
        ApplyChanges(true);
    }
    private void OnValidate()
    {
        if (PrefabModeIsActive()) ApplyChanges(false);
    }

    bool PrefabModeIsActive()
    {
        bool isStart =true;
        #if UNITY_EDITOR
 isStart=PrefabStageUtility.GetCurrentPrefabStage() != null;
#endif
return isStart;
        
    }

    void ApplyChanges(bool isStart)
    {

        switch (gateType)
        {
            case GateType.Length:
                {
                    CycleObj.SetActive(false);
                    AmountText.gameObject.SetActive(true);
                    isLength = true;
                    AmountText.text = (isAddition ? "+" : "-") + Amount;
                    meshRenderer.sharedMaterial = LengthMat;
                    GateTypeText.text = "LENGTH";
                    ParticleFlowObj.SetActive(false);
                    spriteRenderer.sprite = emptySprite;
                    break;
                }
            case GateType.Width:
                {
                    CycleObj.SetActive(false);
                    AmountText.gameObject.SetActive(true);
                    isLength = false;
                    AmountText.text = (isAddition ? "+" : "-") + Amount;
                    meshRenderer.sharedMaterial = WidthMat;
                    GateTypeText.text = "WIDTH";
                    ParticleFlowObj.SetActive(false);
                    spriteRenderer.sprite = emptySprite;
                    break;
                }
            case GateType.Color:
                {
                    CycleObj.SetActive(false);
                    Material tempMat = new(LengthMat);
                    tempMat.color = GateColor;
                    AmountText.text = "";
                    meshRenderer.material = tempMat;
                    ParticleFlowObj.SetActive(true);
                    spriteRenderer.sprite = emptySprite;
                    SetColor();


                    GateTypeText.text = "COLOR";

                    break;
                }
            case GateType.Shape:
                {
                    meshRenderer.material = ShapeMat;
                    SetShape(gateShape);
                    AmountText.text = "";
                    GateTypeText.text = "SHAPE";
                    ParticleFlowObj.SetActive(false);
                    break;
                }
        }


        Color tempColor = meshRenderer.sharedMaterial.color;
        tempColor.a = 181;
        spriteRenderer.color = tempColor;



    }

    private void OnEnable()
    {
        ActionController.OnGateCrossed += UpdateGateAmount;
    }
    private void OnDisable()
    {
        ActionController.OnGateCrossed -= UpdateGateAmount;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(other.GetComponent<PlayerController>().TriggerActivationCoroutine());
            if (gateType == GateType.Length || gateType == GateType.Width)
            {
                ActionController.OnRowColumnAddRemove(isLength, isAddition, Amount, false);

            }
            else if (gateType == GateType.Color)
            {
                ActionController.OnColorChanged.Invoke(meshRenderer.material.color);
            }
            else if (gateType == GateType.Shape)
            {
                Debug.Log("Shape CHANGE");
                int hairType = 0;

                switch (gateShape)
                {
                    case GateShape.Straight:
                        {
                            hairType = 0;
                            break;
                        }
                    case GateShape.Curly:
                        {
                            hairType = 1;
                            break;
                        }
                    case GateShape.Wavy:
                        {
                            hairType = 2;
                            break;
                        }
                    case GateShape.Rasta:
                        {
                            hairType = 3;
                            break;
                        }
                }

                ActionController.OnShapeChanged.Invoke(hairType);
            }

            StartCoroutine(SinkGate());
        }
    }


    IEnumerator SinkGate()
    {
        yield return new WaitForSeconds(0.1f);
        transform.DOMoveY(-10f, 1);
    }


    void SetShape(GateShape shape)
    {
        CycleObj.SetActive(true);
        AmountText.gameObject.SetActive(false);
        switch (gateShape)
        {
            case GateShape.Curly:
                {
                    GateTypeText.text = "CURLY";

                    spriteRenderer.sprite = SpriteList[2];
                    break;
                }
            case GateShape.Straight:
                {
                    GateTypeText.text = "STRAIGHT";
                    spriteRenderer.sprite = SpriteList[0];
                    break;
                }
            case GateShape.Wavy:
                {
                    GateTypeText.text = "WAVY";
                    spriteRenderer.sprite = SpriteList[1];
                    break;
                }
            case GateShape.Rasta:
                {
                    GateTypeText.text = "RASTA";
                    spriteRenderer.sprite = SpriteList[3];
                    break;
                }

        }
    }

    void SetColor()
    {
        AmountText.gameObject.SetActive(false);
        foreach (var particle in particles)
        {
            particle.startColor = GateColor;
        }
    }

    public void UpdateGateAmount(int totalHairNum, int lengthNum, int widthNum)
    {
        int ComingAmount = Mathf.CeilToInt((gateType == GateType.Width ? (totalHairNum / lengthNum) : (totalHairNum / widthNum)) * Random.Range(0f, 20.0f));
        int ComingWidth = Mathf.CeilToInt(ComingAmount / (totalHairNum / lengthNum));
        if (lengthNum > widthNum)
        {
            if (gateType == GateType.Length)
            {
                if (ComingAmount > MaxHairNum * 0.9f) isAddition = false;
                ComingAmount = Mathf.CeilToInt((MaxHairNum / widthNum) / Random.Range(isAddition ? 2f : 4f, isAddition ? 3f : 5f));
                if (ComingAmount + totalHairNum < MinHairNum && isAddition)
                {
                    ComingAmount = Mathf.CeilToInt((MinHairNum - totalHairNum)*Random.Range(1f,1.2f));
                }

            }
            else
            {
                isAddition = true;
                ComingAmount = Mathf.CeilToInt(Random.Range(0.9f, 2.2f) * MaxHairNum / widthNum);
                if (ComingAmount + totalHairNum > MaxHairNum) ComingAmount = Mathf.CeilToInt((MaxHairNum - totalHairNum) * Random.Range(0.1f, 1f));
                ComingWidth = Mathf.CeilToInt(ComingAmount / (totalHairNum / lengthNum));
                if (ComingWidth > MaxWidth) ComingAmount = Mathf.CeilToInt((MaxWidth - (totalHairNum / lengthNum)) * lengthNum);
                if (ComingAmount <= 0) ComingAmount = 1;
                else if ((ComingAmount + totalHairNum) < MinHairNum)
                {
                    ComingAmount = Mathf.CeilToInt((MinHairNum - totalHairNum)*Random.Range(1f,1.2f));
                }
            }
        }
        else
        {
            if (gateType == GateType.Length)
            {
                isAddition = true;
                ComingAmount = Mathf.CeilToInt(Random.Range(0.9f, 2.2f) * MaxHairNum / lengthNum);
                if ((ComingAmount + totalHairNum) > MaxHairNum) ComingAmount = Mathf.CeilToInt((MaxHairNum - totalHairNum) * Random.Range(0.1f, 1f));
                if ((ComingAmount + totalHairNum) <= MinHairNum)
                {
                    ComingAmount = Mathf.CeilToInt((MinHairNum - totalHairNum)*Random.Range(1f,1.2f));
                }

            }
            else
            {

                if ((totalHairNum / lengthNum) > MaxWidth * 0.9f) isAddition = false;
                else
                {
                    isAddition = true;
                    if ((ComingAmount + totalHairNum) < MinHairNum)
                    {
                        ComingAmount = MinHairNum - (ComingAmount + totalHairNum);
                    }
                }
                ComingAmount = Mathf.CeilToInt((MaxHairNum / lengthNum) / Random.Range(isAddition ? 1f : 4f, isAddition ? 2f : 5f));
                if (ComingAmount + totalHairNum < MinHairNum && isAddition)
                {
                    ComingAmount = MinHairNum - (ComingAmount + totalHairNum);
                }
                if (isAddition && Mathf.CeilToInt(ComingAmount / (totalHairNum / lengthNum)) > MaxWidth)
                {
                    ComingAmount = (totalHairNum / widthNum) * (MaxWidth - widthNum);
                }
                if ((ComingAmount + totalHairNum) < MinHairNum )
                {
                    isAddition=true;
                    ComingAmount = Mathf.CeilToInt((MinHairNum - totalHairNum)*Random.Range(1f,1.2f));
                }
                if(!isAddition && ComingAmount>totalHairNum)
                {
                    ComingAmount=totalHairNum/2;
                }
                if (((totalHairNum+ComingAmount) / lengthNum) > MaxWidth * 0.9f){
                    isAddition = false;
ComingAmount=totalHairNum/2;

                } 

            }
        }


        Amount = ComingAmount;

        AmountText.text = (isAddition ? "+" : "-") + Amount;
    }
}

