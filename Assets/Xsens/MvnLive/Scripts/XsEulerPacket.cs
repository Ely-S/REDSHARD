///<summary>
/// This class will read the data from the stream as Euler angles and convert them to Quaternions.
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
using System.IO;

namespace xsens
{
    /// <summary>
    /// Xsens euler packet parse euler angles values from the stream.
    /// </summary>
    class XsEulerPacket : XsDataPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="xsens.XsEulerPacket"/> class.
        /// </summary>
        /// <param name='readData'>
        /// Create the packet from this data.
        /// </param>
        public XsEulerPacket(byte[] readData)
            : base(readData)
        {

        }

        /// <summary>
        /// Parses the payload.
        /// </summary>
        /// <param name='startPoint'>
        /// Start point.
        /// </param>
        protected override double[] parsePayload(BinaryReader br)
        {
            double[] payloadData = new double[XsMvnPose.MvnSegmentCount * 8];
            int startPoint = 0;
            int segmentCounter = 0;

            while (segmentCounter != XsMvnPose.MvnSegmentCount)
            {
				
                payloadData[startPoint + 0] = convert32BitInt(br.ReadBytes(4));     // Segment ID

                payloadData[startPoint + 1] = convert32BitFloat(br.ReadBytes(4)) / 100f;   // X position  <- switched with -Z
                payloadData[startPoint + 2] = convert32BitFloat(br.ReadBytes(4)) / 100f;   // Y Position
                payloadData[startPoint + 3] = convert32BitFloat(br.ReadBytes(4)) / 100f;   // Z Position  <- switched with x

                float x = (float)convert32BitFloat(br.ReadBytes(4)); //X ori
                float y = (float)convert32BitFloat(br.ReadBytes(4)); //Y ori
                float z = (float)convert32BitFloat(br.ReadBytes(4)); //Z ori
                Quaternion q = Quaternion.Euler(new Vector3(x, -y, -z));

                payloadData[startPoint + 4] = q.w;   // Quaternion W
                payloadData[startPoint + 5] = q.x;   // Quaternion X
                payloadData[startPoint + 6] = q.y;   // Quaternion Y
                payloadData[startPoint + 7] = q.z;   // Quaternion Z

                segmentCounter++;
                startPoint += 8;
            }

            return payloadData;
        }

    }//class XsEulerPacket

}//namespace xsens	
