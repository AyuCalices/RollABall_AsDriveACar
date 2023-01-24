using System;
using System.Collections.Generic;
using UnityEngine;

public class AiCarNodes : MonoBehaviour
{
    public List<AiCarNodesContainer> rightAiNodes;
    public List<AiCarNodesContainer> leftAiNodes;
    
    private GameObject _car;

    private void Awake()
    {
        foreach (AiCarNodesContainer aiCarNodesContainer in rightAiNodes)
        {
            aiCarNodesContainer.BaseLocalPosition = aiCarNodesContainer.CurrentPosition;
        }
        
        foreach (AiCarNodesContainer aiCarNodesContainer in leftAiNodes)
        {
            aiCarNodesContainer.BaseLocalPosition = aiCarNodesContainer.CurrentPosition;
        }
    }

    public Vector3 GetOriginPosition(AiDirectionType aiDirectionType)
    {
        if (aiDirectionType == AiDirectionType.Right)
        {
            return rightAiNodes.Find(x => x.isOrigin).CurrentPosition;
        }
        
        return leftAiNodes.Find(x => x.isOrigin).CurrentPosition;
    }
    
    public Vector3 GetTargetPosition(AiDirectionType aiDirectionType)
    {
        if (aiDirectionType == AiDirectionType.Right)
        {
            return rightAiNodes.Find(x => !x.isOrigin).CurrentPosition;
        }
        
        return leftAiNodes.Find(x => !x.isOrigin).CurrentPosition;
    }
    
    public void SetAllTransformPositionX(float positionXAddition)
    {
        List<AiCarNodesContainer> combinedTransforms = new List<AiCarNodesContainer>();
        combinedTransforms.AddRange(rightAiNodes);
        combinedTransforms.AddRange(leftAiNodes);
        Vector3 addition = new Vector3(positionXAddition, 0, 0);
        
        foreach (AiCarNodesContainer combinedTransform in combinedTransforms)
        {
            if (combinedTransform.isOrigin) continue;
            
            combinedTransform.transform.localPosition = combinedTransform.BaseLocalPosition + addition;
        }
    }
}

[Serializable]
public class AiCarNodesContainer
{
    public Transform transform;
    public bool isOrigin;

    public Vector3 CurrentPosition => transform.position;
    public Vector3 BaseLocalPosition { get; set; }
}
