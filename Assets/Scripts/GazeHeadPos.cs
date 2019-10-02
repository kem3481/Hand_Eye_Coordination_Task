﻿using System.Collections;
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
                public Vector3 vector;

                public GameObject fixationPoint;
                public GameObject head;
                public GameObject rig;

                public float angularError;

                void Update()
                {
                    fixationpointPos = fixationPoint.transform.position;
                    headPos = head.transform.position;

                    fixationDirection.x = fixationpointPos.x - headPos.x;
                    fixationDirection.y = fixationpointPos.y - headPos.y;
                    fixationDirection.z = fixationpointPos.z - headPos.z;

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

                            gazeDirection = head.transform.TransformPoint(binocularEIHorigin) + head.transform.TransformDirection(binocularEIHdirection);
                            AngularError(gazeDirection, fixationDirection);
                        }
                    }
                    Debug.DrawRay(headPos, fixationDirection, Color.cyan);
                    Debug.DrawRay(head.transform.TransformPoint(binocularEIHorigin), head.transform.TransformDirection(binocularEIHdirection), Color.red);
                }

                void AngularError(Vector3 vector1, Vector3 vector2)
                {
                    angularError = Mathf.Acos((Vector3.Dot(vector1, vector2))/(Vector3.Magnitude(vector1)*Vector3.Magnitude(vector2))) * Mathf.Rad2Deg;
                }
            }
        }
    }
}
