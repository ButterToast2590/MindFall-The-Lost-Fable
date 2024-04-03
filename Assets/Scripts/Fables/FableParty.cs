using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FableParty : MonoBehaviour
{
    [SerializeField] List<Fables> fables;


    private void Start()
    {
        foreach (var fable in fables)
        {
            fable.Init();
        }
    }
    public Fables GetHealthyFable()
    {
        return fables.Where(x => x.HP > 0).FirstOrDefault();
    }
}
