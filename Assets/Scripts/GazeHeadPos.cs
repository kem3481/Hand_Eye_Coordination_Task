using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class SR_GazeGetter : MonoBehaviour
            {
                private static EyeData eyeData;
                private static VerboseData verboseData;
                
                public Vector3 binocularEIHorigin, binocularEIHdirection;
                public Vector3 gazeDirection, fixationDirection, fixationpointPos, headPos;

                public GameObject fixationPoint;
                public GameObject head;

                void Update()
                {
                    if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
                    {
                        VerboseData data;
                        if (SRanipal_Eye.GetVerboseData(out data) &&
                            data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY) &&
                            data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY)
                            )
                        {
                            SRanipal_Eye.GetEyeData(ref eyeData);
                            SRanipal_Eye.GetVerboseData(out verboseData);
                            SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out binocularEIHorigin, out binocularEIHdirection);

                            gazeDirection = binocularEIHorigin + binocularEIHdirection;
                            AngularError(gazeDirection, fixationDirection);
                        }
                    }

                }

                void AngularError(Vector3 vector1, Vector3 vector2)
                {
                    fixationpointPos = fixationPoint.transform.position;
                    headPos = head.transform.position;

                    fixationDirection = fixationpointPos - headPos;
                }
            }
        }
    }
}
