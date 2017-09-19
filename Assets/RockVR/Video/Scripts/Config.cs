using UnityEngine;
using System;

namespace RockVR.Video
{
    /// <summary>
    /// Config setup for video related path.
    /// </summary>
    public class PathConfig
    {
        public static string persistentDataPath = Application.persistentDataPath;
        public static string streamingAssetsPath = Application.streamingAssetsPath;
        public static string fullpath = Application.dataPath;
        /// <summary>
        /// The video folder, save recorded video.
        /// </summary>
        public static string saveFolder { get { return fullpath; } }
        /// <summary>
        /// The ffmpeg path.
        /// </summary>
        public static string ffmpegPath
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                return streamingAssetsPath + "/RockVR/FFmpeg/Windows/ffmpeg.exe";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
				return streamingAssetsPath + "/RockVR/FFmpeg/OSX/ffmpeg";
#else
                return "";
#endif
            }
        }
        ///// <summary>
        ///// The <c>YoutubeUploader</c> script path.
        ///// </summary>
        //public static string youtubeUploader
        //{
        //    get
        //    {
        //        return streamingAssetsPath + "/RockVR/Scripts/YoutubeUploader.py";
        //    }
        //}
    }
}