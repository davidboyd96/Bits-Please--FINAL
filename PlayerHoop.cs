using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Drawing;
using System.Media;


namespace Microsoft.Samples.Kinect.BodyBasics
{
    class BoundingBox
    {
        public int x;
        public int y;
       
        public int width;
        public int height;

        public BoundingBox(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.width = w;
            this.height = h;
        }

        public bool isPointInside(Point p)
        {
            if (p.X > x && p.X < x + width)
                if (p.Y > y && p.Y < y + height)
                    return true;
            return false;
        }

        
    }
    class PlayerHoop
    {
       
        public int score;
        private bool isLeft;
        private BoundingBox box;
        private BitmapImage hoop;
        private SoundPlayer cheer;
        private Brush playerColor;
        
        private int displayWidth;
        private int displayHeight;

        public PlayerHoop (bool isLeft, int w, int h)
        {
            this.cheer = new SoundPlayer("../../../Sounds/cheer.wav");
            this.displayWidth = w;
            this.displayHeight = h;
            if (isLeft)
            {
                this.box = new BoundingBox(0, 200, 300, 300);
                this.hoop = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\../../../Images\\hoop-left.png"));
            }
            else
            {
                
                this.box = new BoundingBox(displayWidth - 620, 200, 300, 300);
                this.hoop = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\../../../Images\\hoop-right.png"));
            }

            this.score = 0;
            this.isLeft = isLeft;

            this.displayWidth = w;
            this.displayHeight = h;
            //this.hoop = new BitmapImage(new Uri("Images/hoop.gif"));

            if (isLeft) playerColor = Brushes.BlueViolet;
            else playerColor = Brushes.Brown;
        }


        public  bool isBallScored(HandleObject ball)
        {
            if (box.isPointInside(ball.center()) && ball.getVelocity().Y > 0 )
            {
                score++;
                cheer.Play();
                ball.processed();
                return true;
            }
            return false;
        }

        public void draw (DrawingContext dc)
        {
            displayBasket(dc);
            displayScore(dc);
            //displayBoundingBox(dc);
        }

        private void displayBoundingBox(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Transparent,  new Pen(Brushes.Red, 2), new Rect(new Point(box.x, box.y), new Point(box.x + box.width, box.y + box.height)));

        }
        private void displayBasket(DrawingContext dc)
        {
            if (isLeft) dc.DrawImage(hoop, new Rect(new Point(0, 200), new Point(300, 500)));
            else dc.DrawImage(hoop, new Rect(new Point(displayWidth - 620, 200), new Point(displayWidth - 280, 500)));
        }

        private void displayScore(DrawingContext dc)
        {
            FormattedText tx = new FormattedText(score.ToString(),
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                54,
                playerColor);

            if(isLeft)
                dc.DrawText(tx, new Point(displayWidth/2 - 230  , 30));
            else
                dc.DrawText(tx, new Point(displayWidth/2  - 40, 30)); ;
            
        }
    }
}
