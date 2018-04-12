using System;
using System.Collections.Generic;
using System.Data;

namespace MelodicMath
{
    public class MathGenerator
    {
        int tempo = 0;
        int wholeNotes = 0;
        int halfNotes = 0;
        int quarterNotes = 0;
        int eighthNotes = 0;
        int sixteenthNotes = 0;
        string firstNote = "";
        string firstNoteOctave = "";
        string lastNote = "";
        string lastNoteOctave = "";

        List<int> NoteCounts = new List<int>();
        List<string> NoteNames = new List<string>();

        List<string> QuestionSet = new List<string>();

        public MathGenerator()
        {
            Setup();
        }

        private void Setup()
        {
            //load the questions
            QuestionSet.Add("C:How many [note1] notes are in your melody?:A:[note1Count]");
            //QuestionSet.Add("CS:What is the letter name that is [x] [lineSpaces] [upDown] the [position] note?");
            QuestionSet.Add("A:Your melody currently has [note1Count] [note1] notes, if you add [y] more [note1] notes how many would you have?:E:[note1Count] + [y]");
            QuestionSet.Add("S:Your melody currently has [note1Count] [note1] notes, if you subtract [y] [note1] notes how many would you have?:E:[note1Count] - [y]");
            QuestionSet.Add("S:Your melody has [Total] notes, if you subtract [y] notes how many would you have left?:E:[Total] - [y]");
            QuestionSet.Add("A:Your melody has [Total] notes, if you add [y] notes how many would you have?:E:[Total] + [y]");
            QuestionSet.Add("A:Your melody has [Total] notes, if you want it to have [y] notes, how many would you have to add?:E:[y] - [Total]");
            QuestionSet.Add("S:Your melody has [Total] notes, if you want it to have [y] notes, how many would you have to substract?:E:[Total] - [y]");
            QuestionSet.Add("G:True of False | Your melody contains more [note1] notes than [note2] notes.:TF:[TF]");

            NoteNames.Add("Whole");
            NoteNames.Add("Half");
            NoteNames.Add("Quarter");
            NoteNames.Add("Eighth");
            NoteNames.Add("Sixteenth");

            NoteCounts.Add(0);//whole is 0
            NoteCounts.Add(0);//half is 1
            NoteCounts.Add(0);//quarter is 2
            NoteCounts.Add(0);//eighth is 3
            NoteCounts.Add(0);//sixteenth is 4
        }

        public List<string> Generate(List<string> melody)
        {
            DataTable dt = new DataTable();//using this to calculate string expression
            List<string> QuestionInfo = new List<string>();
            string question = "";
            string qType = "";
            string qExecute = "";
            string answer = "";
            string formula = "";
            int note1Count = 0;
            int note2Count = 0;
            int Total = 0;

            //----Set up the melody-------------
            //example: "C|1|1,A|1|0,C|.25|1,A|.25|0,C|.25|1,A|.25|0"
            tempo = Convert.ToInt32(melody[1]);
            string[] splitMelody = { "," };
            string[] melodyInfo = melody[0].Split(splitMelody, StringSplitOptions.RemoveEmptyEntries);

            foreach (string noteInfo in melodyInfo)
            {
                string[] splitNote = { "|" };
                string[] note = noteInfo.Split(splitNote, StringSplitOptions.RemoveEmptyEntries);

                if (firstNote.Equals(""))
                {
                    firstNote = note[0];
                    firstNoteOctave = note[2];
                }

                lastNote = note[0];
                lastNoteOctave = note[2];

                if (note[1].Equals("4"))
                    NoteCounts[0] = ++wholeNotes;
                else if (note[1].Equals("2"))
                    NoteCounts[1] = ++halfNotes;
                else if (note[1].Equals("1"))
                    NoteCounts[2] = ++quarterNotes;
                else if (note[1].Equals(".5"))
                    NoteCounts[3] = ++eighthNotes;
                else
                    NoteCounts[4] = ++sixteenthNotes;

            }
            //---end melody set up------------

            Random random = new Random();
            int qNum = random.Next(QuestionSet.Count);

            string[] split = { ":" };
            string[] qInfo = QuestionSet[qNum].Split(split, StringSplitOptions.RemoveEmptyEntries);

            qType = qInfo[0];
            question = qInfo[1];
            qExecute = qInfo[2];
            answer = qInfo[3];


            if(question.Contains("[note1]"))
            {
                int rnd = random.Next(NoteNames.Count);
                question = question.Replace("[note1]", NoteNames[rnd]);
                question = question.Replace("[note1Count]", NoteCounts[rnd].ToString());
                answer = answer.Replace("[note1Count]", NoteCounts[rnd].ToString());
                note1Count = NoteCounts[rnd];
            }

            if (question.Contains("[note2]"))
            {
                int rnd = random.Next(NoteNames.Count);
                question = question.Replace("[note2]", NoteNames[rnd]);
                question = question.Replace("[note2Count]", NoteCounts[rnd].ToString());
                answer = answer.Replace("[note2Count]", NoteCounts[rnd].ToString());
                note2Count = NoteCounts[rnd];
            }

            if (question.Contains("[Total]"))
            {
                Total = NoteCounts[0] + NoteCounts[1] + NoteCounts[2] + NoteCounts[3] + NoteCounts[4];
                question = question.Replace("[Total]", Total.ToString());
                answer = answer.Replace("[Total]", Total.ToString());
            }

            if (question.Contains("[y]"))
            {
                string rnd = "";
                int max = (Total < note1Count) ? note1Count : Total;
                if(qType.Equals("S"))
                    rnd = random.Next(max).ToString();
                else
                    rnd = random.Next(20).ToString();
                
                question = question.Replace("[y]", rnd);
                answer = answer.Replace("[y]", rnd);
            }

            if(qExecute.Equals("TF"))
            {
                if (note1Count > note2Count)
                    answer = "T";
                else
                    answer = "F";
            }

            //certain questions need executed
            if (qExecute.Equals("E"))
            {
                formula = answer;
                answer = ((int)dt.Compute(answer, "")).ToString();
            }
            else{
                formula = "No hint.";
            }

            QuestionInfo.Add(question);
            QuestionInfo.Add(answer);
            QuestionInfo.Add(formula);

            return QuestionInfo;
        }
    }
}
