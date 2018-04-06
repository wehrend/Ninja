/*using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using LSL;
using UnityEngine.SceneManagement;
using SMI;
using System.Collections.Generic;
using Assets.MobiSA.Scripts;

/// <summary>
/// This is not working right now, will be added in the future
/// </summary>


[TestFixture]
public class TestLslStreamsAvailability {
   
    liblsl.ContinuousResolver resolver;

    GameObject player;
    SteamVR_Camera vrCamera;
    SteamVR_Controller.Device controllerDevice;
    SMIEyetracking eyetracking;


    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
	public IEnumerator TestLslStreamsAvailabilityWithEnumeratorPasses() {
        //Arrange 

        //Get Lsl resolver 
        resolver = new liblsl.ContinuousResolver();

        //Get SteamVR's Player script
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;

        player = GameObject.Find("Player(with Eyetracking)");

        //check camera avail
        vrCamera = player.GetComponentInChildren<SteamVR_Camera>();

        //check controller device avail
        controllerDevice = player.GetComponentInChildren<SteamVR_Controller.Device>();

        //check eyetracking avail
        eyetracking = player.GetComponentInChildren<SMIEyetracking>();



   

        //Act 
        var streamInfo = resolver.results();
        foreach (var item in streamInfo)

        Debug.Log(string.Format("Found {0} streams", streamInfo.Length));
        var listOfStreams = new List<liblsl.StreamInfo>(streamInfo);
        //Assert
        if (vrCamera != null) {
            var hmdStream = new RBHmdStream().GetStreamInfo();
            Assert.Contains(hmdStream, listOfStreams, "LSL streams contains hmd stream");
        }
        if (controllerDevice != null)
        {
            var controllerStream = new RBControllerStream().GetStreamInfo();
            Assert.Contains(controllerStream, listOfStreams, "LSL streams contains controller stream");
        }
        if (eyetracking != null)
        {
            var eyeStream = new RBeyetrackingStream().GetStreamInfo();
            Assert.Contains(eyeStream, listOfStreams, "LSL streams contains eyetracking stream");
        }
    }
}*/
