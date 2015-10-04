using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Timers;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class HandleObject
    {
        private const float GrabRadiusOffset = 50;
        

        private int ID;
        // coordinates
        private Point defaultPos;
        private Point pos;
        private Point vel;
        private Point activePoint;
        private Point prevHandPoint;
        private Point[] averageOfPoints;
        private int index;

        //brushes for interaction
        private Brush currentBrush;
        private Brush inactiveBrush;
        private Brush activeBrush;
        private Pen pen;

        //circle radius
        private float radius;
        private double maxRadiusFactor, minRadiusFactor;
        private float adjRadius;

        private BitmapImage ball;

        //booleans
        HandState prevHandState;
        HandState currHandState;
        HandState activeHandState;


        //display
        private int displayWidth, displayHeight;

        bool isLeftHandOnObject, isRightHandOnObject;
        public bool isBoundToHand;

        private float activeDepth;


        //constructors
        public HandleObject()
        {
            pos = new Point(200, 200);
            vel = new Point(0, 0);
            this.radius = 20;
            inactiveBrush = Brushes.Red;
            activeBrush = Brushes.Green;
            pen = new Pen();
            prevHandState = HandState.Unknown;
            isLeftHandOnObject = false;
            isRightHandOnObject = false;
            isBoundToHand = false;
            currentBrush = inactiveBrush;
            maxRadiusFactor = 1.5;
            minRadiusFactor = 0.75;

        }
        public HandleObject(Point position, int radius, Point velocity, int displayWidth, int displayHeight, int id = -1)
        {
            this.ball = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\../../../Images\\basketball.png"));
            this.ID = id;
            this.pos = position;
            this.vel = velocity;
            this.radius = radius;
            this.adjRadius = radius;
            this.defaultPos = position;
            inactiveBrush = Brushes.Red;
            activeBrush = Brushes.Green;
            pen = new Pen();
            prevHandState = HandState.Unknown;
            isLeftHandOnObject = false;
            isRightHandOnObject = false;
            isBoundToHand = false;
            currentBrush = inactiveBrush;
            this.displayWidth = displayWidth;
            this.displayHeight = displayHeight;
            averageOfPoints = new Point[5];
            
        }

        public bool IsGrabbed()
        {
            return prevHandState == HandState.Open &&
                currHandState == HandState.Closed;
        }

        public bool IsReleased()
        { 
            return prevHandState == HandState.Closed &&
                currHandState == HandState.Open;
        }

        public float getRadius() {
            return radius;
        }

        public void onCollide(double x, double y)
        {
            vel.X = x;
            vel.Y = y;
        }

        public Point center()
        {
            return pos;
        }
        public Point getVelocity()
        {
            return vel;
        }
        
        public void processed()
        {
            pos = defaultPos;
            hAcc = 0;
            vel.X = 0;
            vel.Y = 0;
        }

        public bool IsHolding()
        {
            return prevHandState == HandState.Closed &&
                currHandState == HandState.Closed;
        }

        public bool isHandOnObject(Point point)
        {
            float totalRadius = radius + GrabRadiusOffset;
            return (Math.Abs(this.pos.X - point.X) < totalRadius && Math.Abs(this.pos.Y - point.Y) < totalRadius);
        }

        float hAcc = 0f;
        float hAccIncrement = 4f / 30f;
        double u;

        public void update (HandState leftHand, HandState rightHand, Point leftPoint, Point rightPoint, float leftHandDepth,float rightHandDepth)
        {
            prevHandPoint = activePoint;
            
            
            isLeftHandOnObject = isHandOnObject(leftPoint);
            isRightHandOnObject = isHandOnObject(rightPoint);

            if (isLeftHandOnObject)
            {
                activeHandState = leftHand;
                activePoint = leftPoint;
                activeDepth = leftHandDepth;
            }else if(isRightHandOnObject){
                activeHandState = rightHand;
                activePoint = rightPoint;
                activeDepth = rightHandDepth;
            }
            averageOfPoints[index % 5] = new Point(activePoint.X - prevHandPoint.X, activePoint.Y - prevHandPoint.Y);
            index += 1;

            if (pos.X + adjRadius>= displayWidth || pos.X - adjRadius<= 0)
            {
                vel.X = -vel.X;
            }
            
            this.pos.X += this.vel.X;


            if (!isBoundToHand)
            {

                hAcc += hAccIncrement;

                if (pos.Y + adjRadius >= displayHeight)
                {
                    vel.Y = -vel.Y;
                    pos.Y = displayHeight - radius * 2;
                }
                vel.Y += hAcc;
                this.pos.Y += this.vel.Y;

            }
            else
            {
                hAcc = 0;
                vel.Y = 0;
            }

            if (isLeftHandOnObject || isRightHandOnObject  || isBoundToHand)
            {
                if (activeHandState == HandState.Closed || activeHandState == HandState.Open)
                {
                    prevHandState = currHandState;
                    currHandState = activeHandState;
                }

                if (IsHolding() && !isBoundToHand)
                    return;
               

                //set color depending on the action performed 
                if (IsGrabbed())
                {  
                    if (isLeftHandOnObject || isRightHandOnObject)
                        isBoundToHand = true;
                    currentBrush = activeBrush;
                    radius = radius * activeDepth;
                    if (adjRadius > 60) adjRadius = 60;
                    if (adjRadius < 35) adjRadius = 35;
                }
                else if (IsReleased())
                {
                    double xTotal = 0, yTotal = 0;
                    isBoundToHand = false;
                    foreach (Point p in averageOfPoints)
                    {
                        xTotal += p.X;
                        yTotal += p.Y;
                    }
                    vel.X = xTotal / averageOfPoints.Length;
                    vel.Y = yTotal / averageOfPoints.Length;


                    currentBrush = inactiveBrush;
                }

                if ((IsGrabbed() || IsHolding()) && !isBoundToHand)
                {
                    Console.WriteLine("dh");
                }
                if ((IsGrabbed() || IsHolding()) && isBoundToHand)
                {
                    this.pos = activePoint;
                    adjRadius = radius / activeDepth;
                }
                else
                {
                    radius = adjRadius;
                }
            }
            else
            {
               // prevHandState = HandState.Closed;
                //currHandState = HandState.Closed;
            }
        }
        
        //draws the circle based on whether it is grabbed or not
        public void draw(DrawingContext dc)
        {
            if (adjRadius > 90) adjRadius = 90;
            if (adjRadius < 40) adjRadius = 40;
            if (ID == 2) dc.DrawEllipse(Brushes.Brown, pen, pos, adjRadius+5, adjRadius+5);
            else dc.DrawEllipse(Brushes.BlueViolet, pen, pos, adjRadius + 5, adjRadius + 5);
            Rect rc = new Rect(new Point(pos.X - adjRadius, pos.Y - adjRadius), new Point(pos.X + adjRadius, pos.Y + adjRadius));
            dc.DrawImage(ball, rc);
        }
    }
}
