using System;
using System.Collections.Generic;
using ToolBox.Pools;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralParkourBehaviour : MonoBehaviour
{
    [Header("Car")]
    [SerializeField] private List<AiCarBehaviour> aiCarBehaviourPrefab;
    [SerializeField] private int onStartTileSkipCount = 1;
    [SerializeField] [Range(0f, 1f)] private float spawnChance;
    
    [Header("Tiles")]
    [SerializeField] private float minPositionZ;
    [SerializeField] private float maxPositionZ;
    [SerializeField] private ParkourObject parkourObjectPrefab;
    [SerializeField] private Transform playerControllerTransform;
    [SerializeField] private int spawnCount = 10;
    
    private List<ParkourObject> _instantiatedGameObjects;
    private Quaternion _minRotation;
    private Quaternion _maxRotation;

    private Vector3 BehindCharacterSpawnedLength =>
        (parkourObjectPrefab.startTransform.position + parkourObjectPrefab.targetTransform.position);

    private bool IsSpawn => Random.Range(0f, 1f) <= spawnChance;

    private void Awake()
    {
        CharacterMoveBehaviour.onDeath += () =>
        {
            AiCarBehaviour.ReleaseAllCars();
            Clear();
            Initialize();
        };
        
        foreach (AiCarBehaviour aiCarBehaviour in aiCarBehaviourPrefab)
        {
            aiCarBehaviour.gameObject.Populate(5);
        }
        
        parkourObjectPrefab.gameObject.Populate(15);
        _instantiatedGameObjects = new List<ParkourObject>();
    }

    private void OnDestroy()
    {
        CharacterMoveBehaviour.onDeath -= () =>
        {
            AiCarBehaviour.ReleaseAllCars();
            Clear();
            Initialize();
        };
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        
        GameObject pooledObject = parkourObjectPrefab.gameObject.Reuse(playerControllerTransform.position - BehindCharacterSpawnedLength, Quaternion.identity);
        ParkourObject parkourComponent = pooledObject.GetComponent<ParkourObject>();
        parkourComponent.Deform(0);
        _instantiatedGameObjects.Add(parkourComponent);
        
        for (int i = 0; i < spawnCount - 1; i++)
        {
            SpawnSingle(i >= onStartTileSkipCount && IsSpawn);
        }
    }

    private void Clear()
    {
        foreach (ParkourObject instantiatedGameObject in _instantiatedGameObjects)
        {
            instantiatedGameObject.gameObject.Release();
        }
        
        _instantiatedGameObjects.Clear();
    }

    private float GetRandomPositionX()
    {
        return Random.Range(minPositionZ, maxPositionZ);
    }

    private bool IsRelease(ParkourObject parkourObject)
    {
        return parkourObject.targetTransform.position.z < playerControllerTransform.position.z - BehindCharacterSpawnedLength.z;
    }

    private void SpawnSingle(bool spawnCar)
    {
        GameObject pooledObject = parkourObjectPrefab.gameObject.Reuse(_instantiatedGameObjects[^1].targetTransform.position, Quaternion.identity);
        ParkourObject parkourComponent = pooledObject.GetComponent<ParkourObject>();
        AiCarNodes aiCarNodes = pooledObject.GetComponent<AiCarNodes>();

        float randomPositionX = GetRandomPositionX();
        aiCarNodes.SetAllTransformPositionX(randomPositionX);
        parkourComponent.Deform(randomPositionX);
        _instantiatedGameObjects.Add(parkourComponent);

        if (!spawnCar) return;
        int totalMembersInEnum = Enum.GetValues(typeof(AiDirectionType)).Length;
        AiDirectionType randomAiDirectionType = (AiDirectionType) Random.Range(0, totalMembersInEnum);
        aiCarBehaviourPrefab[Random.Range(0, aiCarBehaviourPrefab.Count)].ReuseAndInitialize(_instantiatedGameObjects, parkourComponent, aiCarNodes, randomAiDirectionType);
    }

    private void Update()
    {
        if (IsRelease(_instantiatedGameObjects[0]))
        {
            _instantiatedGameObjects[0].gameObject.Release();
            _instantiatedGameObjects.RemoveAt(0);
            SpawnSingle(IsSpawn);
        }
    }
}
