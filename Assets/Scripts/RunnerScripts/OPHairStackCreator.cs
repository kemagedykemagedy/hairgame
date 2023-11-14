using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OPHairStackCreator : MonoBehaviour
{
    [SerializeField] GameObject Player;
    public int StartHairNumber = 18;
    public int StartHairWidth = 5;
    public float PaddingWithLines;
    public float PaddingWithHairCells;
    [SerializeField] GameObject HairLine;
    [SerializeField] GameObject HairCell;
    public List<GameObject> Team;
    float childWidth;
    [SerializeField] Transform PoolParent;
    [SerializeField] Color BaseColor;


    int width = 30;//have to be even number
    int length = 30;
    int reminder = 30;
    private void Start()
    {
        StartHairNumber = PlayerPrefs.GetInt("OPStartHairNumber", StartHairNumber);
        StartHairWidth = PlayerPrefs.GetInt("OPStartHairWidth", StartHairWidth);

        CreateHairLines();
        CreateStartHairCells();
    }

    private void OnEnable()
    {
       ActionController.OnResetForNewLevel+=NewLevel;
    }
    private void OnDisable()
    {
        ActionController.OnResetForNewLevel-=NewLevel;
    }




    private void CreateHairLines()
    {
        
        CalculateSizes();
        for (int i = 0; i < length; i++)
        {
            
//            Debug.Log(new Vector3(0, 0, PaddingWithLines) * i);
            GameObject hairLineGO = Instantiate(HairLine,
                                                 Player.transform.position -
                                                 new Vector3(0, 0, PaddingWithLines) * i,
                                                 Quaternion.identity);
            
            Team.Add(hairLineGO);
        }
        ActionController.OnChangeOnTeam.Invoke(Team);
       
    }

    void CreateStartHairCells()
    {
//Debug.Log("CreateHairLines");
        for (int i = 0; i < Team.Count-(reminder==0?0:1); i++)
        {
            for (int k = 0; k < width; k++)
            {
                GameObject hairCellGO = Instantiate(HairCell, Vector3.zero, Quaternion.identity);
                hairCellGO.transform.SetParent(Team[i].transform);
                hairCellGO.GetComponent<HairCell>().PoolParent=PoolParent;
                hairCellGO.GetComponent<HairCell>().ChangeColor(BaseColor);
                
            }
        CenterAlignChildren(Team[i]);
        }
        if(reminder!=0)
        {
            for (int k = 0; k < reminder; k++)
            {
                GameObject hairCellGO = Instantiate(HairCell, Vector3.zero, Quaternion.identity);
                 hairCellGO.GetComponent<HairCell>().PoolParent=PoolParent;
                hairCellGO.transform.SetParent(Team[Team.Count-1].transform);
                hairCellGO.GetComponent<HairCell>().ChangeColor(BaseColor);
                
            }
            CenterAlignChildren(Team[Team.Count-1]);    
        }
        ActionController.OnChangeOnTeam.Invoke(Team);
    UIManager.Instance.UpdateHairNumber();
        
    }
 




    void CalculateSizes()
    {
        StartHairNumber = PlayerPrefs.GetInt("OPStartHairNumber", StartHairNumber);
        StartHairWidth = PlayerPrefs.GetInt("OPStartHairWidth", StartHairWidth);
        length = StartHairNumber / StartHairWidth;
        reminder = StartHairNumber % StartHairWidth;
        width = StartHairWidth;
        if(reminder!=0) length+=1;
        Debug.Log(StartHairNumber+" "+StartHairWidth+" "+length+" "+width+" "+reminder);
    }

    public void CenterAlignChildren(GameObject parentGO)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in parentGO.transform)
        {
            children.Add(child);
        }

        //children.Sort((a, b) => a.localPosition.x.CompareTo(b.localPosition.x));

        float totalWidth = 0f;
        foreach (Transform child in children)
        {
            childWidth=child.localScale.x;
            totalWidth += child.localScale.x; // You can also use child.GetComponent<Renderer>().bounds.size.x
        }

        Vector3 offset = new Vector3((totalWidth / -2f)+childWidth/2, 0f, 0f);
        foreach (Transform child in children)
        {
            Vector3 newPosition = child.localPosition + offset;
//            Debug.Log("offset "+offset);
            child.localPosition = new Vector3(newPosition.x, child.localPosition.y, 0);
            offset.x += child.localScale.x; // You can also use child.GetComponent<Renderer>().bounds.size.x
        }
    }

    public void NewLevel()
    {
       
        Team.Clear();
        CreateHairLines();
        CreateStartHairCells();
    }
    

}

