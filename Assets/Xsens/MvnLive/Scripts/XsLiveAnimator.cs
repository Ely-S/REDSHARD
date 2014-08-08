///<summary>
/// XsLiveAnimator is a component to bind Xsens MVN Studio stream to Unity3D Game Engine.
/// MVN Studio capable to stream 4 actors at the same time and this component makes the 
/// connections between those actors and the characters in Unity3D.
/// 
/// Using the same settings on different characters will result of multiple characters are playing the same animation.
/// 
/// Relocation of the animation based on the start position of the character.
/// 
/// This component uses Macanim own animator to bind the bones with MVN avatar and the real model in Unity3D.
/// 
/// The animation applied on the pelvis as global position and rotation, while only the local rotation applied on each segments.
///</summary>
/// <version>
/// 1.0, 2013.04.11 by Attila Odry
/// </version>
///<remarks>
/// Copyright (c) 2013, Xsens Technologies B.V.
/// All rights reserved.
/// 
/// Redistribution and use in source and binary forms, with or without modification,
/// are permitted provided that the following conditions are met:
/// 
/// 	- Redistributions of source code must retain the above copyright notice, 
///		  this list of conditions and the following disclaimer.
/// 	- Redistributions in binary form must reproduce the above copyright notice, 
/// 	  this list of conditions and the following disclaimer in the documentation 
/// 	  and/or other materials provided with the distribution.
/// 
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
/// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
/// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS
/// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
/// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
/// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
/// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
/// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///</remarks>

//todo turn on this feature once we send T-Pose in the first frame
//#define TPOSE_FIRST

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace xsens
{

    /// <summary>
    /// Xsens Live Animator.
    /// 
    /// This class provide the logic to read an actor pose from MVN Stream and 
    /// retarget the animation to a character.
    /// </summary>
    /// <remarks>
    /// Attach this component to the character and bind the MvnActors with this object.
    /// </remarks>
    public class XsLiveAnimator : MonoBehaviour
    {

        public XsStreamReader mvnActors;			//network streamer, which contains all 4 actor's poses
        public int actorID = 1;						//current actor ID, where 1 is the first streamed character from MVN
        public bool applyRootMotion = true;			//if true, position will be applied to the root (pelvis)

        private Transform mvnActor; 				//reference for MVN Actor. This has the same layout as the streamed pose.
        private Transform target;                   //Reference to the character in Unity3D.
        private Transform origPos;					//original position of the animation, this is the zero
        private Transform[] targetModel;			//Model holds each segments
        private Transform[] currentPose;			//animation applyed on skeleton. global position and rotation, used to represent a pose	
        private Quaternion[] modelRotTP;			//T-Pose reference rotation on each segment
        private Vector3[] modelPosTP;				//T-Pose reference position on each segment
        private Vector3 pelvisPosTP; 				//T-Pose's pos for the pelvis
        private float scale = 0;
        private GameObject missingSegments;			//Empty object for not used segments
        private Animator animator;					//Animator object to get the Humanoid character mapping correct.
        private Dictionary<XsAnimationSegment, HumanBodyBones> mecanimBones;
        private bool isInited;						//flag to check if the plugin was correctly intialized.

#if TPOSE_FIRST		
        private bool isFirstPose;					//check if the first pose is passed
#endif		
        private bool isDebugFrame = false;			//debug animation skeleton

        /// <summary>
        /// Contains the segment numbers for the animation
        /// </summary>
        public enum XsAnimationSegment
        {
            Pelvis = 0, //hips

            RightUpperLeg = 1,
            RightLowerLeg = 2,
            RightFoot = 3,
            RightToe = 4,
            //--
            LeftUpperLeg = 5,
            LeftLowerLeg = 6,
            LeftFoot = 7,
            LeftToe = 8,

            L5 = 9,		//not used
            L3 = 10,	//spine
            T12 = 11,	//not used
            T8 = 12,	//chest

            LeftShoulder = 13,
            LeftUpperArm = 14,
            LeftLowerArm = 15,
            LeftHand = 16,

            RightShoulder = 17,
            RightUpperArm = 18,
            RightLowerArm = 19,
            RightHand = 20,

            Neck = 21,
            Head = 22

        }

        /// <summary>
        /// The segment order.
        /// </summary>
        int[] segmentOrder =
		{
					(int)XsAnimationSegment.Pelvis,
					(int)XsAnimationSegment.RightUpperLeg,
					(int)XsAnimationSegment.RightLowerLeg,
					(int)XsAnimationSegment.RightFoot,
					(int)XsAnimationSegment.RightToe,
					(int)XsAnimationSegment.LeftUpperLeg,
					(int)XsAnimationSegment.LeftLowerLeg,
					(int)XsAnimationSegment.LeftFoot,
					(int)XsAnimationSegment.LeftToe,
			
					(int)XsAnimationSegment.L5,
					(int)XsAnimationSegment.L3,
					(int)XsAnimationSegment.T12,
					(int)XsAnimationSegment.T8,
			
					(int)XsAnimationSegment.LeftShoulder,
					(int)XsAnimationSegment.LeftUpperArm,
					(int)XsAnimationSegment.LeftLowerArm,
					(int)XsAnimationSegment.LeftHand,
					
					(int)XsAnimationSegment.RightShoulder,
					(int)XsAnimationSegment.RightUpperArm,
					(int)XsAnimationSegment.RightLowerArm,
					(int)XsAnimationSegment.RightHand,
					(int)XsAnimationSegment.Neck,
					(int)XsAnimationSegment.Head
	
		};

        /// <summary>
        /// Maps the mecanim bones.
        /// </summary>
        protected void mapMecanimBones()
        {
            mecanimBones = new Dictionary<XsAnimationSegment, HumanBodyBones>();

            mecanimBones.Add(XsAnimationSegment.Pelvis, HumanBodyBones.Hips);
            mecanimBones.Add(XsAnimationSegment.LeftUpperLeg, HumanBodyBones.LeftUpperLeg);
            mecanimBones.Add(XsAnimationSegment.LeftLowerLeg, HumanBodyBones.LeftLowerLeg);
            mecanimBones.Add(XsAnimationSegment.LeftFoot, HumanBodyBones.LeftFoot);
            mecanimBones.Add(XsAnimationSegment.LeftToe, HumanBodyBones.LeftToes);
            mecanimBones.Add(XsAnimationSegment.RightUpperLeg, HumanBodyBones.RightUpperLeg);
            mecanimBones.Add(XsAnimationSegment.RightLowerLeg, HumanBodyBones.RightLowerLeg);
            mecanimBones.Add(XsAnimationSegment.RightFoot, HumanBodyBones.RightFoot);
            mecanimBones.Add(XsAnimationSegment.RightToe, HumanBodyBones.RightToes);
            mecanimBones.Add(XsAnimationSegment.L5, HumanBodyBones.LastBone);	//not used
            mecanimBones.Add(XsAnimationSegment.L3, HumanBodyBones.Spine);
            mecanimBones.Add(XsAnimationSegment.T12, HumanBodyBones.LastBone);	//not used
            mecanimBones.Add(XsAnimationSegment.T8, HumanBodyBones.Chest);
            mecanimBones.Add(XsAnimationSegment.LeftShoulder, HumanBodyBones.LeftShoulder);
            mecanimBones.Add(XsAnimationSegment.LeftUpperArm, HumanBodyBones.LeftUpperArm);
            mecanimBones.Add(XsAnimationSegment.LeftLowerArm, HumanBodyBones.LeftLowerArm);
            mecanimBones.Add(XsAnimationSegment.LeftHand, HumanBodyBones.LeftHand);
            mecanimBones.Add(XsAnimationSegment.RightShoulder, HumanBodyBones.RightShoulder);
            mecanimBones.Add(XsAnimationSegment.RightUpperArm, HumanBodyBones.RightUpperArm);
            mecanimBones.Add(XsAnimationSegment.RightLowerArm, HumanBodyBones.RightLowerArm);
            mecanimBones.Add(XsAnimationSegment.RightHand, HumanBodyBones.RightHand);
            mecanimBones.Add(XsAnimationSegment.Neck, HumanBodyBones.Neck);
            mecanimBones.Add(XsAnimationSegment.Head, HumanBodyBones.Head);
        }

        /// <summary>
        /// Awake this instance and initialize the live objects.
        /// </summary>
        void Awake()
        {

            isInited = false;
#if TPOSE_FIRST
            isFirstPose = true;
#endif

            //save start positions
            target = gameObject.transform;
            origPos = target;

            //create an MvnActor 
            GameObject obj = (GameObject)Instantiate(Resources.Load("MvnActor"));
            obj.transform.parent = gameObject.transform;
            mvnActor = obj.transform;
            if (mvnActor == null)
            {
                Debug.LogError("[xsens] No AnimationSkeleton found!");
                return;
            }

            // Search for the network stream, so we can communicate with it.
            if (mvnActors == null)
            {
                Debug.LogError("[xsens] No MvnActor found! You must assing an MvnActors to this component.");
                return;
            }

            try
            {

                //setup arrays for pose's
                targetModel = new Transform[XsMvnPose.MvnSegmentCount];
                modelRotTP = new Quaternion[XsMvnPose.MvnSegmentCount];
                modelPosTP = new Vector3[XsMvnPose.MvnSegmentCount];
                currentPose = new Transform[XsMvnPose.MvnSegmentCount];

                //add an empty object, which we can use for missing segments
                missingSegments = new GameObject("MissingSegments");
                missingSegments.transform.parent = gameObject.transform;

                //map each bone with xsens bipad model and mecanim bones
                mapMecanimBones();
                //setup the animation and the model as well
                if (!setupMvnActor())
                {
                    Debug.Log("[xsens] failed to init MvnActor");
                    return;
                }

                if (!setupModel(target, targetModel))
                {
                    Debug.Log("[xsens] failed to init the model");
                    return;
                }



                //face model to the right direction	
                //target.transform.rotation = transform.rotation;

                isInited = true;
            }
            catch (Exception e)
            {
                print("[xsens] Something went wrong setting up.");
                Debug.LogException(e);
            }

        }


        /// <summary>
        /// Setups the mvn actor, with binding the currentPose to the actor bones.
        /// </summary>
        /// <returns>
        /// true on success
        /// </returns>
        public bool setupMvnActor()
        {

            mvnActor.rotation = transform.rotation;
            mvnActor.position = transform.position;

            currentPose[(int)XsAnimationSegment.Pelvis] = mvnActor.Find("Pelvis");
            currentPose[(int)XsAnimationSegment.L5] = mvnActor.Find("Pelvis/L5");

            currentPose[(int)XsAnimationSegment.L3] = mvnActor.Find("Pelvis/L5/L3");
            currentPose[(int)XsAnimationSegment.T12] = mvnActor.Find("Pelvis/L5/L3/T12");
            currentPose[(int)XsAnimationSegment.T8] = mvnActor.Find("Pelvis/L5/L3/T12/T8");
            currentPose[(int)XsAnimationSegment.LeftShoulder] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder");
            currentPose[(int)XsAnimationSegment.LeftUpperArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder/LeftUpperArm");
            currentPose[(int)XsAnimationSegment.LeftLowerArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder/LeftUpperArm/LeftLowerArm");
            currentPose[(int)XsAnimationSegment.LeftHand] = mvnActor.Find("Pelvis/L5/L3/T12/T8/LeftShoulder/LeftUpperArm/LeftLowerArm/LeftHand");

            currentPose[(int)XsAnimationSegment.Neck] = mvnActor.Find("Pelvis/L5/L3/T12/T8/Neck");
            currentPose[(int)XsAnimationSegment.Head] = mvnActor.Find("Pelvis/L5/L3/T12/T8/Neck/Head");

            currentPose[(int)XsAnimationSegment.RightShoulder] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder");
            currentPose[(int)XsAnimationSegment.RightUpperArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder/RightUpperArm");
            currentPose[(int)XsAnimationSegment.RightLowerArm] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder/RightUpperArm/RightLowerArm");
            currentPose[(int)XsAnimationSegment.RightHand] = mvnActor.Find("Pelvis/L5/L3/T12/T8/RightShoulder/RightUpperArm/RightLowerArm/RightHand");

            currentPose[(int)XsAnimationSegment.LeftUpperLeg] = mvnActor.Find("Pelvis/LeftUpperLeg");
            currentPose[(int)XsAnimationSegment.LeftLowerLeg] = mvnActor.Find("Pelvis/LeftUpperLeg/LeftLowerLeg");
            currentPose[(int)XsAnimationSegment.LeftFoot] = mvnActor.Find("Pelvis/LeftUpperLeg/LeftLowerLeg/LeftFoot");
            currentPose[(int)XsAnimationSegment.LeftToe] = mvnActor.Find("Pelvis/LeftUpperLeg/LeftLowerLeg/LeftFoot/LeftToe");
            currentPose[(int)XsAnimationSegment.RightUpperLeg] = mvnActor.Find("Pelvis/RightUpperLeg");
            currentPose[(int)XsAnimationSegment.RightLowerLeg] = mvnActor.Find("Pelvis/RightUpperLeg/RightLowerLeg");
            currentPose[(int)XsAnimationSegment.RightFoot] = mvnActor.Find("Pelvis/RightUpperLeg/RightLowerLeg/RightFoot");
            currentPose[(int)XsAnimationSegment.RightToe] = mvnActor.Find("Pelvis/RightUpperLeg/RightLowerLeg/RightFoot/RightToe");

            //reset pelvis TP
            pelvisPosTP = currentPose[(int)XsAnimationSegment.Pelvis].position;

            return true;

        }

        /// <summary>
        /// Setups the model.
        /// Bind the model with the animation.
        /// This funciton will use Macanim animator to find the right bones, 
        /// then it will store it in an array by animation segment id
        /// </summary>
        /// <param name='model'>
        /// Model.
        /// </param>
        /// <param name='modelRef'>
        /// Model reference.
        /// </param>
        /// <returns>
        /// true on success
        /// </returns>
        public bool setupModel(Transform model, Transform[] modelRef)
        {

            animator = model.GetComponent("Animator") as Animator;
            if (!animator)
            {
                Debug.LogError("[xsens] No Animator component found for the model! You need to have an Animator attached to your model!");
                return false;
            }

            //face the input model same as our animation
            model.rotation = transform.rotation;
            model.position = transform.position;

            //go through the model's segments and store values
            for (int i = 0; i < XsMvnPose.MvnSegmentCount; i++)
            {

                XsAnimationSegment segID = (XsAnimationSegment)segmentOrder[i];
                HumanBodyBones boneID = mecanimBones[(XsAnimationSegment)segmentOrder[i]];

                try
                {

                    if (boneID == HumanBodyBones.LastBone)
                    {
                        //not used bones
                        modelRef[(int)segID] = null;
                        modelPosTP[(int)segID] = Vector3.zero;
                        modelRotTP[(int)segID] = Quaternion.Euler(Vector3.zero);

                    }
                    else
                    {
                        //used bones
                        modelRef[(int)segID] = animator.GetBoneTransform(boneID);
                        modelPosTP[(int)segID] = modelRef[(int)segID].position;
                        modelRotTP[(int)segID] = modelRef[(int)segID].rotation;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("[xsens] Can't find " + boneID + " in the model! " + e);
                    modelRef[(int)segID] = null;
                    modelPosTP[(int)segID] = Vector3.zero;
                    modelRotTP[(int)segID] = Quaternion.Euler(Vector3.zero);

                    return false;
                }

            }

            //calculate simple scale factor
            scale = (float)(modelPosTP[(int)XsAnimationSegment.Pelvis].y / pelvisPosTP.y);

            return true;

        }

        /// <summary>
        ///	Update the body segments in every frame.
        ///
        /// The mvn actor's segment orientations and positions is read from the network,
        /// using the MvnLiveActor component. 
        /// This component provides all data for the current pose for each actors.
        /// </summary>
        void Update()
        {
            //only do magic if we have everything ready
            if (!isInited)
                return;

            Vector3[] latestPositions;
            Quaternion[] latestOrientations;
            // Get the pose data in one call, else the orientation might come from a different pose
            // than the position
            if (mvnActors.getLatestPose(actorID - 1, out latestPositions, out latestOrientations))
            {
    
#if TPOSE_FIRST
                if (isFirstPose)
                {
                    //init the animation skeleton with the first pose
                    initSkeleton(currentPose, latestPositions, latestOrientations);
                }
                else
#endif                    
                {
					
                    //update pose		
                    updateMvnActor(currentPose, latestPositions, latestOrientations);
                    updateModel(currentPose, targetModel);
                }
            }

        }

        /// <summary>
        /// Updates the actor skeleton local positions, based on the first valid pose
        /// 
        /// </summary>
        /// <param name='model'>
        /// Model.
        /// </param>
        /// <param name='positions'>
        /// Positions.
        /// </param>
        protected void initSkeleton(Transform[] model, Vector3[] positions, Quaternion[] orientations)
        {

            //wait for real data
            if (positions[(int)XsAnimationSegment.Pelvis] == Vector3.zero)
            {
                return;
            }

            //reposition the segments based on the data
#if TPOSE_FIRST			
            isFirstPose = false;
#endif

            for (int i = 0; i < segmentOrder.Length; i++)
            {
                if (XsAnimationSegment.Pelvis == (XsAnimationSegment)segmentOrder[i])
                {
                    //global for pelvis
                    model[segmentOrder[i]].transform.position = positions[segmentOrder[i]];
                    model[segmentOrder[i]].transform.rotation = orientations[segmentOrder[i]];

                }
                else
                {
                    //local for segments
                    model[segmentOrder[i]].transform.localPosition = positions[segmentOrder[i]];
                    model[segmentOrder[i]].transform.localRotation = orientations[segmentOrder[i]];
                }
            }

            //reinit the actor
            setupMvnActor();
        }

        /// <summary>
        /// Updates the mvn actor segment orientations and positions.
        /// </summary>
        /// <param name='model'>
        /// Model to update.
        /// </param>
        /// <param name='positions'>
        /// Positions in array
        /// </param>
        /// <param name='orientations'>
        /// Orientations in array
        /// </param>
        private void updateMvnActor(Transform[] model, Vector3[] positions, Quaternion[] orientations)
        {

            try
            {
                for (int i = 0; i < segmentOrder.Length; i++)	//front
                {
                    if (XsAnimationSegment.Pelvis == (XsAnimationSegment)segmentOrder[i])
                    {
                        //we apply global position and orientaion to the pelvis
                        if (applyRootMotion)
                        {
                            model[segmentOrder[i]].transform.position = positions[segmentOrder[i]] + origPos.position;
                        }
                        model[segmentOrder[i]].transform.rotation = orientations[segmentOrder[i]];

                    }
                    else
                    {
                        //segment's data in local orientation (no position)
                        if (model[segmentOrder[i]] == null)
                        {
                            Debug.LogError("[xsens] XsLiveAnimator: Missing bone from mvn actor! Did you change MvnLive plugin? Please check if you are using the right actor!");
                            break;

                        }
                        model[segmentOrder[i]].transform.localRotation = orientations[segmentOrder[i]];

                        //draw wireframe for original animation
                        if (isDebugFrame)
                        {

                            Color color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                            int id = segmentOrder[i];
                            if (model[id - 1] != null)
                            {
                                if (((id - 1) != (int)XsAnimationSegment.LeftHand)
                                && ((id - 1) != (int)XsAnimationSegment.RightHand)
                                && ((id - 1) != (int)XsAnimationSegment.Head)
                                && ((id - 1) != (int)XsAnimationSegment.LeftToe)
                                && ((id - 1) != (int)XsAnimationSegment.RightToe))
                                {
                                    Debug.DrawLine(model[id].position, model[id - 1].position, color);
                                }
                            }
                        }//isDebugFrame
                    }

                }//for i
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Updates the model.
        /// Evert pose contains the transform objects for each segments within the model.
        /// </summary>
        /// <param name='pose'>
        /// Pose holds the positions and orientations of the actor.
        /// </param>
        /// <param name='model'>
        /// Model to update with the pose.
        /// </param>
        private void updateModel(Transform[] pose, Transform[] model)
        {

            //reset the target, then set it based on segments
            Vector3 pelvisPos = new Vector3();
            Vector3 lastPos = target.position;
            target.position = Vector3.zero;
            for (int i = 0; i < XsMvnPose.MvnSegmentCount; i++)
            {
                
                switch (i)
                {
                    //no update required
                    case (int)XsAnimationSegment.L5:
                    case (int)XsAnimationSegment.T12:
                        break;

                    case (int)XsAnimationSegment.Pelvis:
                        //position only on the y axis, leave the x,z to the body
                        pelvisPos = (pose[i].position * scale);
                        model[i].position = new Vector3(model[i].position.x, pelvisPos.y, model[i].position.z);

                        //we apply global position and orientaion to the pelvis
                        model[i].rotation = pose[i].rotation * modelRotTP[i];
                        break;

                    default:
                        //only update rotation for rest of the segments
                        if (model[i] != null)
                        {
                            model[i].rotation = pose[i].rotation * modelRotTP[i];

                        }
                        break;
                }

            }

            //apply root motion if flag enabled only
            if (applyRootMotion)
            {
                //only update x and z, pelvis is already modified by y
                target.position = new Vector3(pelvisPos.x + pelvisPosTP.x, lastPos.y, pelvisPos.z + pelvisPosTP.z);
            }
            //set final rotation of the full body, but only position it to face similar as the pelvis
            Quaternion q = Quaternion.Inverse(modelRotTP[(int)XsAnimationSegment.Pelvis]) * model[(int)XsAnimationSegment.Pelvis].rotation;
            target.rotation = new Quaternion(target.rotation.x, q.y, target.rotation.z, target.rotation.w);

        }

    }//class XsLiveAnimator

}//namespace Xsens