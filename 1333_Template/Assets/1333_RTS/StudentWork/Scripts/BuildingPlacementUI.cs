using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementUI : MonoBehaviour
{
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private GameObject ButtonPrefab;
    [SerializeField] private BuildingTypes BuildingData;

    // Start is called before the first frame update
    void Start()
    {
        foreach(BuildingData t in BuildingData.Buildings)
        {
            GameObject button = Instantiate(ButtonPrefab, layoutGroupParent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
