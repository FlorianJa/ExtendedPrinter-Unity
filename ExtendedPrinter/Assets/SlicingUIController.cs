using Assets._ExtendedPrinter.Scripts.Helper;
using Assets._ExtendedPrinter.Scripts.SlicingService;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    private string _modelFileName;

    public StringEvent ProfileSelected;

    public string SelectedProfile { get; private set; }

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
                bch.MainLabelText = profile;
                listItem.GetComponent<Interactable>().OnClick.AddListener(() => ProfileListItemSelected(profile));
            }
            SelectedProfile = profiles[profiles.Count-1];
            SelectedProfileTextMesh.text = SelectedProfile;
            ProfileCollection.GetComponent<GridObjectCollection>().UpdateCollection();
        }
    }

    private void ProfileListItemSelected(string profile)
    {
        SelectedProfile = profile; 
        SelectedProfileTextMesh.text = SelectedProfile;
        ProfileSelected?.Invoke(SelectedProfile);
        ProfileCollection.gameObject.SetActive(false);
        ProfileToggleButton.IsToggled = false;
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
        if(slicingServiceConnection != null)
        {
            if(STLPreview == null)
            {
                return;
            }

            var stlContainer = STLPreview.transform.GetChild(0);
            var center = new Vector2(stlContainer.localPosition.x * 1000, stlContainer.localPosition.z * 1000);
            var scale = stlContainer.localScale.x;
            var rotation = new Vector3(-stlContainer.localRotation.eulerAngles.x, -stlContainer.localRotation.eulerAngles.z, - stlContainer.localRotation.eulerAngles.y);
            slicingServiceConnection.SliceModel(_modelFileName,center,scale, rotation);

            gameObject.SetActive(false);
        }
    }
}
