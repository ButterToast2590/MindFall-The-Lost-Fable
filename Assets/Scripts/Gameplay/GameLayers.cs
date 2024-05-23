using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask Obstacles;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask triggerLayer;

    public static GameLayers i { get; set; }
    private void Awake()
    {
        i = this;
    }
    public LayerMask ObstaclesLayer { get => Obstacles; }
    public LayerMask InteractableLayer { get => interactableLayer; }
    public LayerMask GrassLayer { get => grassLayer; }
    public LayerMask PlayerLayer { get => playerLayer; }
    public LayerMask FovLayer { get => fovLayer; }
    public LayerMask TriggerLayers{ get => grassLayer | fovLayer | triggerLayer; }
}
