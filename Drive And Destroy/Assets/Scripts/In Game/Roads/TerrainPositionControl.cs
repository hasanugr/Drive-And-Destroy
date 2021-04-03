using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPositionControl : MonoBehaviour
{
    public enum TerrainTriggerLocation
    {
        Top,
        Bottom,
        Left,
        Right
    }
    public TerrainTriggerLocation LimitPosition;

    [SerializeField]
    private InGameManager Igm;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("PlayerShip"))
        {
            switch (LimitPosition)
            {
                case TerrainTriggerLocation.Top:
                    Igm.ChangeTerrainPosition("Vertical");
                    break;
                case TerrainTriggerLocation.Bottom:
                    Igm.ChangeTerrainPosition("Vertical");
                    break;
                case TerrainTriggerLocation.Left:
                    Igm.ChangeTerrainPosition("Horizontal");
                    break;
                case TerrainTriggerLocation.Right:
                    Igm.ChangeTerrainPosition("Horizontal");
                    break;
            }
        }
    }
}
