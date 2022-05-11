using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [Header("Art Stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize=0.645f;
    [SerializeField] private float yOffSet=0.05f; 
    [SerializeField] private Vector3 boardCenter=Vector3.zero;
    [SerializeField] private float deathSize=0.3f;
    [SerializeField] private float draggOffset=0.5f;


    
    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;



    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();

    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;


    // LOGIC
    private void Awake(){
        GenerateAllTiles(tileSize,TILE_COUNT_X,TILE_COUNT_Y);

        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update() {
        if(!currentCamera){
            currentCamera = Camera.main;
            return;
        }
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray,out info,100,LayerMask.GetMask("Tile","Hover"))){
            //Get the inex of the tile i've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);
            //if we're hovering a tile after not hoevering any tiles
            if(currentHover == -Vector2Int.one){
                currentHover = hitPosition;
                tiles[hitPosition.x,hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            //If we were already hovering a tile, change the previous one
            if(currentHover != hitPosition){
                tiles[currentHover.x,currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x,hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            //if we press down on the mouse
            if(Input.GetMouseButtonDown(0)){
                if(chessPieces[hitPosition.x,hitPosition.y]!=null){
                    //Is it our turn?
                    if(true){
                        currentlyDragging = chessPieces[hitPosition.x,hitPosition.y];
                    }

                }
            }
            //if we releasuing the mouse button

            if(currentlyDragging != null &&  Input.GetMouseButtonUp(0)){
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX,currentlyDragging.currentY);
                bool validMove = MoveTo(currentlyDragging,hitPosition.x, hitPosition.y);

                if(!validMove){
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x,previousPosition.y));
                }
                currentlyDragging = null;
            
            }

        }else{
                if(currentHover != -Vector2Int.one){
                    tiles[currentHover.x,currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    currentHover = -Vector2Int.one;
                }
                if(currentlyDragging && Input.GetMouseButtonUp(0)){
                    currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX,currentlyDragging.currentY));
                    currentlyDragging=null;
                }  
            }

        //If we're dragging a piece
        if(currentlyDragging){
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up*yOffSet);
            float distance = 0.0f;
            if(horizontalPlane.Raycast(ray,out distance)){
                currentlyDragging.SetPosition(ray.GetPoint(distance)+Vector3.up*draggOffset); 
            }
        }

    }

    


    //Generate the Board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY){
        yOffSet = transform.position.y;
        bounds = new Vector3((tileCountX/2)*tileSize,0,(tileCountY/2)*tileSize);

        tiles = new GameObject[tileCountX,tileCountY];
        for (int x = 0; x < tileCountX ; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x,y] = GenerateSingleTile(tileSize,x,y);
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}",x,y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = this.tileMaterial;

        Vector3[] verices = new Vector3[4];
        verices[0] = new Vector3(x*tileSize,yOffSet,y*tileSize) -bounds;
        verices[1] = new Vector3(x*tileSize,yOffSet,(y+1)*tileSize) -bounds;
        verices[2] = new Vector3((x+1)*tileSize,yOffSet,y*tileSize) -bounds;
        verices[3] = new Vector3((x+1)*tileSize,yOffSet,(y+1)*tileSize) -bounds;

        int[] tris = new int[] {0,1,2,1,3,2};

        mesh.vertices = verices;
        mesh.triangles =tris;

        mesh.RecalculateNormals();
        tileObject.layer = LayerMask.NameToLayer("Tile");

        tileObject.AddComponent<BoxCollider>();


        return tileObject;
    }


//Spawning of the pieces
private void SpawnAllPieces(){
    chessPieces = new ChessPiece[TILE_COUNT_X,TILE_COUNT_Y];

    //White team
    chessPieces[0,0] = SpawnSinglePieces(ChessPieceType.Rook,ChessTeamType.White);
    chessPieces[1,0] = SpawnSinglePieces(ChessPieceType.Knight,ChessTeamType.White);
    chessPieces[2,0] = SpawnSinglePieces(ChessPieceType.Bishop,ChessTeamType.White);
    chessPieces[3,0] = SpawnSinglePieces(ChessPieceType.King,ChessTeamType.White);
    chessPieces[4,0] = SpawnSinglePieces(ChessPieceType.Queen,ChessTeamType.White);
    chessPieces[5,0] = SpawnSinglePieces(ChessPieceType.Bishop,ChessTeamType.White);
    chessPieces[6,0] = SpawnSinglePieces(ChessPieceType.Knight,ChessTeamType.White);
    chessPieces[7,0] = SpawnSinglePieces(ChessPieceType.Rook,ChessTeamType.White);
    
    for(int i=0; i<TILE_COUNT_X; i++){
        chessPieces[i,1] = SpawnSinglePieces(ChessPieceType.Pawn,ChessTeamType.White);
        chessPieces[i,6] = SpawnSinglePieces(ChessPieceType.Pawn,ChessTeamType.Black);
    }
    // Black team
    chessPieces[0,7] = SpawnSinglePieces(ChessPieceType.Rook,ChessTeamType.Black);
    chessPieces[1,7] = SpawnSinglePieces(ChessPieceType.Knight,ChessTeamType.Black);
    chessPieces[2,7] = SpawnSinglePieces(ChessPieceType.Bishop,ChessTeamType.Black);
    chessPieces[3,7] = SpawnSinglePieces(ChessPieceType.King,ChessTeamType.Black);
    chessPieces[4,7] = SpawnSinglePieces(ChessPieceType.Queen,ChessTeamType.Black);
    chessPieces[5,7] = SpawnSinglePieces(ChessPieceType.Bishop,ChessTeamType.Black);
    chessPieces[6,7] = SpawnSinglePieces(ChessPieceType.Knight,ChessTeamType.Black);
    chessPieces[7,7] = SpawnSinglePieces(ChessPieceType.Rook,ChessTeamType.Black);

     
}
private ChessPiece SpawnSinglePieces(ChessPieceType type, ChessTeamType team){
    ChessPiece cp = Instantiate(prefabs[((int)type)-1], transform).GetComponent<ChessPiece>();
    if(cp != null){
        cp.type = type;
        cp.team = (int)team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[(int)team];
        cp.transform.Rotate(Vector3.forward, (int)cp.team ==0? -90:90);
    }else
        throw new Exception("Null Piece Error!");
    return cp;
}

//Positioning
private void PositionAllPieces(){
    for (int x = 0; x < TILE_COUNT_X; x++)
        for (int y = 0; y < TILE_COUNT_Y; y++)
            if(chessPieces[x,y]!= null)
                PositionSinglePieces(x,y,true);
        
}
private void PositionSinglePieces(int x, int y, bool force = false){
    ChessPiece current = chessPieces[x,y];
    current.currentX = x;
    current.currentY = y;
    current.SetPosition(GetTileCenter(x,y),force);

}
private Vector3 GetTileCenter(int x, int y){
    return  new Vector3(x*tileSize, yOffSet, y*tileSize)-bounds+ new Vector3(tileSize/2,0,tileSize/2);
}
//Operation
    private Vector2Int LookupTileIndex(GameObject hitInfo){
        
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if(tiles[x,y] == hitInfo)
                    return new Vector2Int(x,y);
        return -Vector2Int.one;
    }
    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        Vector2Int previousPosition = new Vector2Int(cp.currentX,cp.currentY);
        //Is there another piece on the target Position?
        if(chessPieces[x,y] != null){
            ChessPiece otherCp = chessPieces[x,y];
            if(cp.team == otherCp.team){
                return false;
            }
            //if its the enemy team
            if(otherCp.team == (int)ChessTeamType.White){
                deadWhites.Add(otherCp);
                otherCp.SetPosition(new Vector3(-1*tileSize, yOffSet, TILE_COUNT_Y* tileSize)
                -bounds
                +new Vector3(tileSize/2,0,tileSize/2)
                + (Vector3.back*deathSize)* deadWhites.Count
                );
            } else{
                deadBlacks.Add(otherCp);
                otherCp.SetPosition(new Vector3(TILE_COUNT_X*tileSize, yOffSet, -1* tileSize)
                -bounds
                +new Vector3(tileSize/2,0,tileSize/2)
                - (Vector3.back*deathSize)* deadBlacks.Count
                );
            }
            otherCp.SetScale(deathSize);

        }
        chessPieces[x,y] = cp;
        chessPieces[previousPosition.x,previousPosition.y] = null;
        PositionSinglePieces(x,y);
        return true;

    }
}

