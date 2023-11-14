using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{

public static InputController Instance;

private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                if(raycastHit.collider.CompareTag("HouseGround"))
                {
                    ActionController.OnHouseGroundSelected.Invoke(raycastHit.collider.transform.parent.gameObject);
                }else if(raycastHit.collider.CompareTag("ShopGround"))
                {
                    ActionController.OnShopGroundSelected.Invoke(raycastHit.collider.transform.parent.gameObject);
                }else if(raycastHit.collider.CompareTag("CoinSafe"))
                {
                    if(raycastHit.collider.gameObject==null) Debug.Log("raycastHit.collider.gameObject NULL");
                    ActionController.OnCoinSafeSelected.Invoke(raycastHit.collider.gameObject);
                }else if(raycastHit.collider.CompareTag("HouseModel"))
                {
                    
                }
               
            }
        }
    }
}



