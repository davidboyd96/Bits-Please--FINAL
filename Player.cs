using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class Player
    {
        public Body body { get; set; }
        public Dictionary<JointType, Point> jointPoints { get; set; }
        public Dictionary<JointType, float> jointPointDepths { get; set; }

        public Player(Body body)
        {
            this.body = body;
        }
    }
}
