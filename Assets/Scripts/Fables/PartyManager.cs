using UnityEngine;
using System.Collections.Generic;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance { get; private set; }

    [SerializeField] PartyScreen partyScreen;
    List<Fables> fables;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        fables = new List<Fables>();
        // Initialize with some fables for demonstration
        // fables.Add(...);
        // fables.Add(...);
        UpdatePartyScreen();
    }

    public void AddFable(Fables fable)
    {
        fables.Add(fable);
        UpdatePartyScreen();
    }

    public void RemoveFable(Fables fable)
    {
        fables.Remove(fable);
        UpdatePartyScreen();
    }

    public void UpdatePartyScreen()
    {
        if (partyScreen != null)
        {
            partyScreen.SetPartyData(fables);
        }
    }
}
