using Assets._ExtendedPrinter.Scripts.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{

    public DialogClosed DialogClosed;

    public void OnEnable()
    {
        // Load data from player prefs
    }
    public void Save()
    {
        // validate inputs. save if valid, show dialog if no valid
    
    }
    public void Cancel()
    {
        DialogClosed?.Invoke(DialogResult.Canceled);

        gameObject.SetActive(false);
    }
}
