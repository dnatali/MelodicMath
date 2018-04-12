using System;

using UIKit;
using CoreGraphics;

using System.Collections.Generic;
using Foundation;
using System.IO;

namespace MelodicMath
{
    public partial class ViewController : UIViewController
    {
        private string currentNoteVal = "1";
        private string currentNoteTone = "C";
        private string currentOctave = "0";
        private string currentMelody = "";
        private int currentTempo = 60;

        //this creates the music staff and notes
        private StaffViewer layout;

        private MidiPlayer player = new MidiPlayer();

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

          
            View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile("BackMain.jpg"));

            //--------Staff set up---------

            //notePositions will tell us where to draw staff in case sizes, positions change, noteHieght lets us find mid point
            int[] notePositions = { (int)Btn_FP1.Frame.Y, (int)Btn_D.Frame.Y , (int)Btn_B.Frame.Y , (int)Btn_G.Frame.Y , (int)Btn_E.Frame.Y };
            int noteHeight = (int)Btn_FP1.Frame.Height;
            int staffOffset = (int)(Btn_FP1.Frame.X * .9);

            //start of staff will be based on second E + width and will go down 90% from top, end points will be entire screen minus offsets and bottom E note
            layout = new StaffViewer(new CGRect(Btn_EP1.Frame.X + Btn_EP1.Frame.Width, (int)(Btn_FP1.Frame.X * .9), UIScreen.MainScreen.Bounds.Width - Btn_FP1.Frame.X - (Btn_EP1.Frame.X + Btn_EP1.Frame.Width), Btn_E.Frame.Y + Btn_E.Frame.Height), notePositions, noteHeight, staffOffset);

            //-----Staff set up end--------

            //-----Handle stepper for tempo---------------
            stepper_tempo.ValueChanged += (object sender, EventArgs e) =>
            {
                txtTempo.Text = stepper_tempo.Value.ToString();
                currentTempo = Convert.ToInt32(stepper_tempo.Value.ToString());
            };

            txtTempo.ShouldChangeCharacters += (UITextField textfield, NSRange range, string replacement) =>
            {
                string combine = textfield.Text + replacement;

                if (combine.Equals(""))
                    combine = "0";

                double newVal = Convert.ToDouble(combine);

                if (newVal >= 30.0 && newVal <= 180.0)
                    stepper_tempo.Value = newVal;
                else if (newVal < 30.0)
                    stepper_tempo.Value = 30.0;
                else
                    stepper_tempo.Value = 180.0;

                currentTempo = Convert.ToInt32(stepper_tempo.Value.ToString());

                return true;
            };

            //-----Handle Button Presses--------

            Btn_Math.TouchUpInside += (object sender, EventArgs e) =>
            {
             };

            //---------Save/Load file-------
            btn_Save.TouchUpInside += (object sender, EventArgs e) =>
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filename = Path.Combine(documents, txt_name.Text);
                File.WriteAllText(filename, currentMelody + ":" + currentTempo);
            };

            btn_Load.TouchUpInside += (object sender, EventArgs e) =>
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filename = Path.Combine(documents, txt_Load.Text);
                string[] splitData = { ":" };
                string [] txtData = File.ReadAllText(filename).Split(splitData,StringSplitOptions.RemoveEmptyEntries);
                currentMelody = txtData[0];
                currentTempo = Convert.ToInt32(txtData[1]);
                stepper_tempo.Value = currentTempo;
                txtTempo.Text = currentTempo.ToString();
                layout.SetMelody(currentMelody);
                layout.SetNeedsDisplay();
            };
            //-------end file save/load----------

            //----Play/Remove buttons---------
            PlayButton.TouchUpInside += (object sender, EventArgs e) =>
            {
                //MidiPlayer player = new MidiPlayer();
                //player.play("C|1|1,A|1|0,C|.25|1,A|.25|0,C|.25|1,A|.25|0", 60);
                player.play(currentMelody, currentTempo);
            };

            Btn_Remove.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("", "", "R");
            };
            //-----end Play/Remove Buttons------

            //-----Instrument buttons------------
            Btn_Piano.TouchUpInside += (object sender, EventArgs e) =>
            {
                player.SetInstrument("piano");
            };

            Btn_Choir.TouchUpInside += (object sender, EventArgs e) =>
            {
                player.SetInstrument("voice");
            };

            Btn_Guitar.TouchUpInside += (object sender, EventArgs e) =>
            {
                player.SetInstrument("guitar");
            };

            Btn_Bass.TouchUpInside += (object sender, EventArgs e) =>
            {
                player.SetInstrument("bass");
            };
            //----end instrument buttons----------


            //---Note Value Buttons---------
            Btn_NoteWhole.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageNoteButtons(Btn_NoteWhole);
            };

            Btn_Half.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageNoteButtons(Btn_Half);
            };

            Btn_Quarter.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageNoteButtons(Btn_Quarter);
            };

            Btn_Eighth.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageNoteButtons(Btn_Eighth);
            };

            Btn_Sixteen.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageNoteButtons(Btn_Sixteen);
            };
            //----end note value buttons------

            //----Note Pitch Buttons---------
            Btn_D0.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("D", "0", "A");
            };

            Btn_E.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("E", "0", "A");
            };

            Btn_F.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("F", "0", "A");
            };

            Btn_G.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("G", "0", "A");
            };

            Btn_A.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("A", "0", "A");
            };

            Btn_B.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("B", "0", "A");
            };

            Btn_C.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("C", "1", "A");
            };

            Btn_D.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("D", "1", "A");
            };

            Btn_EP1.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("E", "1", "A");
            };

            Btn_FP1.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("F", "1", "A");
            };

            Btn_G1.TouchUpInside += (object sender, EventArgs e) =>
            {
                ManageMelody("G", "1", "A");
            };
            //----end note pitch buttons--------

            //------end Handle Button presses-----------

            //add the view to the main view
            //var layout = new StaffViewer(new CGRect(100, 50, UIScreen.MainScreen.Bounds.Width - 100, UIScreen.MainScreen.Bounds.Height - 400));
            //layout.BackgroundColor = UIColor.Red;
            View.AddSubview(layout);

        }

        //when user picks note add it to medoly and repaint staff
        private void ManageMelody(string note, string octave, string action)
        {
            //"C|1|1,A|1|0,C|.25|1,A|.25|0,C|.25|1,A|.25|0", 60
            currentNoteTone = note;
            currentOctave = octave;

            //add new note, or remove last note
            if (action.Equals("A"))
            {
                string addNoteInfo = currentNoteTone + "|" + currentNoteVal + "|" + currentOctave + ",";

                currentMelody += addNoteInfo;
            }
            else{
                int lastNote = currentMelody.LastIndexOf(",", StringComparison.Ordinal);
                currentMelody = currentMelody.Substring(0, lastNote);
                lastNote = currentMelody.LastIndexOf(",", StringComparison.Ordinal);
                if (lastNote > 0)
                    currentMelody = currentMelody.Substring(0, lastNote) + ",";
                else{
                    currentMelody = ",";   
                }
            }

            //make staff redraw itself, set melody should add note
            layout.SetMelody(currentMelody);
            layout.SetNeedsDisplay();
        }

        //when button click change appearance so user knows what's been clicked
        private void ManageNoteButtons(UIKit.UIButton btn)
        {
            if(btn == Btn_NoteWhole){
                currentNoteVal = "4";
                Btn_NoteWhole.Layer.BorderWidth = 3;
                Btn_NoteWhole.Layer.CornerRadius = 4;
                Btn_NoteWhole.Layer.BorderColor = UIColor.Red.CGColor;
            }
            else{
                Btn_NoteWhole.Layer.BorderWidth = 0;
                Btn_NoteWhole.Layer.CornerRadius = 0;
            }

            if (btn == Btn_Half)
            {
                currentNoteVal = "2";
                Btn_Half.Layer.BorderWidth = 3;
                Btn_Half.Layer.CornerRadius = 4;
                Btn_Half.Layer.BorderColor = UIColor.Red.CGColor;
            }
            else
            {
                Btn_Half.Layer.BorderWidth = 0;
                Btn_Half.Layer.CornerRadius = 0;
            }

            if (btn == Btn_Quarter)
            {
                currentNoteVal = "1";
                Btn_Quarter.Layer.BorderWidth = 3;
                Btn_Quarter.Layer.CornerRadius = 4;
                Btn_Quarter.Layer.BorderColor = UIColor.Red.CGColor;
            }
            else
            {
                Btn_Quarter.Layer.BorderWidth = 0;
                Btn_Quarter.Layer.CornerRadius = 0;
            }

            if (btn == Btn_Eighth)
            {
                currentNoteVal = ".5";
                Btn_Eighth.Layer.BorderWidth = 3;
                Btn_Eighth.Layer.CornerRadius = 4;
                Btn_Eighth.Layer.BorderColor = UIColor.Red.CGColor;
            }
            else
            {
                Btn_Eighth.Layer.BorderWidth = 0;
                Btn_Eighth.Layer.CornerRadius = 0;
            }

            if (btn == Btn_Sixteen)
            {
                currentNoteVal = ".25";
                Btn_Sixteen.Layer.BorderWidth = 3;
                Btn_Sixteen.Layer.CornerRadius = 4;
                Btn_Sixteen.Layer.BorderColor = UIColor.Red.CGColor;
            }
            else
            {
                Btn_Sixteen.Layer.BorderWidth = 0;
                Btn_Sixteen.Layer.CornerRadius = 0;
            }

        }


      /*  public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //var button = UIButton.FromType(UIButtonType.System); // New type in iOS 7
            Btn_NoteWhole.Layer.BorderWidth = 1;
            Btn_NoteWhole.Layer.CornerRadius = 4;
            Btn_NoteWhole.Layer.BorderColor = UIColor.Black.CGColor;
        }
    */
        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            // set the View Controller that’s powering the screen we’re
            // transitioning to

            var mathViewController = segue.DestinationViewController as MathViewController;

            //set the Table View Controller’s list of phone numbers to the
            // list of dialed phone numbers

            List<string> MelInfo = new List<string>();
            MelInfo.Add(currentMelody);
            MelInfo.Add(currentTempo.ToString());

            if (mathViewController != null)
            {
                mathViewController.MelodyInfo = MelInfo;
            }
        }
    }
}
