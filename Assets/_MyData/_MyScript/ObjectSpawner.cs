using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType {SmallGem, BigGem, Enemy}

    public Tilemap tilesMap;
    public GameObject[] objectPrefabs;
    public float bigGemProbibility = 0.2f;
    public float enemyProbibility = 0.1f;
    public int maxObjects = 5;
    public float gemLifeTime = 10f;
    public float spawnInterval = 0.5f;

    List<Vector3> validSpawnPositions = new List<Vector3>();
    List<GameObject> spawnObjects = new List<GameObject>();
    bool isSpawning = false;

    public bool canDraw = false;


    private void Start()
    {
        this.GatherValidPositions();

        StartCoroutine(SpawnObjectsIfNeeded());

    }

    private void Update()
    {
        if (!tilesMap.gameObject.activeInHierarchy)
        {
            LevelChange();
        }

        if (!isSpawning && ActiveObjectCount() < maxObjects)
        {
            StartCoroutine(SpawnObjectsIfNeeded());
        }
    }

    void LevelChange()
    {
        tilesMap = GameObject.Find("Ground").GetComponent<Tilemap>();
        GatherValidPositions();
        DestroyAllGameObject();
    }

    private int ActiveObjectCount()
    {
        spawnObjects.RemoveAll(item => item == null);
        return spawnObjects.Count;
    }

    IEnumerator SpawnObjectsIfNeeded()
    {
        isSpawning = true;
        while(ActiveObjectCount() < maxObjects)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
        isSpawning &= false;
    }

    private bool PosHasObject(Vector3 positionCheck)
    {
        return spawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionCheck) < 0.1f);
    }

    private ObjectType RamdomObjectType()
    {
        float randomchoice = Random.value;

        if (randomchoice <= enemyProbibility)
        {
            return ObjectType.Enemy;
        }

        else if (randomchoice <= (enemyProbibility + bigGemProbibility))
        {
            return ObjectType.BigGem;
        }

        else
        {
            return ObjectType.SmallGem;
        }
    }

    void SpawnObject()
    {
        if (validSpawnPositions.Count == 0) return;

        Vector3 spawnPos = Vector3.zero;
        bool validPosFound = false;

        while(!validPosFound && validSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPositions.Count);
            Vector3 potentialPos = validSpawnPositions[randomIndex];
            Vector3 leftPos = potentialPos + Vector3.left;
            Vector3 rightPos = potentialPos + Vector3.right;

            if (!this.PosHasObject(leftPos) && !PosHasObject(rightPos))
            {
                spawnPos = potentialPos;
                validPosFound = true;
            }

            validSpawnPositions.RemoveAt(randomIndex);
        }

        if(validPosFound)
        {
            ObjectType objectType = this.RamdomObjectType();
            GameObject gameObject = Instantiate(objectPrefabs[(int)objectType], spawnPos, Quaternion.identity);
            spawnObjects.Add(gameObject);

            if (objectType != ObjectType.Enemy)
            {
                StartCoroutine(this.DestroyGameObjectAfterTime(gameObject, gemLifeTime));
            }
        }
    }

    IEnumerator DestroyGameObjectAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);

        if (gameObject)
        {
            spawnObjects.Remove(gameObject);
            validSpawnPositions.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    void DestroyAllGameObject()
    {
        foreach(GameObject gameObject in spawnObjects)
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
        spawnObjects.Clear();
    }


    void GatherValidPositions()
    {
        validSpawnPositions.Clear();
        BoundsInt boundsInt = tilesMap.cellBounds;
        //Debug.LogWarning(boundsInt.ToString());
        TileBase[] allTiles = tilesMap.GetTilesBlock(boundsInt);
        Vector3 start = tilesMap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));

        for (int x = 0; x < boundsInt.size.x; x++)
        {
            for (int y = 0; y < boundsInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundsInt.size.x];
                if (tile != null)
                {
                    Vector3 place = start + new Vector3(x + 0.5f, y + 2f, 0);
                    //Debug.Log(start + new Vector3(x, y, 0));
                    //Debug.Log("===============");
                    validSpawnPositions.Add(place);
                }
                else
                {
                    //Debug.Log("found nothing");
                }
            }
        }

    }



    private void OnDrawGizmos()
    {
        if (this.tilesMap == null) return;
        if (!this.canDraw) return;
        BoundsInt bounds = this.tilesMap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int vector3Int = new Vector3Int(x, y, 0);
                Vector3 worldPos = tilesMap.CellToWorld(vector3Int);

                Gizmos.DrawWireCube(worldPos + tilesMap.cellSize / 2, tilesMap.cellSize);

            }
        }
    }

}
