using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Microsoft.Samples.Kinect.BodyBasics
{
    class BasketballManager
    {
        public HandleObject leftBall;
        public HandleObject rightBall;

        private PlayerHoop leftHoop;
        private PlayerHoop rightHoop;


        private BitmapImage scoreBoard;

        private int width;

        public BasketballManager(int displayWidth, int displayHeight)
        {
            width = displayWidth;
            scoreBoard = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\../../../Images\\court_scoreboard.jpg"));
            leftBall = new HandleObject(new Point(400, -100), 100, new Point(0, 1), displayWidth, displayHeight, 2);
            rightBall = new HandleObject(new Point(1520, -100), 100, new Point(0, 1), displayWidth, displayHeight);

            leftHoop = new PlayerHoop(true, displayWidth, displayHeight);
            rightHoop = new PlayerHoop(false, displayWidth, displayHeight);
        }

        public void update(Player pLeft, Player pRight)
        {
            updateBall(pLeft, leftBall);
            updateBall(pRight, rightBall);
            
            leftHoop.isBallScored(rightBall);
            rightHoop.isBallScored(leftBall);
        }

        public void draw(DrawingContext dc)
        {
            
            leftBall.draw(dc);
            rightBall.draw(dc);
            dc.DrawImage(scoreBoard, new Rect(new Point(width / 2 - 250, 0), new Point(width / 2, 150)));
            leftHoop.draw(dc);
            rightHoop.draw(dc);
            
        }

        private void updateBall(Player p, HandleObject b)
        {
            Point leftPoint = p.jointPoints[JointType.HandLeft];
            HandState leftHand = p.body.HandLeftState;
            float leftDepth = p.jointPointDepths[JointType.HandLeft];

            Point rightPoint = p.jointPoints[JointType.HandRight];
            HandState rightHand = p.body.HandRightState;
            float rightDepth = p.jointPointDepths[JointType.HandRight];

            if (ballsCollide(leftBall, rightBall))
            {
                if (!(leftBall.isBoundToHand || rightBall.isBoundToHand))
                {
                    Point leftVel = leftBall.getVelocity();
                    Point rightVel = rightBall.getVelocity();

                    leftBall.onCollide(rightVel.X, rightVel.Y);
                    rightBall.onCollide(leftVel.X, leftVel.Y);
                }
            }

            b.update(leftHand, rightHand, leftPoint, rightPoint, leftDepth, rightDepth);

        }

        private bool ballsCollide(HandleObject ballA, HandleObject ballB)
        {
            double a = ballA.getRadius() + ballB.getRadius();
            double dx = ballA.center().X - ballB.center().X;
            double dy = ballA.center().Y - ballB.center().Y;
            return a * a > (dx * dx + dy * dy);
        }
    }
}
