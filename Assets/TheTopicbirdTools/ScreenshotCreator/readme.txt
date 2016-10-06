******************************
***   Screenshot-Creator   ***
******************************
by The Topicbird
www.thetopicbird.com

With this script you can take screenshots inside the Unity Editor. 


INSTRUCTIONS:
1. To take screenshots, simply add the Script "ScreenshotCreator" to a GameObject in your Scene or open a new Editor Window by clicking on the Menu Item "Window" and then "Screenshot Creator".

2. The Settings tab can be expanded. It lets you set all the important options of your screenshots:
- size multiplier: defines the resolution of the screenshot (as multiples of the Camera resolution).
- folder: the save location (you can mark and copy this).
- custom name: name of your screenshot. If you leave this empty, your Scenes name will be used.
- some toggles for additional screenshot name information and a file format selection (PNG is lossless, JPG is compressed).
- file name: the projected name of you next screenshot, though the actual Camera name will be used depending on the Camera.
- capture method: Render Texture is selected as a default, because the lines are much cleaner, but it can only be used with a single Camera. Fallback for other solutions is the Application.CaptureScreenshot method that was also used in the first version of this tool.

3. To add Cameras, press the blue "ADD CAMERA" button at the bottom of the Inspector / Editor Window. Then drag any Camera from your Scene into the empty new slot that says "None (Game Object)" (can use "Render Texture" capture method). You can also select a parent GameObject with other Cameras as children (fallback to "Capture Screenshot" capture method). For ScreenshotCreator on a GameObject, there is also a hotkey button which works in PlayMode.

4. You can delete a Camera slot by pressing the respective red "X" button. The delete button will ask you if you are sure and you have to click the button a second time consecutive.


For any feedback or questions please contact us at talk@thetopicbird.com.
If you like this tool, feel free to leave a comment in the Assetstore.