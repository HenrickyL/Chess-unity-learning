using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType{
     None = 0,
     Pawn = 1,
     Rook = 2,
     Knight = 3,
     Bishop = 4,
     Queen =5,
     King = 6
}

public enum ChessTeamType{
    White,Black
}

public class ChessPiece : MonoBehaviour
{

    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;
    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one*32;

    

    private void Update() {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime*10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime*10);
    }
   
   public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board,int tileCountX, int tileCountY){
       List<Vector2Int> r = new List<Vector2Int>();
        r.Add(new Vector2Int(currentX,currentY+1));
        r.Add(new Vector2Int(currentX,currentY-1));
        r.Add(new Vector2Int(currentX+1,currentY));
        r.Add(new Vector2Int(currentX-1,currentY));

       return r;
   }
    public virtual void SetPosition(Vector3 position, bool force =false){
        desiredPosition = position;

        if(force)
            transform.position = desiredPosition;
    }

    public virtual void SetScale(float scale, bool force =false){
        desiredScale = desiredScale*scale;

        if(force)
            transform.localScale = desiredScale;
    }



}
