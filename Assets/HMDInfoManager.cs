using UnityEngine;
using UnityEngine.XR;

public class HMDInfoManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Is device active: "+ XRSettings.isDeviceActive);
        Debug.Log("Device name is: "+ XRSettings.loadedDeviceName);

        if(!XRSettings.isDeviceActive){
            Debug.Log("No headset plugged");
        }
        else if (XRSettings.isDeviceActive && XRSettings.loadedDeviceName == "MockHMD Display"){
            Debug.Log("Using Mock HMD");
        }
        else {
            Debug.Log("We have a headset: "+ XRSettings.loadedDeviceName);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
