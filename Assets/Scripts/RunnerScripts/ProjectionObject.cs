using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class ProjectionObject : MonoBehaviour
    {
        bool inHead=false;
        bool inface=false;

        [SerializeField] DollHeadController dollHeadController;
        private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Head")
        {
            dollHeadController.OnFace = false;

          //Debug.Log("FaceHairMaskObj ENTER");

          //ActionController.OnHeadSpinning.Invoke(true, dollHeadController.OnFace);
          //ActionController.OnProjectonOnFace.Invoke(false);

        }
    }
   private void OnTriggerStay(Collider other) {
        
     
       
//Debug.Log("INFACE "+dollHeadController.OnFace);


   }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Head")
        {

            dollHeadController.OnFace = true;
//         //   Debug.Log("FaceHairMaskObj EXIT " );
           // //ActionController.OnHeadSpinning.Invoke(true,OnFace);
           // ActionController.OnProjectonOnFace.Invoke(true);
//

        }
    }
    }

