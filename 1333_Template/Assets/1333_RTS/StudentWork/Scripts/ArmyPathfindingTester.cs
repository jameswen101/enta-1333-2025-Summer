using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ArmyPathfindingTester : MonoBehaviour
{
    public GridManager gridManager;
    public PathFinder pathFinder;
    [SerializeField] private AStarPathfinder aStarPathfinder;
    [SerializeField] private List<ArmyComposition> armyCompositions = new();
    [SerializeField] private int patrolRange = 0;
    [SerializeField] private float detectionRange = 3f;

    private readonly List<ArmyManager> armies = new();
    private enum UnitState { Patrol, Follow }
    private readonly Dictionary<UnitInstance, UnitState> unitStates = new();
    private readonly Dictionary<UnitInstance, Vector3[]> patrolPoints = new();
    private readonly Dictionary<UnitInstance, int> patrolTargetIndex = new();
    private readonly Dictionary<UnitInstance, UnitInstance[]> followTargets = new();
    private readonly Dictionary<UnitInstance, Vector3> lastKnownEnemyPos = new();

    private static readonly Color[] ArmyColors = new Color[]
    {
        Color.cyan, Color.red, Color.yellow, Color.green, Color.magenta, Color.blue, Color.white, Color.black
    };


    // Start is called before the first frame update
    void Start()
    {
        armies.Clear();
        for (int i = 0; i < armyCompositions.Count; i++)
        {
            ArmyManager army = new ArmyManager { ArmyID = i + 1, GridManager = gridManager };
            SpawnArmyUnits(army, armyCompositions[i]);  
            armies.Add(army);
            //UpdateArmyUnits
        }
    }
 
    private void SpawnArmyUnits(ArmyManager army, ArmyComposition composition) //army is the army you want to save units you spawn stuff into
    {
        ArmyComposition.UnitEntry spawnedUnit = new ArmyComposition.UnitEntry(); //composition is the stuff you want to spawn

        foreach (var entry in composition.entries) 
        {
            for (int i = 0; i < entry.count; i++)
            {
                int attempts = 0;
                int maxAttempts = 1000;
                Vector3 spawnPos = Vector3.zero;
                bool found = false;
                int unitWidth = entry.unitType.Width;
                int unitHeight = entry.unitType.Height;
                while (!found && attempts < maxAttempts)
                {
                    int x = Random.Range(0, gridManager.GridSettings.GridSizeX - unitWidth + 1);
                    int y = Random.Range(0, gridManager.GridSettings.GridSizeY - unitWidth + 1);
                    
                    if (IsRegionWalkable(x, y, unitWidth, unitHeight))
                    {
                        spawnPos = gridManager.GetNode(x, y).WorldPosition;
                        found = true;
                    }
                    
                    attempts++;
                }

                if (!found)
                {
                    Debug.LogWarning($"Failed to find valid spawn position for unit {entry.unitType.name}.");
                    continue;
                }
                GameObject go = Instantiate(entry.unitType.unitPrefab, spawnPos, Quaternion.identity);
                UnitInstance unit = go.GetComponent<UnitInstance>();
                unit.Initialize(pathFinder, entry.unitType, gridManager, aStarPathfinder);
                army.Units.Add(unit);
                unitStates[unit] = UnitState.Patrol;
                
                patrolPoints[unit] = new Vector3[2]
                {
                    GetRandomPatrolPoint(spawnPos, unit.Width, unit.Height),
                    GetRandomPatrolPoint(spawnPos, unit.Width, unit.Height)
                };
                
                patrolTargetIndex[unit] = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < armies.Count; i++)
        {
            ArmyManager ownArmy = armies[i];
            List<UnitInstance> enemyUnits = new();
            for (int j = 0; j < armies.Count; j++)
            {
                if (i == j) continue;
                enemyUnits.AddRange(armies[j].Units.Select(x => x as UnitInstance));
            }
            UpdateArmyUnits(ownArmy, enemyUnits);
        }
    }

    private void UpdateArmyUnits(ArmyManager ownArmy, List<UnitInstance> enemyUnits) //enemyUnits also used in Update
    {
        foreach (UnitInstance unit in ownArmy.Units)
        {
            if(unit == null) continue;
            unit.SetTarget(GetRandomPatrolPoint(unit.transform.position, unit.Width, unit.Height)); //calling GetRandomPatrolPoint for each unit

            /*
            UnitState state = unitStates[unit];
                        switch (state) //currently error
                        {

                            case UnitState.Patrol:
                    
                                UnitInstance enemy = FindNearestEnemy(unit, enemyUnits);
                                if (enemy != null)
                                {
                                    unitStates[unit] = UnitState.Follow;
                                    followTargets[unit] = enemy;
                                    lastKnownEnemyPos[unit] = enemy.transform.position;
                                    unit.SetTarget(enemy.transform.position);
                                }

                                else
                                {
                                    PatrolBehavior(unit);
                                }

                                break;
                                case UnitState.Follow:
                                if (!followTargets.ContainsKey(unit) || followTargets[unit] == null)
                                {
                                    unitStates[unit] = UnitState.Patrol;
                                    break;
                                }
                                UnitInstance target = followTargets[unit];
                                if (Vector3.Distance(lastKnownEnemyPos[unit], target.transform.position) > 0.5f)
                                {
                                    lastKnownEnemyPos[unit] = target.transform.position;
                                    unit.SetTarget(target.transform.position);
                                }
                                if(Vector3.Distance(unit.transform.position, target.transform.position) > detectionRange + 2)
                                {
                                    unitStates[unit] = UnitState.Patrol;
                                    break;
                                }
                                break;
                        }
            */
        }
    }

    public void Initialize()
    {
        armies.Clear();
        for (int i = 0; i < armyCompositions.Count; i++)
        {
            ArmyManager army = new ArmyManager { ArmyID = i + 1, GridManager = gridManager };
            SpawnArmyUnits(army, armyCompositions[i]);
            armies.Add(army);
        }
    }

    public bool IsRegionWalkable(int x, int y, int unitWidth, int unitHeight)
    {
        return true;
    }

    public Vector3 GetRandomPatrolPoint(Vector3 origin, int unitWidth, int unitHeight)
    {
        /*
        GridNode node = gridManager.GetNodeFromWorldPosition(origin);
        float nodeSize = gridManager.GridSettings.NodeSize;
        int nodeX = Mathf.RoundToInt(node.WorldPosition.x);
        int nodeY = 
        */
        int randomX = Random.Range(0, gridManager.GridSettings.GridSizeX);
        int randomY = Random.Range(0, gridManager.GridSettings.GridSizeY);
        return gridManager.GetNode(randomX, randomY).WorldPosition;
    }

    // Draws each unit's current path as a gizmo in a unique color per army.
    private void OnDrawGizmos()
    {
        // Loop through all armies.
        for (int armyIdx = 0; armyIdx < armies.Count; armyIdx++)
        {
            ArmyManager army = armies[armyIdx];
            // Pick a color for this army.
            Color color = ArmyColors[armyIdx % ArmyColors.Length];
            // Loop through all units in the army.
            foreach (UnitInstance unit in army.Units)
            {
                if (unit == null || unit.CurrentPath == null || unit.CurrentPath.Count < 2)
                    continue;
                // Set gizmo color for this army.
                Gizmos.color = color;
                // Draw lines between each node in the path.
                for (int i = 0; i < unit.CurrentPath.Count - 1; i++)
                {
                    Gizmos.DrawLine(unit.CurrentPath[i].WorldPosition, unit.CurrentPath[i + 1].WorldPosition);
                }
            }
        }
    }
}
