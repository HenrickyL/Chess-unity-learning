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



    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;



    // LOGIC
    private void Awake(){
        GenerateAllTiles(tileSize,TILE_COUNT_X,TILE_COUNT_Y);
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
        }else{
                if(currentHover != -Vector2Int.one){
                    tiles[currentHover.x,currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    currentHover = -Vector2Int.one;
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

//Operation
    private Vector2Int LookupTileIndex(GameObject hitInfo){
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if(tiles[x,y] == hitInfo)
                    return new Vector2Int(x,y);
        return -Vector2Int.one;
    }
}

