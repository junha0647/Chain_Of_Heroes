using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private Material ToachtileMaterial;

    [Header("Prefabs & Materials")]
    [SerializeField] private MapData monsterData;
    [SerializeField] private GameObject Player;

    [Header("Chess board XY")]
    [SerializeField] private float tilesize = 0;
    [SerializeField] private int tile_X = 0;
    [SerializeField] private int tile_Y = 0;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Chess boards arr")]
    private GameObject[,] tiles;
    private Monster[,] monsters;
    private Monster currentDragging;

    private Camera currentCamera;

    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake()
    {
        GenerateAllTiles(tilesize, tile_X, tile_Y);
    }

    private void Start()
    {
        monsterData = MapManager.Instance.monData[MapManager.Instance.stageNum];

        SpawnAllMonster();
        PositionAllMonster();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            // Get the indexs of the tile i've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = new Material(ToachtileMaterial);
            }

            // If we were already hoevering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = new Material(tileMaterial);

                currentHover = hitPosition;

                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = new Material(ToachtileMaterial);

            }

            // -----------------------------------------------------
            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0))
            {
                if (monsters[hitPosition.x, hitPosition.y] != null)
                {
                    // Is it our turn?
                    if(true)
                    {
                        currentDragging = monsters[hitPosition.x, hitPosition.y];
                    }
                }
            }
            // If we are releasing the mouse button
            if (currentDragging != null && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentDragging.currentX, currentDragging.currentY);

                bool validMove = MoveTo(currentDragging, hitPosition.x, hitPosition.y);
                if (!validMove)
                {
                    currentDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                    currentDragging = null;
                }
                else
                {
                    currentDragging = null;
                }
            }

            //-----------------------------------------------------------
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = new Material(tileMaterial);
                currentHover = -Vector2Int.one;

            }

            if(currentDragging && Input.GetMouseButtonUp(0))
            {
                currentDragging.SetPosition(GetTileCenter(currentDragging.currentX, currentDragging.currentY));
                currentDragging = null;
            }
        }
    }

    //  all tile make (Generate the board)
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }
    // single tile Make and Mesh/Material make
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>().size = new Vector3(tilesize, 0.1f, tilesize);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return tileObject;
    }

    // (Operations)
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < tile_X; x++)
        {
            for (int y = 0; y < tile_Y; y++)
            {
                if (tiles[x, y] == hitInfo)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return -Vector2Int.one; // Invalid
    }

    //--------------------------------------------------------------
    private bool MoveTo(Monster cp, int x, int y)
    {
        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        //Is there another piece on the target position?
        if (monsters[x,y] != null)
        {
            Monster ocp = monsters[x,y];

            if(cp.team == ocp.team)
            {
                return false;
            }
        }

        monsters[x, y] = cp;
        monsters[previousPosition.x, previousPosition.y] = null;

        PositionSingleMonster(x, y);
        return true;
    }
    //-------------------------------------------------------------
    // Spawning of the pieces
    private void SpawnAllMonster()
    {
        monsters = new Monster[tile_X, tile_Y];

        for (int i = 0; i < monsterData.MONSTER_NUM; i++)
        {
            if (monsters[(int)monsterData.CurrentXY[i].x, (int)monsterData.CurrentXY[i].y] == null)
            {
                monsters[(int)monsterData.CurrentXY[i].x, (int)monsterData.CurrentXY[i].y] = SpawnSingleMonster(i, 1);
            }
        }
        monsters[7, 4] = SpawnSinglePlayer(0);
    }
    private Monster SpawnSingleMonster(int i, int team)
    {
        Monster cp = Instantiate(monsterData.Monster_pf[(int)monsterData.Type[i]], transform).GetComponent<Monster>();
        cp.team = team;
        return cp;
    }
    private Monster SpawnSinglePlayer(int team)
    {
        Monster player = Instantiate(Player, transform).GetComponent<Monster>();
        player.team = team;
        return player;
    }
    //Positioning
    private void PositionAllMonster()
    {
        for (int i = 0; i < monsterData.MONSTER_NUM; i++)
        {
            if (monsters[(int)monsterData.CurrentXY[i].x, (int)monsterData.CurrentXY[i].y] != null)
            {
                PositionSingleMonster((int)monsterData.CurrentXY[i].x, (int)monsterData.CurrentXY[i].y, true);
            }
        }
        PositionSingleMonster(7, 4, true);
    }

    private void PositionSingleMonster(int x, int y, bool force = false)
    {
        monsters[x, y].currentX = x;
        monsters[x, y].currentY = y;
        monsters[x, y].SetPosition(GetTileCenter(x, y), force);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tilesize, yOffset, y * tilesize) - bounds + new Vector3(tilesize / 2, 0, tilesize / 2);
    }
}