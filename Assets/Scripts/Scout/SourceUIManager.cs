using System.Resources;
using UnityEngine;

public class SourceUIManager : MonoBehaviour
{
    public static SourceUIManager Instance { get; private set; }

    [SerializeField] private ResourceInfoUI resourceInfoUI;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowSourceInfo(ResourceSource source)
    {
        resourceInfoUI.Show(source);
    }

    public void HideSourceInfo()
    {
        resourceInfoUI.Hide();
    }
}
