using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class GazeHeadPos : MonoBehaviour
            {
                private static EyeData eyeData;
                private static VerboseData verboseData;
                
                public Vector3 binocularEIHorigin, binocularEIHdirection;
                public Vector3 gazeDirection, fixationDirection, fixationpointPos, headPos;

                public GameObject fixationPoint;
                public GameObject head;

                public float angularError;

                void Update()
                {
                    fixationpointPos = fixationPoint.transform.position;
                    headPos = head.transform.position;

                    fixationDirection = fixationpointPos - headPos;

                    if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
                    {
                        VerboseData data;
                        Debug.Log("working");
                        if (SRanipal_Eye.GetVerboseData(out data) &&
                            data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY) &&
                            data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY)
                            )
                        {
                            SRanipal_Eye.GetEyeData(ref eyeData);
                            SRanipal_Eye.GetVerboseData(out verboseData);
                            SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out binocularEIHorigin, out binocularEIHdirection);

                            Debug.Log("binocularERHorigin: " + binocularEIHorigin);
                            gazeDirection = binocularEIHorigin + binocularEIHdirection;
                            AngularError(gazeDirection, fixationDirection);
                        }
                    }

                }

                void AngularError(Vector3 vector1, Vector3 vector2)
                {
                    angularError = Mathf.Acos((Vector3.Dot(vector1, vector2))/(Vector3.Magnitude(vector1)*Vector3.Magnitude(vector2)));
                }
            }
        }
    }
}
