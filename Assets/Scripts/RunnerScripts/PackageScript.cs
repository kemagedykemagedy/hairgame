using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class PackageScript : MonoBehaviour
    {
        [SerializeField] LevelEndController levelEndController;
        public void HeadTurnCompleted()
        {
levelEndController.OpenHairColor();
        }
    }

