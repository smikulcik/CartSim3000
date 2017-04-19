using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CartSim3000
{
    public class ScoreboardEntry : IComparable<ScoreboardEntry>
    {
        string name;
        int score;

        public ScoreboardEntry()
        {
            name = "Unnamed";
            score = 0;
        }
        public ScoreboardEntry(string name, int score)
        {
            this.name = name;
            this.score = score;
            
        }

        public int CompareTo(ScoreboardEntry other)
        {
            return other.Value.CompareTo(Value);
        }

        public int Value
        {
            get { return score; }
            set { score = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
    public class Scoreboard
    {
        int size = 0;
        List<ScoreboardEntry> scores;

        public Scoreboard()
        {
            this.size = 10;
            scores = new List<ScoreboardEntry>();
        }
        /// <summary>
        /// Initialize a new Scoreboard
        /// </summary>
        /// <param name="size">number of entries in scoreboard</param>
        public Scoreboard(int size)
        {
            this.size = size;
            scores = new List<ScoreboardEntry>();
        }

        public void LoadFromXML(string filename)
        {
            TextReader reader = new StreamReader(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(Scoreboard));
            Scoreboard loaded = (Scoreboard)serializer.Deserialize(reader);
            reader.Close();
            this.Size = loaded.Size;
            this.Scores = loaded.Scores;
        }
        public void SaveToXML(string filename)
        {
            Type t = GetType();
            XmlSerializer serializer = new XmlSerializer(t);
            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, this);
            writer.Close();
        }
        public void addScore(ScoreboardEntry newScore)
        {
            scores.Add(newScore);
            repositionScores();
        }

        /// <summary>
        /// Sorts scores and removes extras so that there are
        /// no more than size score entries
        /// </summary>
        private void repositionScores()
        {
            scores.Sort();
            while(scores.Count > size)
            {
                scores.RemoveAt(scores.Count - 1);
            }
        }
        public List<ScoreboardEntry> Scores
        {
            get { return scores; }
            set
            {
                scores = value;
                repositionScores();
            }
        }
        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                repositionScores();
            }
        }
        public int LowestScore
        {
            get
            {
                if(scores.Count > 0)
                {
                    repositionScores();
                    return scores[scores.Count - 1].Value;
                }
                else
                {
                    return -1;
                }
            }
        }
    }

    class ScoreboardTester
    {
        public static void test()
        {
            Scoreboard s = new Scoreboard(5);

            if(s.Size != 5)
            {
                throw new Exception("size is wrong");
            }
            s.addScore(new ScoreboardEntry("10", 10));
            s.addScore(new ScoreboardEntry("1", 1));
            s.addScore(new ScoreboardEntry("9", 9));
            s.addScore(new ScoreboardEntry("8", 8));
            s.addScore(new ScoreboardEntry("2", 2));
            s.addScore(new ScoreboardEntry("7", 7));
            s.addScore(new ScoreboardEntry("6", 6));
            s.addScore(new ScoreboardEntry("3", 3));
            s.addScore(new ScoreboardEntry("5", 5));
            s.addScore(new ScoreboardEntry("4", 4));

            if(s.Scores.Count != 5)
            {
                throw new Exception("Wrong number of scores" + s.Scores.Count + " != " + 5);
            }

            for(int i=0; i<5; i++)
            {
                if(s.Scores[i].Value != 10 - i)
                {
                    throw new Exception("Incorrect score value for '" + s.Scores[i].Name + "': " + s.Scores[i].Value + " should be " + (10 - i));
                }
            }

            //test serializer
            s.SaveToXML("scoreboard.xml");
            Scoreboard t = new Scoreboard();
            t.LoadFromXML("scoreboard.xml");

            if (t.Scores.Count != 5)
            {
                throw new Exception("Wrong number of scores" + t.Scores.Count + " != " + 5);
            }
            for (int i = 0; i < 5; i++)
            {
                if (t.Scores[i].Value != 10 - i)
                {
                    throw new Exception("Incorrect score value for '" + t.Scores[i].Name + "': " + t.Scores[i].Value + " should be " + (10 - i));
                }
            }

        }
    }
}
