using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SMI;


public class HMDAPITEST : MonoBehaviour {

    //Text mesh to display info
    public TextMesh GazeStream;
    public string data;
    public TextMesh LogStream;
    string logMessage = "";
    SMI.SMIGazeController.SMIcWrapper.smi_CalibrationClass calibrationClass;

    // Use this for initialization
    void Start () {
        SMI.SMIGazeController.Instance.smi_setupCalibration(calibrationClass);
	}


    // Update is called once per frame
    void Update()
    {

        //Show streamed values for each gaze sample
        // timestamp: sample timestamp [nsec]
        // binocularPor.x and .y: x, y coordinates of the mapped combined (averaged) point of regard on the virtual display [2560, 1440] (90 degree FOV at 1.5 meter)
        // binocularPor.isValid: validity of the point of regard
        // double eyeDistances.iod: interocular distance, i.e. distance between left and right gaze base point [mm]
        // double eyeDistances.ipd: interpupillary distance, i.e. distance between left and right pupil center [mm]
        // bool eyeDistances.isValid: validity of eye distances
        SMI.SMIGazeController.unity_SampleHMD sample
            = SMI.SMIGazeController.Instance.smi_getSample();
        if (sample != null)
        {
            //GazeStream.text = 
            data= 
                "sample:\t " + sample.timeStamp + "\n" +
                "Por:\n\t(" + sample.por.x + ", "
                    + sample.por.y + ")\n" +
                "\tisValid: " + sample.isValid + "\n" +
                "eyeDistances\n\tiod: " + sample.iod +
                " ipd: " + sample.ipd + "\n"
                ;
            Debug.Log(data);

        }

        //Show gameobject in gaze focus
        //TestGetGameObjectInFocus();

        LogStream.text = logMessage;

    }
    /// <summary>
    /// has to be wrapped
    /// </summary>
    //Test available calibrations
    public void TestAvailableCalibrations()
    {
        string[] s = SMI.SMIGazeController.SMIcWrapper.smi_getAvailableCalibrations();
        logMessage = "Available Calibrations\n";
        foreach (string t in s)
        {
            logMessage += t + " ";
        }
    }
    /// <summary>
    /// has to be wrapped
    /// </summary>
    /// <param name="userName"></param>
    //Test save calibration
    public void TestSaveCalibration(string userName)
    {
        SMI.SMIGazeController.SMIcWrapper.smi_saveCalibration( userName);

    }

    /// <summary>
    /// has to be wrapped
    /// </summary>
    /// <param name="userName"></param>
    //Test load calibration
    public void TestLoadCalibration(string userName)
    {
        SMI.SMIGazeController.SMIcWrapper.smi_loadCalibration(userName);
    }

    /// <summary>
    /// has to be wrapped
    /// </summary>
    /// <param name="userName"></param>
    //Test calibration
    public void TestOnePointCalibration()
    {
        //3 point is the default
        Debug.Log("SMI calibrate");
        SMI.SMIGazeController.SMIcWrapper.smi_calibrate();
    }
/*
    //Test validation
    public void TestValidation()
    {
        SMI.SMIEyeTrackingMobile.Instance.smi_Validate();
        Invoke("TestCloseValidation", 5f);
    }

    //Test close validation
    public void TestCloseValidation()
    {
        SMI.SMIEyeTrackingMobile.Instance.smi_CloseVisualization();
    }

    */
    //Test get gameobject in focus
    public void TestGetGameObjectInFocus()
    {
        GameObject gameObj = SMI.SMIGazeController.Instance.smi_getGameObjectInFocus();
        if (gameObj != null)
        {
            GazeStream.text += ("Gazed object: " + gameObj.name + "\n");

            //3D gaze point can be obtained from raycast
            RaycastHit gazeHit;
            if (SMI.SMIGazeController.Instance.smi_getRaycastHitFromGaze(out gazeHit))
                GazeStream.text += ("3D gaze hit point in world space: \n    (" + gazeHit.point.x + ", " + gazeHit.point.y + ", " + gazeHit.point.z + ")\n");
                
        }
    }

    //Test reset calibration
    public void TestResetCalibration()
    {
        SMI.SMIGazeController.SMIcWrapper.smi_ResetCalibration();
    }

    //Test quit application
    public void TestQuitApplication()
    {
        SMI.SMIGazeController.SMIcWrapper.smi_quit();
    }

   //Test streaming start/stop
    bool isStreaming = true;
    public void TestToggleStreaming()
    {
        bool simulate = false;
        IntPtr trackingInfo = IntPtr.Zero;
        if (!isStreaming)
        {
            SMI.SMIGazeController.SMIcWrapper.smi_startStreaming(simulate, trackingInfo);
            isStreaming = true;
        }
        else
        {
            SMI.SMIGazeController.SMIcWrapper.smi_stopStreaming();
            isStreaming = false;
        }
    }
}
