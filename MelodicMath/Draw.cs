using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using Foundation;
using System.Collections.Generic;

namespace MelodicMath
{
    public class Whiteboard : UIView
    {
        CGPath path;
        CGPoint initialPoint;
        CGPoint latestPoint;

        List<CGPoint> touchPoints = new List<CGPoint>();

        public Whiteboard()
        {
            BackgroundColor = UIColor.White;

            path = new CGPath();

        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            UITouch touch = touches.AnyObject as UITouch;

            if (touch != null)
            {
                initialPoint = touch.LocationInView(this);
                touchPoints.Add(initialPoint);
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            UITouch touch = touches.AnyObject as UITouch;

            if (touch != null)
            {
                initialPoint = touch.LocationInView(this);
                touchPoints.Add(initialPoint);
                SetNeedsDisplay();
            }
        }

        /*public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            initialPoint = latestPoint;

        }*/

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            if (!initialPoint.IsEmpty)
            {

                //get graphics context
                using (CGContext g = UIGraphics.GetCurrentContext())
                {
                    g.SetAllowsAntialiasing(true);
                    foreach (CGPoint point in touchPoints)
                    {
                        g.AddEllipseInRect(new CGRect(point, new CGSize(15, 15)));
                        g.FillEllipseInRect(new CGRect(point, new CGSize(15, 15)));
                    }
                }
            }
        }
    }
}