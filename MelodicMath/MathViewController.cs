using Foundation;
using System;
using UIKit;
using CoreGraphics;

using System.Collections.Generic;

using System.IO;

using CoreAnimation;

namespace MelodicMath
{
    public partial class MathViewController : UIViewController
    {
        public List<string> MelodyInfo { get; set; }

        public MathGenerator mGen = new MathGenerator();

        //this creates the music staff and notes
        private StaffViewer layout;

        private List<string> questionInfo;
        private string question = "";
        private string answer = "";
        private string formula = "";

        private Whiteboard board;

        public MathViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            vwFeedback.Layer.BorderColor = UIColor.Black.CGColor;
            vwFeedback.Layer.BorderWidth = (nfloat)1.0;

            //--------Staff set up---------

            //notePositions will tell us where to draw staff in case sizes, positions change, noteHieght lets us find mid point
            int[] notePositions = { (int)Btn_FP1.Frame.Y, (int)Btn_D.Frame.Y, (int)Btn_B.Frame.Y, (int)Btn_G.Frame.Y, (int)Btn_E.Frame.Y };
            int noteHeight = (int)Btn_FP1.Frame.Height;
            int staffOffset = (int)(Btn_FP1.Frame.X * .9);

            //start of staff will be based on second E + width and will go down 90% from top, end points will be entire screen minus offsets and bottom E note
            layout = new StaffViewer(new CGRect(Btn_EP1.Frame.X + Btn_EP1.Frame.Width, (int)(Btn_FP1.Frame.X * .9), UIScreen.MainScreen.Bounds.Width - Btn_FP1.Frame.X - (Btn_EP1.Frame.X + Btn_EP1.Frame.Width), Btn_E.Frame.Y + Btn_E.Frame.Height), notePositions, noteHeight, staffOffset);

            layout.SetMelody(MelodyInfo[0]);
            layout.SetNeedsDisplay();

            View.AddSubview(layout);

            //-----Staff set up end--------

            board = new Whiteboard { Frame = new CGRect(0, vwFeedback.Frame.Y + vwFeedback.Frame.Height, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height) }; //UIScreen.MainScreen.Bounds };
            View.AddSubview(board);

            btn_Submit.TouchUpInside += (object sender, EventArgs e) =>
            {
                string message = "";
                if (answer.Equals(txt_Answer.Text))
                    message = "Correct! Good Job!";
                else
                    message = "Try Again!";

                lbl_vwMsg.Text = message;
                vwFeedback.Hidden = false;

                if (lbl_vwMsg.Text.Equals("No"))
                {
                    var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var filename = Path.Combine(documents, "MelodicMath_Errors");
                    File.AppendAllText(filename, question + ":" + answer + ":" + txt_Answer.Text);
                }
            };

            btn_vwOK.TouchUpInside += (object sender, EventArgs e) =>
            {
                vwFeedback.Hidden = true;
                lbl_vwMsg.Text = "";
            };

            btn_Hint.TouchUpInside += (object sender, EventArgs e) =>
            {
                lbl_Hint.Text = formula;
            };

            btn_New.TouchUpInside += (object sender, EventArgs e) =>
            {
                GenerateQuestion();
            };

            GenerateQuestion();
            /*
            questionInfo = mGen.Generate(MelodyInfo);
            question = questionInfo[0];
            answer = questionInfo[1];
            formula = questionInfo[2];
            lblMathQuestion.Text = questionInfo[0];
            */
        }

        public void GenerateQuestion()
        {
            questionInfo = mGen.Generate(MelodyInfo);
            question = questionInfo[0];
            answer = questionInfo[1];
            formula = questionInfo[2];
            lblMathQuestion.Text = questionInfo[0];
            lbl_Hint.Text = "";
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }




    }
}