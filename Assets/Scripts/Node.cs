using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
   public int nodeIndex;
   public bool isStarNode;
   public List<Pawn> pawnsOnNode = new List<Pawn>();

   public const int MAX_PLAYERS_PER_NODE =2;

   public delegate void OnPawnLands(Pawn pawn);
   public event OnPawnLands PawnLandsEvent;

   private void Start()
   {
        if (isStarNode)
        {
            Debug.Log($"Node {nodeIndex} is a star node and is safe!");
        }
   }

    public bool CanPawnLand(Pawn pawn)
    {
        if(pawnsOnNode.Contains(pawn)) return true;
        if(isStarNode) return true;
        
        int playerPieceCount = 0;
        foreach (Pawn p in pawnsOnNode)
        {
            if(p.mainPlayer == pawn.mainPlayer)
            {
                playerPieceCount++;
            }
        }

        if(playerPieceCount < MAX_PLAYERS_PER_NODE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddPawn(Pawn pawn)
    {
        if(pawnsOnNode.Contains(pawn)) return;
        
        //checking if the node is full or not
        if(pawnsOnNode.Count < MAX_PLAYERS_PER_NODE)
        {
            pawnsOnNode.Add(pawn);
            PawnLandsEvent?.Invoke(pawn);
        }
        else
        {
            Debug.Log("Node Full");
        }
    }

    public void RemovePawn(Pawn pawn)
    {
        if(pawnsOnNode.Contains(pawn))
        {
            pawnsOnNode.Remove(pawn);
        }
    }

    public void TryEliminatePawn(Pawn pawn)
    {
        if(isStarNode) return;

        foreach(Pawn p in pawnsOnNode)
        {
            if(p.mainPlayer != pawn.mainPlayer)
            {
                p.ResetToHomePosition();
            }
        }
    }
}
