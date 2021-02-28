using Assets._ExtendedPrinter.Scripts.SlicingService;
using TMPro;
using UnityEngine;

public class PrePrintInformationSlateController : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro UsedFialemnt;

    [SerializeField]
    private TextMeshPro Time;

    [SerializeField]
    private TextMeshPro ModelName;


    private string selectedFile;
    public OctoprintController OctoprintController;

    public void SetFile(string file)
    {
        selectedFile = file;
    }
   
    public void SetupInformationByFileName(FileSlicedMessageArgs info)
    {
        Time.text = info.PrintTime;
        UsedFialemnt.text = info.FilamentLength;
    }
}
