using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransformationUIController : MonoBehaviour
{

    public Transform Host;

    [SerializeField]
    private TextMeshPro PositionX;
    [SerializeField]
    private TextMeshPro PositionY;
    [SerializeField]
    private TextMeshPro PositionZ;
    [SerializeField]
    private TextMeshPro RotationX;
    [SerializeField]
    private TextMeshPro RotationY;
    [SerializeField]
    private TextMeshPro RotationZ;
    [SerializeField]
    private TextMeshPro ScaleX;
    [SerializeField]
    private TextMeshPro ScaleY;
    [SerializeField]
    private TextMeshPro ScaleZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Host != null)
        {
            PositionX.text = (Host.localPosition.x * 1000).ToString();
            PositionY.text = (Host.localPosition.z * 1000).ToString();//printer uses other coordination system 
            PositionZ.text = (Host.localPosition.y * 1000).ToString();

            RotationX.text = Host.localRotation.eulerAngles.x.ToString();
            RotationY.text = Host.localRotation.eulerAngles.z.ToString(); //printer uses other coordination system
            RotationZ.text = Host.localRotation.eulerAngles.y.ToString();

            ScaleX.text = (Host.localScale.x * 100f).ToString();
            ScaleY.text = (Host.localScale.z * 100f).ToString();//printer uses other coordination system
            ScaleZ.text = (Host.localScale.y * 100f).ToString(); 
        }
    }
}
