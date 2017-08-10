using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LSL;

namespace Assets.MobiSA.Scripts
{
    //Using an own LSLMarkerStream here for the Metadata  
    class CaptureMarkerStream : MonoBehaviour
    {

        private const string unique_source_id = "4314E8F41CCE499EBB7414F170D836C6";

        public string lslStreamName = "CaptureMarkerStream";
        public string lslStreamType = "LSL_Marker_Strings";

        private liblsl.StreamInfo lslStreamInfo;
        private liblsl.StreamOutlet lslOutlet;
        private int lslChannelCount = 1;

        //Assuming that markers are never send in regular intervalls
        private double nominal_srate = liblsl.IRREGULAR_RATE;

        private const liblsl.channel_format_t lslChannelFormat = liblsl.channel_format_t.cf_string;

        private string[] sample;

        void Awake()
        {
            sample = new string[lslChannelCount];

            lslStreamInfo = new liblsl.StreamInfo(
                                        lslStreamName,
                                        lslStreamType,
                                        lslChannelCount,
                                        nominal_srate,
                                        lslChannelFormat,
                                        unique_source_id);

            lslOutlet = new liblsl.StreamOutlet(lslStreamInfo);
        }

        public void Write(string marker)
        {
            sample[0] = marker;
            lslOutlet.push_sample(sample);
        }

        /*        public void Write(string marker, double customTimeStamp)
                {
                    sample[0] = marker;
                    lslOutlet.push_sample(sample, customTimeStamp);
                }

                public void Write(string marker, float customTimeStamp)
                {
                    sample[0] = marker;
                    lslOutlet.push_sample(sample, customTimeStamp);
                }
        */

    }

}
