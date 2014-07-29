///<summary>
/// Xsens Mvn Pose represents a all segments data to create a pose.
///</summary>
///<version>
/// 0.1, 2013.03.12, Peter Heinen
/// 1.0, 2013.04.11, Attila Odry
///</version>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace xsens
{
    /// <summary>
    /// This class converts all the data from the packet into something Unity3D can easely read.
    /// This also contains the orientations and position fixes needed because of the different coordinate system.
    /// </summary>
    class XsMvnPose
    {
        public static int MvnSegmentCount = 23;

        public Vector3[] positions;
        public Quaternion[] orientations;

        public XsMvnPose()
        {
            positions = new Vector3[MvnSegmentCount];
            orientations = new Quaternion[MvnSegmentCount];
        }

        /// <summary>
        /// Creates the vector3 positions and the Quaternion rotations for unity, based on the current data packet.
        /// Recursive so it does every segment
        /// </summary>
        /// <param name='startPosition'>
        /// Start position.
        /// </param>
        /// <param name='segmentCounter'>
        /// Segment counter.
        /// </param>
        public void createPose(double[] payloadData)
        {
            int segmentCounter = 0;
            int startPosition = 0;

            while (segmentCounter < MvnSegmentCount)
            {
                Quaternion rotation = new Quaternion();
                Vector3 position = new Vector3();

                position.x = Convert.ToSingle(payloadData[startPosition + 1]);  //X=1
                position.y = Convert.ToSingle(payloadData[startPosition + 2]);	//Y=2
                position.z = Convert.ToSingle(payloadData[startPosition + 3]);	//Z=3

                rotation.w = Convert.ToSingle(payloadData[startPosition + 4]);	//W=4
                rotation.x = Convert.ToSingle(payloadData[startPosition + 5]);	//x=5 
                rotation.y = Convert.ToSingle(payloadData[startPosition + 6]);	//y=6
                rotation.z = Convert.ToSingle(payloadData[startPosition + 7]);	//Z=7

                positions[segmentCounter] = position;
                orientations[segmentCounter] = rotation;

                segmentCounter++;
                startPosition += 8;
            }
        }
    }//class XsMvnPose	
}//namespace xsens