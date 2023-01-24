using System;
using System.Collections.Generic;
using ToolBox.Pools;
using UnityEngine;

public class AiCarBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    
    private AiDirectionType _aiDirectionType;
    private List<ParkourObject> _instantiatedGameObjects;
    private ParkourObject _currentParkourObject;
    
    private static readonly List<AiCarBehaviour> _pooledCars = new List<AiCarBehaviour>();

    public static void ReleaseAllCars()
    {
        for (int index = _pooledCars.Count - 1; index >= 0; index--)
        {
            AiCarBehaviour aiCarBehaviour = _pooledCars[index];
            aiCarBehaviour.Release();
        }

        _pooledCars.Clear();
    }

    private void Release()
    {
        _pooledCars.Remove(this);
        gameObject.Release();
        _instantiatedGameObjects = null;
        _currentParkourObject = null;
        LeanTween.cancel(gameObject);
    }

    public void ReuseAndInitialize(List<ParkourObject> instantiatedGameObjects, ParkourObject currentParkourObject, AiCarNodes currentNode, AiDirectionType aiDirectionType)
    {
        Vector3 startingNodePosition;
        Vector3 nextNode;
        if (aiDirectionType == AiDirectionType.Right)
        {
            startingNodePosition = currentNode.GetOriginPosition(aiDirectionType);
            nextNode = currentNode.GetTargetPosition(aiDirectionType);
        }
        else
        {
            startingNodePosition = currentNode.GetTargetPosition(aiDirectionType);
            nextNode = currentNode.GetOriginPosition(aiDirectionType);
        }
        
        AiCarBehaviour movedObject = gameObject.Reuse<AiCarBehaviour>(startingNodePosition, Quaternion.LookRotation(nextNode - startingNodePosition));
        movedObject._instantiatedGameObjects = instantiatedGameObjects;
        movedObject._aiDirectionType = aiDirectionType;
        movedObject._currentParkourObject = currentParkourObject;

        float movementTime = Vector3.Distance(movedObject.transform.position, nextNode) / movedObject.speed;
        LeanTween.move(movedObject.gameObject, nextNode, movementTime).setOnComplete(() => TryMoveToNext(movedObject));
        _pooledCars.Add(movedObject);
    }

    private static void TryMoveToNext(AiCarBehaviour movedObject)
    {
        if (TryGetNextNode(movedObject, out Vector3 nextNodePosition, ref movedObject._currentParkourObject))
        {
            movedObject.transform.rotation = Quaternion.LookRotation(nextNodePosition - movedObject.transform.position);
            float movementTime = Vector3.Distance(movedObject.transform.position, nextNodePosition) / movedObject.speed;
            LeanTween.move(movedObject.gameObject, nextNodePosition, movementTime).setOnComplete(() => TryMoveToNext(movedObject));
        }
        else
        {
            movedObject.Release();
        }
    }

    private static bool TryGetNextNode(AiCarBehaviour movedObject, out Vector3 nextNodePosition, ref ParkourObject nextNode)
    {
        nextNodePosition = Vector3.zero;
        
        int indexOfCurrentNode = movedObject._instantiatedGameObjects.IndexOf(movedObject._currentParkourObject);
        if (movedObject._aiDirectionType == AiDirectionType.Right)
        {
            if (indexOfCurrentNode + 1 < 0 || indexOfCurrentNode + 1 >= movedObject._instantiatedGameObjects.Count) return false;

            nextNode = movedObject._instantiatedGameObjects[indexOfCurrentNode + 1];
            nextNodePosition = movedObject._currentParkourObject.GetComponent<AiCarNodes>().GetTargetPosition(movedObject._aiDirectionType);
            return true;
        }
        
        if (indexOfCurrentNode - 1 < 0 || indexOfCurrentNode - 1 >= movedObject._instantiatedGameObjects.Count) return false;

        nextNode = movedObject._instantiatedGameObjects[indexOfCurrentNode - 1];
        nextNodePosition = movedObject._currentParkourObject.GetComponent<AiCarNodes>().GetOriginPosition(movedObject._aiDirectionType);
        return true;
    }
}
