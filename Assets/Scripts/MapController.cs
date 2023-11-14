using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class MapController : MonoBehaviour
    {
        [SerializeField] GameObject DollPrefab;
        [SerializeField] GameObject DollsParent;
        [SerializeField] GameObject DollInstantiatePoint;
        [SerializeField] public List<Enums.DollJob> SpecialDollJobs=new();

        private void OnEnable()
    {
        ActionController.OnSpawnDolls+=SpawnDoll;
    }
    private void OnDisable()
    {
        ActionController.OnSpawnDolls-=SpawnDoll;
    }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SpawnDoll(List<GameObject> pointList,List<GameObject> patrolPoints)
        {
           for (int i = 0; i < 2; i++)
           {
            GameObject newDoll = Instantiate(DollPrefab, DollInstantiatePoint.transform.position, Quaternion.identity);
            //newDoll.transform.SetParent(DollsParent.transform);
            //newDoll.transform.localPosition=new Vector3(-8.26000023f,8.57000017f,1.5f);
            newDoll.GetComponent<Doll>().CreationPosition=pointList[i].transform.position;
            newDoll.GetComponent<Doll>().PatrolPoints=patrolPoints;
            newDoll.GetComponent<Doll>().CreateDoll();
           }
            

        }
    }

