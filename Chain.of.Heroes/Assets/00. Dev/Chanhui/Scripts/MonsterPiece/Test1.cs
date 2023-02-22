using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : Pieces
{
    
    // Pawn
    public override List<Vector2Int> GetAvailableMoves(ref Pieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        // One in front
        if (board[currentX, currentY + direction] == null)
        {
            r.Add(new Vector2Int(currentX, currentY + direction));
        }

        // Kill move
        if (currentX != tileCountX - 1)
        {
            if (board[currentX, currentY + direction] != null && board[currentX, currentY + direction].team != team)
            {
                r.Add(new Vector2Int(currentX, currentY + direction));
            }
        }
        
        if(currentX != 0) 
        {
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
            {
                r.Add(new Vector2Int(currentX - 1, currentY + direction));
            }
        }

        return r;
    }

}


