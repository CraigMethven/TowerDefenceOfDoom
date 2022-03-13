using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace TowerDefence
{
    /*
     * The form for creating new levels
     */
    public partial class TDMapCreator : Form
    {
        public static int maxX = 9;
        public static int maxY = 7;
        private Square[,] mySquares;
        private Panel submitBtn;
        private Panel nameBacking;
        private TextBox mapName;
        private Label msgBox;
        private Panel msgBacking;
        private Panel exitBtn;
        public TDMapCreator()
        {
            InitializeComponent();
        }

        /*
         * initialiser creating all the objects used in the form and setting them up
         */
        private void TDMapCreater_Load(object sender, EventArgs e)
        {
            //General for the form
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"MapCreator/basePaper.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            int padding = 20;

            //For the exit button
            exitBtn = new Panel();
            int exitBtnSize = 50;
            exitBtn.BackColor = Color.Transparent;
            exitBtn.BackgroundImageLayout = ImageLayout.Stretch;
            exitBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"MapCreator/exitBtn.png");
            exitBtn.SetBounds(this.Width - exitBtnSize - padding,padding,exitBtnSize, exitBtnSize);
            Controls.Add(exitBtn);
            exitBtn.Click += new EventHandler(exitClick);

            //For the save button
            submitBtn = new Panel();
            submitBtn.BackColor = Color.Transparent;
            submitBtn.SetBounds(this.Width * 4 / 5, this.Height / 8 * 6, this.Width / 5 - padding * 2, this.Height / 8);
            submitBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"MapCreator/saveBtn.png");
            submitBtn.BackgroundImageLayout = ImageLayout.Stretch;
            submitBtn.Click += new EventHandler(saveClick);
            submitBtn.MouseEnter += new EventHandler(saveEnter);
            submitBtn.MouseLeave+= new EventHandler(saveLeave);
            Controls.Add(submitBtn);

            //For entering the name field
            mapName = new TextBox();
            mapName.BackColor = Color.White;
            mapName.ForeColor = Color.FromArgb(255, 113, 207, 248);
            mapName.Font = new Font(mainClass.font, this.Height / 24);
            mapName.SetBounds(padding*2 + this.Width * 4 / 5, this.Height / 8 * 5 - padding*3, this.Width / 5 - padding * 6, this.Height / 8 - padding*6);
            mapName.Text = "Map Name";
            mapName.BorderStyle = BorderStyle.None;
            Controls.Add(mapName);
            //
            nameBacking = new Panel();
            nameBacking.SetBounds(this.Width * 4 / 5, this.Height / 8 * 5 - padding * 4, this.Width / 5 - padding * 2, this.Height / 8 - padding);
            nameBacking.BackColor = Color.Transparent;
            nameBacking.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"MapCreator/nameBacking.png");
            nameBacking.BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(nameBacking);

            //For the messagebox
            msgBox = new Label();
            msgBox.BackColor = Color.Transparent;
            msgBox.ForeColor = Color.FromArgb(255, 113, 207, 248);
            msgBox.Font = new Font(mainClass.font, this.Height / 40);
            msgBox.SetBounds(padding * 2 + this.Width * 4 / 5, this.Height / 5 - padding*2, this.Width / 5 - padding * 6, this.Height / 3 - padding * 6);
            msgBox.BackgroundImageLayout = ImageLayout.Stretch;
            msgBox.Text = "Click on the squares to cycle through the different types";
            Controls.Add(msgBox);
            //
            msgBacking = new Panel();
            msgBacking.SetBounds(this.Width * 4 / 5, this.Height / 5 - padding * 6, this.Width / 5 - padding * 2, this.Height / 3 + padding*4);
            msgBacking.BackColor = Color.Transparent;
            msgBacking.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"MapCreator/msgBacking.png");
            msgBacking.BackgroundImageLayout = ImageLayout.Stretch;
            Controls.Add(msgBacking);


            //Getting panel dimensions
            int myWidth = this.Width * 4 / 5;
            int myHeight = this.Height - 40;
            int squareSize = 2;

            if (myWidth / maxX > myHeight / maxY)
            {
                squareSize = (myHeight / maxY) - 2;
            }
            else
            {
                squareSize = (myWidth / maxX) - 2;
            }
            
            int yBufferSize = (myHeight - (squareSize * maxY)) / 2;
            int xBufferSize = (myWidth - (squareSize * maxX)) / 2;

            Square.initialise(squareSize, xBufferSize, yBufferSize, this);

            //Initialising the map
            mySquares = new Square[maxX,maxY];
            for (int i = 0; i < maxX; i++)
            {
                for (int counter = 0; counter < maxY; counter++)
                {
                    mySquares[i,counter] = new Square(i, counter);
                }
            }
        }

        /*
         * Checks to see if the map currently on the screen is possible for the enemies to get to the end of
         */
        public bool checkMapPossible()
        {
            bool isStart = false;
            bool isEnd = false;

            for(int i = 0; i < maxX; i++)
            {
                for(int counter = 0; counter < maxY; counter++)
                {
                    string tempString = mySquares[i, counter].getType();
                    if (tempString == "S")
                    {
                        if(countSurroundingPaths(i,counter) != 1)
                        {
                            setMsgBox("Couldn't create the map as there's no where to go after the start or multiple paths");
                            return false;
                        }
                        isStart = true;
                    }
                    else if (tempString == "D")
                    {
                        if (countSurroundingPaths(i, counter) != 1)
                        {
                            setMsgBox("Couldn't create the map as there's multiple paths or no way to reach the end");
                            return false;
                        }
                        isEnd = true;
                    }
                    else if (tempString == "P")
                    {
                        if (countSurroundingPaths(i, counter) != 2)
                        {
                            setMsgBox("Couldn't create the map as there's multiple paths or no way to reach the end");
                            return false;
                        }
                    }
                }
            }

            if(isEnd && isStart)
            {
                return true;
            }
            setMsgBox("Couldn't create the map as there's either no start or end (or both)");
            return false;
        }

        /*
         * Counts and returns how many paths are surrounding the inputted cords
         */
        public int countSurroundingPaths(int myX, int myY)
        {
            int numPaths = 0;
            for(int i = -1; i <= 1; i++)
            {
                if(i!=0 && myX+i>=0 && myX+i < maxX)
                {
                    if(mySquares[myX + i, myY].getType() != "L")
                    {
                        numPaths++;
                    }
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                if (i != 0 && myY + i >= 0 && myY + i < maxY)
                {
                    if (mySquares[myX, myY + i].getType() != "L")
                    {
                        numPaths++;
                    }
                }
            }
            return numPaths;
        }

        /*
         * Sets a message in the message box
         */
        public void setMsgBox(string tempS)
        {
            msgBox.Text = tempS;
        }

        /*
         * Saves the current map to the file of the inputted name
         */
        public void saveToFile(String map)
        {
            string line;
            using StreamWriter file = new StreamWriter(Map.resourcesDirectory + @"Maps/"+map+".txt");
            for (int i = 0; i < maxY; i++)
            {

                line = mySquares[0,i].getType();

                for (int counter = 1; counter < maxX; counter++)
                {
                    line += ',' + mySquares[counter, i].getType();
                }

                file.WriteLine(line);
            }
            file.Close();
        }

        //EventHandlers:
        //When save button is clicked save if map is possible and the name is good
        public void saveClick(object sender, EventArgs e)
        {
            //if map is possible
            if (checkMapPossible())
            {
                //if name is good
                string map = mapName.Text;
                if(!string.IsNullOrWhiteSpace(map))
                {
                    if(!map.Contains(' ') && !map.Contains('.') && !map.Contains('/') && !map.Contains('\\') && !map.Contains(':') && !map.Contains('*') && !map.Contains('?') && !map.Contains('\"') && !map.Contains('<') && !map.Contains('>') && !map.Contains('|') && !map.Contains('\''))
                    {
                        saveToFile(map);
                        setMsgBox("Map Saved as: " + map);
                    }
                    else
                    {
                        setMsgBox("Please enter a map name that doesn't contain any of the following: space . < > ? \" \' \\ / : |");
                    }  
                }
                else
                {
                    setMsgBox("Please Enter a map name below");
                }
            }
        }

        public void saveEnter(object sender, EventArgs e)
        {
            submitBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"MapCreator/saveBtn2.png");
        }

        public void saveLeave(object sender, EventArgs e)
        {
            submitBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"MapCreator/saveBtn.png");
        }

        public void exitClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /*
         * The class used to store all the information about a square on the map
         */
        class Square{
            private Panel myPanel;
            private int myX;
            private int myY;
            private static Random rand;
            //This is either P (Path), L (Land), S (Starting Point) or D (Defend Point)
            private string myType = "L";
            //Statics
            private static List<Image> Land;
            private static Image Start = Image.FromFile(Map.resourcesDirectory + @"Backgrounds/S.png");
            private static List<Image> Path;
            private static Image Defend = Image.FromFile(Map.resourcesDirectory + @"Backgrounds/D.png");
            private static bool alreadyDefend = false;
            private static bool alreadyStart = false;
            private static int xBufferSize;
            private static int yBufferSize;
            private static int squareSize;
            public static Form myForm;

            public Square(int tempX, int tempY)
            {
                myX = tempX;
                myY = tempY;
                setUpPanel();
                changeType("L");
            }

            //Initialisng all the static variables, setting the image lists to all the images of path and land tiles
            public static void initialise(int tempSize, int xSize, int ySize, Form tempForm)
            {
                xBufferSize = xSize;
                yBufferSize = ySize;
                squareSize = tempSize;
                myForm = tempForm;
                rand = new Random();

                Path = new List<Image>();
                Land = new List<Image>();
                for (int i = 1; i <= 7; i++)
                {
                    Path.Add(Image.FromFile(Map.resourcesDirectory + @"Backgrounds\P" + i.ToString() + ".png"));
                    Land.Add(Image.FromFile(Map.resourcesDirectory + @"Backgrounds\L" + i.ToString() + ".png"));
                }
            }

            //Sets up my default panel
            public void setUpPanel()
            {
                myPanel = new Panel();
                myPanel.BackgroundImageLayout = ImageLayout.Stretch;
                myPanel.BackColor = Color.Transparent;
                myPanel.Size = new Size(squareSize, squareSize);
                myPanel.Location = new Point(xBufferSize + myX * squareSize, yBufferSize + myY * squareSize);
                myPanel.Click += new EventHandler(PanelClick);
                myForm.Controls.Add(myPanel);
            }

            //When the panel is clicked this cycles it round the different tile types
            public void PanelClick(object sender, EventArgs e)
            {
                switch (myType)
                {
                    case "P":
                        if (alreadyStart)
                        {
                            if (alreadyDefend)
                            {
                                changeType("L");
                            }
                            else
                            {
                                changeType("D");
                                alreadyDefend = true;
                            }
                        }
                        else
                        {
                            changeType("S");
                            alreadyStart = true;
                        }
                        break;
                    case "L":
                        changeType("P");
                        break;
                    case "D":
                        changeType("L");
                        alreadyDefend = false;
                        break;
                    case "S":
                        if (alreadyDefend)
                        {
                            changeType("L");
                        }
                        else
                        {
                            changeType("D");
                            alreadyDefend = true;
                            
                        }
                        alreadyStart = false;
                        break;
                };
            }
            //Changes this tiles type
            public void changeType(string input)
            {
                myType = input;
                switch(myType){
                    case "L":
                        myPanel.BackgroundImage = Land[rand.Next(0, Land.Count)];
                        break;
                    case "P":
                        myPanel.BackgroundImage = Path[rand.Next(0, Path.Count)];
                        break;
                    case "S":
                        myPanel.BackgroundImage = Start;
                        break;
                    case "D":
                        myPanel.BackgroundImage = Defend;
                        break;
                };
            }

            public string getType()
            {
                return myType;
            }
        }
    }
}
