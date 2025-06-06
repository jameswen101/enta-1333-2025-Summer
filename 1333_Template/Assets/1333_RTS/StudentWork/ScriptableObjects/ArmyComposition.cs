using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ArmyComposition", menuName = "Game/Army Composition")]

public class ArmyComposition : ScriptableObject
{
    [System.Serializable]

    public class UnitEntry
    {
        public UnitType unitType;
        public int count = 1;
    }
    public List<UnitEntry> entries = new List<UnitEntry>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
