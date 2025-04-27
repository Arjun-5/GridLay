using System.Collections.Generic;
using UnityEngine;

public abstract class GridPostProcessSO : ScriptableObject
{
    public abstract void OnGridInstantiationCompleted(List<GameObject> tiles, Transform instantiateTransform);
}
