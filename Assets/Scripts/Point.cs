using Sirenix.OdinInspector;
using UnityEngine;

public class Point : MonoBehaviour
{
    public string PointName;
    public Enums.PointTypes PointType;
    [ShowIf("PointType",Enums.PointTypes.Shop)]
    public ShopController ShopController;
    public float WaitTime;
}
