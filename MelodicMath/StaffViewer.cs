using System;
using UIKit;
using CoreGraphics;
using System.Collections;

using System.ComponentModel;
using Foundation;

namespace MelodicMath
{
    [Register("StaffViewer"), DesignTimeVisible(true)] 
    public class StaffViewer : UIView
    {
        private string melody;
        private bool drawMelody = false;
        private int[] notePos;
        private int height;
        private int offset;

        private Hashtable notePositionMap = new Hashtable();
        private Hashtable noteImages = new Hashtable();


        public StaffViewer(CGRect p, int [] NotePositions, int noteHieght, int staffOffset) : base(p)
        {
            //set back ground to transparent
            BackgroundColor = UIColor.Clear;
            Opaque = false;

            //this is used to determine where staff lines will be placed
            notePos = NotePositions;
            height = noteHieght;
            offset = staffOffset;

            notePositionMap.Add("G1", notePos[0] - Convert.ToInt32((notePos[1] - notePos[0]) / 2));
            notePositionMap.Add("F1", notePos[0]);
            notePositionMap.Add("E1", Convert.ToInt32((notePos[0] + notePos[1])/2));
            notePositionMap.Add("D1", notePos[1]);
            notePositionMap.Add("C1", Convert.ToInt32((notePos[1] + notePos[2]) / 2));
            notePositionMap.Add("B0", notePos[2]);
            notePositionMap.Add("A0", Convert.ToInt32((notePos[2] + notePos[3]) / 2));
            notePositionMap.Add("G0", notePos[3]);
            notePositionMap.Add("F0", Convert.ToInt32((notePos[3] + notePos[4]) / 2));
            notePositionMap.Add("E0", notePos[4]);
            notePositionMap.Add("D0", notePos[4] + Convert.ToInt32((notePos[4] - notePos[3]) / 2));

            noteImages.Add(".25", "note_sixteen_sc.png");
            noteImages.Add(".5", "note_eigth_sc.png");
            noteImages.Add("1", "note_quarter_sc.png");
            noteImages.Add("2", "note_half_sc.png");
            noteImages.Add("4", "note_whole_sc.png");
            noteImages.Add(".25D", "note_sixteen_scD.png");
            noteImages.Add(".5D", "note_eigth_scD.png");
            noteImages.Add("1D", "note_quarter_scD.png");
            noteImages.Add("2D", "note_half_scD.png");
            noteImages.Add("4D", "note_whole_scD.png");
        }

        public void SetMelody(string newMelody)
        {
            //if (newMelody.Length > 1)
                drawMelody = true;
            //else
            //    drawMelody = false;
            melody = newMelody;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            using (CGContext g = UIGraphics.GetCurrentContext())
            {

                //set up drawing attributes
                g.SetLineWidth(2);
                UIColor.Blue.SetFill();
                UIColor.Black.SetStroke();

                //create geometry
                var path = new CGPath();

                //This creates the staff lines************************
                //int lineOffset = (int)height / 2;

                //int linePlacement = notePos[0] + lineOffset;

                path.AddLines(new CGPoint[] { new CGPoint(0, notePos[0]), new CGPoint(this.Frame.Width, notePos[0]) });

                path.AddLines(new CGPoint[] { new CGPoint(0, notePos[1]), new CGPoint(this.Frame.Width, notePos[1]) });

                path.AddLines(new CGPoint[] { new CGPoint(0, notePos[2]), new CGPoint(this.Frame.Width, notePos[2]) });

                path.AddLines(new CGPoint[] { new CGPoint(0, notePos[3]), new CGPoint(this.Frame.Width, notePos[3]) });

                path.AddLines(new CGPoint[] { new CGPoint(0, notePos[4]), new CGPoint(this.Frame.Width, notePos[4]) });

                path.CloseSubpath();

                //add geometry to graphics context and draw it
                g.AddPath(path);
                g.DrawPath(CGPathDrawingMode.FillStroke);

                //end staff lines************************

                //"C|1|1,A|1|0,C|.25|1,A|.25|0,C|.25|1,A|.25|0"
                if(drawMelody)
                {
                    //CGRect circleRect = new CGRect(0, notePos[0], 50, 50);
                    //circleRect = circleRect.Inset(0.5f, 0.5f);

                    // Draw the Circle
                    //path.AddEllipseInRect(circleRect);

                    string[] splitMelody = { "," };
                    string[] melodyInfo = melody.Split(splitMelody, StringSplitOptions.RemoveEmptyEntries);

                    int startPoint = 0;
                    foreach (string noteInfo in melodyInfo)
                    {
                        string[] splitNote = { "|" };
                        string[] note = noteInfo.Split(splitNote, StringSplitOptions.RemoveEmptyEntries);

                        //this uses the note and octave to find position
                        int pos = (int)notePositionMap[note[0] + note[2]];

                        //what note to display
                        string upDown = "";
                        int offsetPosition = pos;
                        int scaleWidth = 0;
                        if (pos < notePos[2])
                        {
                            upDown = "D";
                            offsetPosition = pos - 8;
                            scaleWidth = 25;

                        }
                        else{
                            offsetPosition = pos - 50 + 8;
                            scaleWidth = 35;
                        }
                        
                        CGSize scale = new CGSize(scaleWidth, 50);
                        CGRect imageRect = new CGRect(startPoint, offsetPosition, scaleWidth, 50);

                        g.DrawImage(imageRect, UIImage.FromFile((string)noteImages[note[1] + upDown]).Scale(scale).CGImage);
                        startPoint += scaleWidth;
                    }
                }

                //draw circle-----
                //CGRect circleRect = new CGRect(0, notePos[0]-25, 50, 25);
                //circleRect = circleRect.Inset(0.5f, 0.5f);
                //path.AddEllipseInRect(circleRect);
                //end draw circle


            }

        }
    }
}
