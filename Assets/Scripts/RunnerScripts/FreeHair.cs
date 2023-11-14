using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
    public class FreeHair : MonoBehaviour
    {
    


    public GameObject prefab; // The prefab you want to spawn.
    public int rows = 5; // Number of rows in the grid.
    public int columns = 5; // Number of columns in the grid.
    public float spacing = 1.0f; // Spacing between grid elements.
    [SerializeField] Color BaseColor;
    public bool SpawnHair;
    
    

    

    void Start()
    {
       // SpawnGrid();
             ChangeColor(BaseColor);
    }

    private void OnEnable()
    {
        ActionController.OnColorChanged += ChangeColor;
        ActionController.OnShapeChanged += ChangeShape;

    }

    private void OnDisable()
    {
        ActionController.OnColorChanged -= ChangeColor;
        ActionController.OnShapeChanged -= ChangeShape;

    }

    private void OnValidate() {
        if(SpawnHair)
        {
            SpawnGrid();
            SpawnHair=false;
        }
        
    }

    void SpawnGrid()
    {
        //int tempint=transform.GetChild(0).childCount;
        //for (int i = 0; i < tempint; i++)
        //{
        //    DestroyImmediate(transform.GetChild(0).GetChild(0).gameObject);
        //}
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 spawnPosition = new Vector3( row* spacing, 0,  col* spacing);
                Quaternion spawnRotation = Quaternion.identity;

                
                GameObject newObject = Instantiate(prefab, spawnPosition, spawnRotation);
                newObject.transform.SetParent(transform.GetChild(0));
                newObject.transform.localPosition=new Vector3(newObject.transform.localPosition.x,
                                                              0,
                                                              newObject.transform.localPosition.z+transform.position.z);
                
                newObject.GetComponent<HairCell>().HairActivation(true);
                newObject.GetComponent<BoxCollider>().enabled=false;
              
               
            }
        }
  
        UpdateText();
CenterAlignHairParent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(other.GetComponent<PlayerController>().TriggerActivationCoroutine());
           
            ActionController.OnRowColumnAddRemove(true, true, rows*columns,false);

            StartCoroutine(SinkGate());
        }
    }


    IEnumerator SinkGate()
    {
        yield return new WaitForSeconds(0.1f);
        transform.DOMoveY(-10f,1);
    }

    void ChangeColor(Color color)
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
        
            transform.GetChild(0).GetChild(i).GetComponent<HairCell>().ChangeColor(color);
        }
    }

    void ChangeShape(int shapeNum)
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
        
            transform.GetChild(0).GetChild(i).GetComponent<HairCell>().ChangeModel(shapeNum);
        }
    }

    void UpdateText()
    {
        transform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>().text=(rows*columns).ToString();
    }
    void CenterAlignHairParent()
    {
        transform.GetChild(0).localPosition=new Vector3(-rows*0.2f/2,0,0);
    }
}

