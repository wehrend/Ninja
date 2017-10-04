using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RockVR.Video;

namespace Assets.MobiSA.Scripts
{
    public class DiagnoseUI : MonoBehaviour {

        // Update is called once per frame
        void OnGUI() {
            GUI.TextField(new Rect(new Vector2(30, 30), new Vector2(250, 20)), "Capture Path:" + PathConfig.saveFolder);
            if (VideoCaptureCtrl.instance.status == VideoCaptureCtrlBase.StatusType.FINISH)
                GUI.TextField(new Rect(new Vector2(30, 60), new Vector2(500, 20)), string.Format("Video Capture File is {0} written.", StringUtils.GetMp4FileName(CaptureStringUtils.GetBlockName())));


        }
    }

}