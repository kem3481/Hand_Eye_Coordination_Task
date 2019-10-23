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
                public Vector3 gazeDirection, fixationDirection, fixationpointPos;
                public Vector3 vector;

                public GameObject fixationPoint;
                public GameObject head;

                public bool ready;

                public float angularError;

                private void Start()
                {
                    ready = false;
                }

                void Update()
                {
                    fixationPoint.transform.SetParent(head.transform);
                    fixationPoint.transform.localPosition = Vector3.forward * 0.5f;
                    fixationpointPos = fixationPoint.transform.localPosition;

                    
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

                            AngularError(gazeDirection,  fixationpointPos);
                        }
                        else
                        {
                            angularError = 180;
                        }
                    }
                    if (angularError < 2)
                    {
                       ready = true;
                    }
                    if (angularError > 2)
                    {
                       ready = false;
                    }
                }
                

                void AngularError(Vector3 vector1, Vector3 vector2)
                {
                    angularError = (Mathf.Acos((Vector3.Dot(vector1, vector2))/((Vector3.Magnitude(vector1))* (Vector3.Magnitude(vector2)))) * Mathf.Rad2Deg);
                }
            }
        }
    }
}
