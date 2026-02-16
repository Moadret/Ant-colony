using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvolveSelectUI : MonoBehaviour
{
    public Button[] buttons;
    public TMP_Text[] buttonTexts;
    public UpgradeDatabase upgradeDatabase;

    private ResourceSource currentSource;
    

    List<Upgrades> currentOffers;

    private void Awake()
    {
        gameObject.SetActive(false); // hidden by default
    }

    public void ShowSelectUI(ResourceSource source)
    {
        currentSource = source;
        gameObject.SetActive(true);
        SetButtons();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }


    private void SetButtons()
    {
        currentOffers = upgradeDatabase.GetRandomUpgrades(3);

        int count = Mathf.Min(buttons.Length, currentOffers.Count);

        // First, disable all buttons (important!)
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }

        // Then, configure only valid ones
        for (int i = 0; i < count; i++)
        {
            int index = i; // closure safety

            buttons[i].gameObject.SetActive(true);
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(
                () => EvolveUpgrade(currentOffers[index].id)
            );

            buttonTexts[i].text = currentOffers[i].displayName;
        }
    }

    public void EvolveUpgrade(string evolveType)
    {
        Debug.Log($"Evolving {currentSource.name} with {evolveType}");

        currentSource.evolveName = evolveType;
        Hide();


    }
}
