using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GridLay/Procedural/Random Tray Spawner")]
public class RandomTraySpawner : GridPostProcessSO
{
    [SerializeField] private List<GameObject> trayPrefabs;

    [SerializeField] private int trayInstantiationCount = 7;

    public override void OnGridInstantiationCompleted(List<GameObject> tiles, Transform instantiateTransform)
    {
        if (trayPrefabs == null || trayPrefabs.Count == 0 || tiles == null || tiles.Count == 0)
        {
            return;
        }

        var shuffled = new List<GameObject>(tiles);

        shuffled.Sort((a, b) => Random.value.CompareTo(Random.value));


        for (int i = 0; i < Mathf.Min(trayInstantiationCount, shuffled.Count); i++)
        {
            GameObject randomObstacle = trayPrefabs[Random.Range(0, trayPrefabs.Count)];

            GameObject tile = shuffled[i];

            Instantiate(randomObstacle, tile.transform.position, randomObstacle.transform.rotation, instantiateTransform);
        }
    }
}
