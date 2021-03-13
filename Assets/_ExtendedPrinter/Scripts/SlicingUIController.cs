using Assets._ExtendedPrinter.Scripts.Helper;
using Assets._ExtendedPrinter.Scripts.SlicingService;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SlicingUIController : MonoBehaviour
{
    //[SerializeField]
    //private ButtonConfigHelper sliceButton;

    [SerializeField]
    private GameObject ProfileListItemPrefab;

    [SerializeField]
    private GameObject STLPreview;

    [SerializeField]
    private SlicingServiceConnection slicingServiceConnection;

    [SerializeField]
    private Transform ProfileCollection;

    [SerializeField]
    private TextMeshPro SelectedProfileTextMesh;

    [SerializeField]
    private Interactable ProfileToggleButton;


    [SerializeField]
    private ProgressController ProgressController;

    [SerializeField]
    private STLPreviewController STLPreviewController;

    [SerializeField]
    private GCodeController GCodeController;

    private string _modelFileName;

    public StringEvent ProfileSelected;

    public string SelectedProfile { get; private set; }

    private bool SettingsChanged = true;
    
    public UnityEvent ClosedWithoutSlicing;

    public void OnDisable()
    {
        //var boundsControl = STLPreview.transform.GetChild(0).GetComponent<BoundsControl>();

        //boundsControl.RotateStopped.RemoveListener(OnChange);
        //boundsControl.ScaleStopped.RemoveListener(OnChange);
        //boundsControl.TranslateStopped.RemoveListener(OnChange);
    }

    public void SetupUI(string modelName)
    {
        this._modelFileName = modelName;
        var profiles = slicingServiceConnection.AvailableProfiles;

        if(profiles.Count > 0)
        {
            ClearProfileCollection();

            foreach (var profile in profiles)
            {
                var listItem = Instantiate(ProfileListItemPrefab, ProfileCollection);
                var bch = listItem.GetComponent<ButtonConfigHelper>();
                bch.MainLabelText = profile.Remove(profile.LastIndexOf('.'));
                listItem.GetComponent<Interactable>().OnClick.AddListener(() => ProfileListItemSelected(profile));
            }
            SelectedProfile = SelectedProfile ?? profiles[profiles.Count - 1];
            //SelectedProfile ??= profiles[profiles.Count - 1];
            SelectedProfileTextMesh.text = SelectedProfile;
            ProfileCollection.GetComponent<GridObjectCollection>().UpdateCollection();
        }

        var boundsControl = STLPreview.transform.GetChild(0).GetComponent<BoundsControl>();
        var objectManipulator = STLPreview.transform.GetChild(0).GetComponent<ObjectManipulator>();

        boundsControl.RotateStopped.AddListener(OnChange);
        boundsControl.ScaleStopped.AddListener(OnChange);
        //boundsControl.TranslateStopped.AddListener(OnChange);

        objectManipulator.OnManipulationStarted.AddListener(OnChange);

    }

    

    private void ProfileListItemSelected(string profile)
    {
        SelectedProfile = profile; 
        SelectedProfileTextMesh.text = SelectedProfile;
        ProfileSelected?.Invoke(SelectedProfile);
        ProfileCollection.gameObject.SetActive(false);
        ProfileToggleButton.IsToggled = false;
        SettingsChanged = true;
    }

    private void ClearProfileCollection()
    {
        while (ProfileCollection.childCount > 0)
        {
            ProfileCollection.GetChild(0).gameObject.GetComponent<Interactable>().OnClick.RemoveAllListeners();
            DestroyImmediate(ProfileCollection.GetChild(0).gameObject);
        }
        //ProfileCollection.GetComponent<GridObjectCollection>().UpdateCollection();
    }

    public void Slice()
    {
        if(SettingsChanged == false)
        {
            STLPreviewController.DisableSTLPreview();
            ClosedWithoutSlicing.Invoke();
        }
        else if(slicingServiceConnection != null)
        {
            if(STLPreview == null)
            {
                return;
            }


            ProgressController.ActivateProgressIndicator();
            STLPreviewController.DisableSTLPreview();
            GCodeController.RemoveChildren();

            var stlContainer = STLPreview.transform.GetChild(0);
            var center = new Vector2(stlContainer.localPosition.x * 1000, stlContainer.localPosition.z * 1000);
            var scale = stlContainer.localScale.x;
            var rotation = new Vector3(-stlContainer.localRotation.eulerAngles.x, -stlContainer.localRotation.eulerAngles.z, - stlContainer.localRotation.eulerAngles.y);

            SettingsChanged = false;
            slicingServiceConnection.SliceModel(_modelFileName,center,scale, rotation);

        }
        gameObject.SetActive(false);
    }

    public void OnChange()
    {
        SettingsChanged = true;
    }
    private void OnChange(ManipulationEventData arg0)
    {
        OnChange();
    }
}
