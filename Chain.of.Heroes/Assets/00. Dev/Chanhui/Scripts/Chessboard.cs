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
    [SerializeField] private Material HighilghMaterial;
    [SerializeField] private Material AttackMaterial;
    //--------------------------------------
    [SerializeField] private float deathSize = 0.7f;
    [SerializeField] private float deathSpacing = 0.3f;
    //[SerializeField] private float dragOffset = 0.75f;
    //---------------------------------------
    [Header("UI")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject AttackScreen;


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
    private Pieces[,] monsters;
    //---------------------------------------------
    private Pieces currentDragging;
    [SerializeField] private List<Pieces> deadPlayer = new List<Pieces>();
    [SerializeField] private List<Pieces> deadMonster = new List<Pieces>();
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    //---------------------------------------------

    private Camera currentCamera;

    private Vector2Int currentHover;
    private Vector3 bounds;
    private bool isPlayerTurn;
    private bool isUIClick;

    //-----------------------------
    private Vector2Int PresentPosition;

    private void Awake()
    {
        isPlayerTurn = true;
        isUIClick = false;

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
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight", "Attack")))
        {
            // Get the indexs of the tile i've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                RenderObject(hitPosition.x, hitPosition.y, ToachtileMaterial);
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                if(ContainsValidMove(ref availableMoves, currentHover))
                {
                    if(monsters[currentHover.x, currentHover.y] != null)
                    {
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Attack");
                        RenderObject(currentHover.x, currentHover.y, AttackMaterial);
                    }
                    else
                    {
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Highlight");
                        RenderObject(currentHover.x, currentHover.y, HighilghMaterial);
                    }
                }
                else
                {
                    tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    RenderObject(currentHover.x, currentHover.y, tileMaterial);
                }

                currentHover = hitPosition;

                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                RenderObject(hitPosition.x, hitPosition.y, ToachtileMaterial);
            }

            // -----------------------------------------------------
            // 마우스로 피스들을 움직이는 코드 = 이부분은 사용할 일이 없을 것 같다. 보류!
            // If we press down on the mouse
            if (Input.GetMouseButtonDown(0))
            {
                if (monsters[hitPosition.x, hitPosition.y] != null)
                {
                    // Is it our turn?
                    if((monsters[hitPosition.x, hitPosition.y].team == 0 && isPlayerTurn))
                    {
                        AttackState(0);
                        PresentPosition = hitPosition;
                    }
                    else if(monsters[hitPosition.x, hitPosition.y].team == 1 && !isPlayerTurn)
                    {
                        currentDragging = monsters[hitPosition.x, hitPosition.y];

                        //Get a list of where I can go, hightlight tiles as well
                        availableMoves = currentDragging.GetAvailableMoves(ref monsters, tile_X, tile_Y);
                        isUIClick = true;
                        HighlightTiles();
                    }
                }
            }
            

            // If we are releasing the mouse button
            if (currentDragging != null && isUIClick && Input.GetMouseButtonUp(0))
            {
                Vector2Int previousPosition = new Vector2Int(currentDragging.currentX, currentDragging.currentY);
                bool validMove = MoveTo(currentDragging, hitPosition.x, hitPosition.y);
                
                if (!validMove)
                {
                    currentDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));
                } 
                currentDragging = null;
                // 이동시 타일 색 변경
                ReMoveHighlightTiles();

                isUIClick = !isUIClick;
                AttackScreen.transform.GetChild(0).gameObject.SetActive(false);
                AttackScreen.SetActive(false);
            }

            //-----------------------------------------------------------
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                if (ContainsValidMove(ref availableMoves, currentHover))
                {
                    if (monsters[currentHover.x, currentHover.y] != null)
                    {
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Attack");
                        RenderObject(currentHover.x, currentHover.y, AttackMaterial);
                    }
                    else
                    {
                        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Highlight");
                        RenderObject(currentHover.x, currentHover.y, HighilghMaterial);
                    }
                }
                else
                {
                    tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    RenderObject(currentHover.x, currentHover.y, tileMaterial);
                }

                currentHover = -Vector2Int.one;
            }
        }  
    }

    // {Generate the board} = all tile make 
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
        vertices[0] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[1] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 0, 2, 3 };
        Vector2[] uvs = new Vector2[] { new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f) };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.uv = uvs;

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>().size = new Vector3(tilesize, 0.1f, tilesize);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return tileObject;
    }

    // {Operations}
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
    // 죽였을때 피스틀을 이동시키는 코드 = 이 코드는 데이터 낭비를 줄이기 위해 사용 가능!!
    private bool MoveTo(Pieces cp, int x, int y)
    {
        if(!ContainsValidMove(ref availableMoves, new Vector2(x,y)))
        {
            return false;
        }
        int monsterX = x;
        int monsterY = y;

        //int monsterfront = (monsters[x, y] != null) ? y - 1 : y;

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        //Is there another piece on the target position?
        if (monsters[x,y] != null)
        {
            Pieces ocp = monsters[x,y];

            if(cp.team == ocp.team)
            {
                return false;
            }

            //If its the enemy team
            if(ocp.team == 0)
            {
                /*
                if (deadPlayer.Count > 0)
                {
                    CheckMate(1);
                }*/
                CheckMate(1);

                deadPlayer.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3(8 * tilesize, yOffset, -1 * tilesize)
                    - bounds + new Vector3(tilesize / 2, 0, tilesize / 2)
                    + (Vector3.forward * deathSpacing) * deadPlayer.Count);
            }
            else
            {
                if (deadMonster.Count == monsterData.MONSTER_NUM - 1)
                {
                    CheckMate(0);
                }

                deadMonster.Add(ocp);
                monsters[ocp.currentX, ocp.currentY] = null;
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3(-1 * tilesize, yOffset, 8 * tilesize)
                    - bounds + new Vector3(tilesize / 2, 0, tilesize / 2)
                    + (Vector3.back * deathSpacing) * deadMonster.Count);
            }

            if(cp.currentX == ocp.currentX)
            {
                if (cp.currentY < ocp.currentY)
                    monsterY = y - 1;
                else
                    monsterY = y + 1;
            }
            else if(cp.currentY == ocp.currentY)
            {
                if(cp.currentX < ocp.currentX)
                    monsterX = x - 1;
                else
                    monsterX = x + 1;
            }
            else if(cp.currentX < ocp.currentX)
            {
                if(cp.currentY < ocp.currentY)
                {
                    monsterX = x - 1;
                    monsterY = y - 1;
                }
                else
                {
                    monsterX = x - 1;
                    monsterY = y + 1;
                }
            }
            else if (cp.currentX > ocp.currentX)
            {
                if (cp.currentY < ocp.currentY)
                {
                    monsterX = x + 1;
                    monsterY = y - 1;
                }
                else
                {
                    monsterX = x + 1;
                    monsterY = y + 1;
                }
            }
        }

        if (monsters[monsterX, monsterY] != monsters[previousPosition.x, previousPosition.y])
        {
            monsters[monsterX, monsterY] = cp;
            monsters[previousPosition.x, previousPosition.y] = null;
            PositionSingleMonster(monsterX, monsterY);
        }
        
        isPlayerTurn = !isPlayerTurn;

        return true;
    }
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for(int i = 0; i < moves.Count; i++)
        {
            if(moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }
    //-------------------------------------------------------------

    // {Spawning of the pieces}
    private void SpawnAllMonster()
    {
        monsters = new Pieces[tile_X, tile_Y];

        for (int i = 0; i < monsterData.MONSTER_NUM; i++)
        {
            if (monsters[(int)monsterData.CurrentXY[i].x, (int)monsterData.CurrentXY[i].y] == null)
            {
                monsters[(int)monsterData.CurrentXY[i].x, (int)monsterData.CurrentXY[i].y] = SpawnSingleMonster(i, 1);
            }
        }
        monsters[4, 0] = SpawnSinglePlayer(0);
    }
    private Pieces SpawnSingleMonster(int i, int team)
    {
        Pieces cp = Instantiate(monsterData.Monster_pf[(int)monsterData.Type[i]], transform).GetComponent<Pieces>();
        
        cp.team = team;

        return cp;
    }
    private Pieces SpawnSinglePlayer(int team)
    {
        Pieces player = Instantiate(Player, transform).GetComponent<Pieces>();
        player.team = team;
        return player;
    }

    // {Positioning}
    private void PositionAllMonster()
    {
        for (int i = 0; i < monsterData.MONSTER_NUM; i++)
        {
            if (monsters[(int)monsterData.CurrentXY[i].x, (int)monsterData.CurrentXY[i].y] != null)
            {
                PositionSingleMonster((int)monsterData.CurrentXY[i].x, (int)monsterData.CurrentXY[i].y, true);
            }
        }
        PositionSingleMonster(4, 0, true);
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
    
    // {Highlight Tiles}
    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            if (monsters[availableMoves[i].x, availableMoves[i].y] != null)
            {
                tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Attack");
                RenderObject(availableMoves[i].x, availableMoves[i].y, AttackMaterial);
            }
            else
            {
                tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
                RenderObject(availableMoves[i].x, availableMoves[i].y, HighilghMaterial);
            }
        }
    }
    private void ReMoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
            RenderObject(availableMoves[i].x, availableMoves[i].y, tileMaterial);
        }
        availableMoves.Clear();
    }

    // {Checkmate}
    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }
    private void DisplayVictory(int winningTeam)
    {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    public void OnResetButton()
    {
        // UI
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);

        // Fields reset
        currentDragging = null;
        availableMoves = new List<Vector2Int>();

        // Clean up
        for(int x = 0; x < tile_X; x++)
        {
            for(int y = 0; y < tile_Y; y++)
            {
                if(monsters[x, y] != null)
                {
                    Destroy(monsters[x, y].gameObject);
                }

                monsters[x, y] = null;
            }
        }

        for(int i = 0; i < deadPlayer.Count; i++)
        {
            Destroy(deadPlayer[i].gameObject);
        }
        for (int i = 0; i < deadMonster.Count; i++)
        {
            Destroy(deadMonster[i].gameObject);
        }

        deadPlayer.Clear();
        deadMonster.Clear();

        SpawnAllMonster();
        PositionAllMonster();
        isPlayerTurn = true;

    }
    public void OnExitButton()
    {
        Application.Quit();
    }

    // {타일의 Material을 결정하는 함수}
    private void RenderObject(int x, int y, Material mal)
    {
        tiles[x, y].GetComponent<MeshRenderer>().material = new Material(mal);
    }

    // Player Attack State
    private void AttackState(int team)
    {
        AttackScreen.SetActive(true);
        AttackScreen.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void OnMoveButton1()
    {
        isUIClick = true;
        currentDragging = monsters[PresentPosition.x, PresentPosition.y];

        //Get a list of where I can go, hightlight tiles as well
        availableMoves = currentDragging.GetAvailableMoves(ref monsters, tile_X, tile_Y);

        HighlightTiles();
    }

    public void OnMoveButton2()
    {
        isUIClick = true;
        currentDragging = monsters[PresentPosition.x, PresentPosition.y];

        //Get a list of where I can go, hightlight tiles as well
        availableMoves = currentDragging.GetAvailableAttacks(ref monsters, tile_X, tile_Y);

        HighlightTiles();
    }
}