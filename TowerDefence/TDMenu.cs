using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace TowerDefence
{
    /*
     * The form for the menu
     */
    public partial class TDMenu : Form
    {
        //For the general buttons
        Panel startPnl;
        Panel instructionsPnl;
        Panel makeLevelPnl;
        Panel exitPnl;
        Panel wholeScreen;

        //For the map select popup
        bool mpslctToggle = false;
        Panel mpslctBackground;
        Panel mpslctLeftarrow;
        Panel mpslctRightarrow;
        Panel mpslctSelect;
        Label curMap;
        string[] allMapsNoTxt;
        int mapSelected = 0;

        //For the level size popup
        bool lvlSizeToggle = false;
        Panel lvlSizeBackground;
        Panel lvlSizeLeftarrowX;
        Panel lvlSizeRightarrowX;
        Panel lvlSizeLeftarrowY;
        Panel lvlSizeRightarrowY;
        Panel lvlSizeSelect;
        Label curlvlSizeX;
        Label curlvlSizeY;
        int lvlSizeX = 7;
        int lvlSizeY = 6;

        public TDMenu()
        {
            InitializeComponent();
        }

        /*
         * initialiser, setting up the screen and objects to the default position and attributes and playing the song
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            //General for the form
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/menu.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;

            //Making a load screen while things are being created
            Panel ScreenCover = new Panel();
            ScreenCover.Size = this.Size;
            ScreenCover.Location = new Point(0, 0);
            ScreenCover.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"loading.png");
            ScreenCover.BackgroundImageLayout = ImageLayout.Stretch;
            this.Controls.Add(ScreenCover);

            //For selecting the map popup
            int padding = 30;
            mpslctBackground = setupButtons(this.Width/3,this.Height/7*4);
            mpslctBackground.Visible = false;
            mpslctBackground.Size = new Size(this.Width/3, this.Height / 3);
            mpslctBackground.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/mpslctBackground.png");

            int arrowSize = mpslctBackground.Height / 3 - padding;
            curMap = new Label();
            curMap.Visible = false;
            curMap.Size = new Size(mpslctBackground.Size.Width - (arrowSize+padding*2)*2, arrowSize);
            curMap.Location = new Point(mpslctBackground.Location.X+arrowSize+padding*2, mpslctBackground.Location.Y+arrowSize+padding);
            curMap.BackColor = Color.Transparent;
            curMap.Font = new Font(mainClass.font, arrowSize/2);
            curMap.ForeColor = Color.FromArgb(255, 113, 207, 248);
            curMap.TextAlign = ContentAlignment.MiddleCenter;

            mpslctLeftarrow = setupButtons(mpslctBackground.Location.X+padding*3, mpslctBackground.Location.Y + arrowSize+padding);
            mpslctLeftarrow.Size = new Size(arrowSize, arrowSize);
            mpslctLeftarrow.Visible = false;
            mpslctLeftarrow.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/arrowLeft.png");
            mpslctLeftarrow.Click += new EventHandler(mapMoveLeft);
            mpslctRightarrow = setupButtons(mpslctBackground.Location.X + mpslctBackground.Width - (arrowSize+padding*3), mpslctBackground.Location.Y + arrowSize + padding);
            mpslctRightarrow.Size = new Size(arrowSize, arrowSize);
            mpslctRightarrow.Visible = false;
            mpslctRightarrow.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/arrowRight.png");
            mpslctRightarrow.Click += new EventHandler(mapMoveRight);

            Controls.Add(curMap);

            mpslctSelect = setupButtons(mpslctBackground.Location.X + padding*4 + arrowSize, mpslctBackground.Location.Y +mpslctBackground.Height - (arrowSize + padding));
            mpslctSelect.Size = new Size(mpslctBackground.Width-((arrowSize+padding*4)*2),arrowSize - padding);
            mpslctSelect.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/mpslctPlay.png");
            mpslctSelect.Visible = false;
            mpslctSelect.MouseEnter += new EventHandler(mpslctHover);
            mpslctSelect.MouseLeave += new EventHandler(mpslctLeave);
            mpslctSelect.Click += new EventHandler(mpslctClick);

            //Making things appear in the correct order
            Controls.Add(mpslctBackground);

            //For changing the size of the level created popup
            lvlSizeBackground = setupButtons(this.Width / 3, this.Height / 7 * 4 - (arrowSize + padding));
            lvlSizeBackground.Visible = false;
            lvlSizeBackground.Size = new Size(this.Width / 3, this.Height / 3 + (arrowSize + padding));
            lvlSizeBackground.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/lvlSizeBackground.png");

            curlvlSizeX = new Label();
            curlvlSizeX.Visible = false;
            curlvlSizeX.Size = new Size(lvlSizeBackground.Size.Width - (arrowSize + padding * 2) * 2, arrowSize);
            curlvlSizeX.Location = new Point(lvlSizeBackground.Location.X + arrowSize + padding * 2, lvlSizeBackground.Location.Y + arrowSize + padding);
            curlvlSizeX.BackColor = Color.Transparent;
            curlvlSizeX.Font = new Font(mainClass.font, arrowSize / 2);
            curlvlSizeX.ForeColor = Color.FromArgb(255, 113, 207, 248);
            curlvlSizeX.TextAlign = ContentAlignment.MiddleCenter;

            curlvlSizeY = new Label();
            curlvlSizeY.Visible = false;
            curlvlSizeY.Size = new Size(lvlSizeBackground.Size.Width - (arrowSize + padding * 2) * 2, arrowSize);
            curlvlSizeY.Location = new Point(lvlSizeBackground.Location.X + arrowSize + padding * 2, lvlSizeBackground.Location.Y + (arrowSize + padding)*2);
            curlvlSizeY.BackColor = Color.Transparent;
            curlvlSizeY.Font = new Font(mainClass.font, arrowSize / 2);
            curlvlSizeY.ForeColor = Color.FromArgb(255, 113, 207, 248);
            curlvlSizeY.TextAlign = ContentAlignment.MiddleCenter;

            lvlSizeLeftarrowX = setupButtons(lvlSizeBackground.Location.X + padding * 3, lvlSizeBackground.Location.Y + arrowSize + padding);
            lvlSizeLeftarrowX.Size = new Size(arrowSize, arrowSize);
            lvlSizeLeftarrowX.Visible = false;
            lvlSizeLeftarrowX.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/arrowLeft.png");
            lvlSizeLeftarrowX.Click += new EventHandler(mapXLeft);
            lvlSizeRightarrowX = setupButtons(lvlSizeBackground.Location.X + lvlSizeBackground.Width - (arrowSize + padding * 3), lvlSizeBackground.Location.Y + arrowSize + padding);
            lvlSizeRightarrowX.Size = new Size(arrowSize, arrowSize);
            lvlSizeRightarrowX.Visible = false;
            lvlSizeRightarrowX.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/arrowRight.png");
            lvlSizeRightarrowX.Click += new EventHandler(mapXRight);

            lvlSizeLeftarrowY = setupButtons(lvlSizeBackground.Location.X + padding * 3, lvlSizeBackground.Location.Y + (arrowSize + padding)*2);
            lvlSizeLeftarrowY.Size = new Size(arrowSize, arrowSize);
            lvlSizeLeftarrowY.Visible = false;
            lvlSizeLeftarrowY.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/arrowLeft.png");
            lvlSizeLeftarrowY.Click += new EventHandler(mapYLeft);
            lvlSizeRightarrowY = setupButtons(lvlSizeBackground.Location.X + lvlSizeBackground.Width - (arrowSize + padding * 3), lvlSizeBackground.Location.Y + (arrowSize + padding) * 2);
            lvlSizeRightarrowY.Size = new Size(arrowSize, arrowSize);
            lvlSizeRightarrowY.Visible = false;
            lvlSizeRightarrowY.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/arrowRight.png");
            lvlSizeRightarrowY.Click += new EventHandler(mapYRight);

            Controls.Add(curlvlSizeX);
            Controls.Add(curlvlSizeY);

            lvlSizeSelect = setupButtons(lvlSizeBackground.Location.X + padding * 4 + arrowSize, lvlSizeBackground.Location.Y + lvlSizeBackground.Height - (arrowSize + padding));
            lvlSizeSelect.Size = new Size(lvlSizeBackground.Width - ((arrowSize + padding * 4) * 2), arrowSize - padding);
            lvlSizeSelect.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/createBtn.png");
            lvlSizeSelect.Visible = false;
            lvlSizeSelect.MouseEnter += new EventHandler(lvlSizeHover);
            lvlSizeSelect.MouseLeave += new EventHandler(lvlSizeLeave);
            lvlSizeSelect.Click += new EventHandler(lvlSizeClick);

            //Making things appear in the correct order
            Controls.Add(lvlSizeBackground);

            //For the instruction screen
            wholeScreen = setupButtons(0,0);
            wholeScreen.Size = this.Size;
            wholeScreen.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/instructions.png");
            wholeScreen.Visible = false;
            wholeScreen.Click += new EventHandler(closeInstructions);

            //For the 4 main buttons
            padding = (this.Height / 6) + 10;
            int myX = this.Width * 2 / 3;

            startPnl = setupButtons(myX, padding);
            startPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/srtBtn.png");
            startPnl.MouseEnter += new EventHandler(strtHover);
            startPnl.MouseLeave += new EventHandler(strtLeave);
            startPnl.Click += new EventHandler(strtClick);

            instructionsPnl = setupButtons(myX, padding*2);
            instructionsPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/instBtn.png");
            instructionsPnl.MouseEnter += new EventHandler(instHover);
            instructionsPnl.MouseLeave += new EventHandler(instLeave);
            instructionsPnl.Click += new EventHandler(instClick);

            makeLevelPnl = setupButtons(myX, padding * 3);
            makeLevelPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/lvlBtn.png");
            makeLevelPnl.MouseEnter += new EventHandler(lvlHover);
            makeLevelPnl.MouseLeave += new EventHandler(lvlLeave);
            makeLevelPnl.Click += new EventHandler(lvlClick);

            exitPnl = setupButtons(myX, padding * 4);
            exitPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/exitBtn.png");
            exitPnl.MouseEnter += new EventHandler(exitHover);
            exitPnl.MouseLeave += new EventHandler(exitLeave);
            exitPnl.Click += new EventHandler(exitClick);

            //remove load screen
            ScreenCover.Visible = false;

            //Playing the song
            System.Media.SoundPlayer song = new System.Media.SoundPlayer(Map.resourcesDirectory + @"song.wav");
            song.PlayLooping();
        }

        //Toggles the visibility of the Map Select popup
        public void toggleMpSlctVisibility()
        {
            mpslctToggle = !mpslctToggle;
            mpslctLeftarrow.Visible = mpslctToggle;
            mpslctRightarrow.Visible = mpslctToggle;
            mpslctSelect.Visible = mpslctToggle;
            curMap.Visible = mpslctToggle;
            mpslctBackground.Visible = mpslctToggle;
            if (mpslctToggle)
            {
                printMap();
            }   
        }

        //Toggles the visibility of the Map Creater size select popup
        public void toggleLvlSizeVisibility()
        {
            lvlSizeToggle = !lvlSizeToggle;
            lvlSizeLeftarrowX.Visible = lvlSizeToggle;
            lvlSizeLeftarrowY.Visible = lvlSizeToggle;
            lvlSizeRightarrowX.Visible = lvlSizeToggle;
            lvlSizeRightarrowY.Visible = lvlSizeToggle;
            lvlSizeSelect.Visible = lvlSizeToggle;
            curlvlSizeX.Visible = lvlSizeToggle;
            curlvlSizeY.Visible = lvlSizeToggle;
            lvlSizeBackground.Visible = lvlSizeToggle;
            if (lvlSizeToggle)
            {
                printXY();
            }   
        }
        public void printMap()
        {
            curMap.Text = allMapsNoTxt[mapSelected];
        }
        public void printXY()
        {
            curlvlSizeX.Text = lvlSizeX.ToString();
            curlvlSizeY.Text = lvlSizeY.ToString();
        }

        //Setting up some default buttons
        public Panel setupButtons(int myX, int myY)
        {
            Panel myPanel = new Panel();
            myPanel.BackColor = Color.Transparent;
            myPanel.Size = new Size(this.Width / 4, this.Height / 6);
            myPanel.Location = new Point(myX, myY);
            Controls.Add(myPanel);
            myPanel.BackgroundImageLayout = ImageLayout.Stretch;
            return myPanel;
        }

        /*
         * Button Actions
         */
        public void mapMoveRight(object sender, EventArgs e)
        {
            mapSelected++;
            if (mapSelected >= allMapsNoTxt.Length)
            {
                mapSelected = 0;
            }
            printMap();
        }
        public void mapMoveLeft(object sender, EventArgs e)
        {
            mapSelected--;
            if (mapSelected < 0)
            {
                mapSelected = allMapsNoTxt.Length - 1;
            }
            printMap();
        }
        public void mapXLeft(object sender, EventArgs e)
        {
            lvlSizeX--;
            if (lvlSizeX < 6)
            {
                lvlSizeX = 15;
            }
            printXY();
        }
        public void mapYLeft(object sender, EventArgs e)
        {
            lvlSizeY--;
            if (lvlSizeY < 5)
            {
                lvlSizeY = 12;
            }
            printXY();
        }
        public void mapXRight(object sender, EventArgs e)
        {
            lvlSizeX++;
            if (lvlSizeX > 16)
            {
                lvlSizeX = 6;
            }
            printXY();
        }
        public void mapYRight(object sender, EventArgs e)
        {
            lvlSizeY++;
            if (lvlSizeY > 12)
            {
                lvlSizeY = 5;
            }
            printXY();
        }
        public void closeInstructions(object sender, EventArgs e)
        {
            wholeScreen.Visible = false;
        }
        public void strtHover(object sender, EventArgs e)
        {
            startPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/srtBtn2.png");
        }
        public void strtLeave(object sender, EventArgs e)
        {
            startPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/srtBtn.png");
        }

        /*
         * Gets all current maps and saves them to the array and makes the map select pop up appear
         */
        public void strtClick(object sender, EventArgs e)
        {
            string[] allMaps = Directory.GetFiles(Map.resourcesDirectory + @"Maps/");
            int tempInt = allMaps.Length;
            allMapsNoTxt = new string[tempInt];
            for(int i = 0; i< tempInt; i++)
            {
                allMapsNoTxt[i] = allMaps[i].Split("Maps/")[1].Split('.')[0];
                //System.Diagnostics.Debug.WriteLine("Original " + allMaps[i]);
                //System.Diagnostics.Debug.WriteLine("My version " +allMapsNoTxt[i]);
            }
            mapSelected = 0;
            if (lvlSizeToggle)
            {
                toggleLvlSizeVisibility();
            }
            toggleMpSlctVisibility();
        }

        public void instHover(object sender, EventArgs e)
        {
            instructionsPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/instBtn2.png");
        }
        public void instLeave(object sender, EventArgs e)
        {
            instructionsPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/instBtn.png");
        }
        public void instClick(object sender, EventArgs e)
        {
            if (mpslctToggle)
            {
                toggleMpSlctVisibility();
            }
            wholeScreen.Visible = true;
        }

        public void lvlHover(object sender, EventArgs e)
        {
            makeLevelPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/lvlBtn2.png");
        }
        public void lvlLeave(object sender, EventArgs e)
        {
            makeLevelPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/lvlBtn.png");
        }
        public void lvlClick(object sender, EventArgs e)
        {
            if (mpslctToggle)
            {
                toggleMpSlctVisibility();
            }
            toggleLvlSizeVisibility();
            
            //TDMapCreater potato = new TDMapCreater();
            //potato.Show();
        }

        public void exitHover(object sender, EventArgs e)
        {
            exitPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/exitBtn2.png");
        }
        public void exitLeave(object sender, EventArgs e)
        {
            exitPnl.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/exitBtn.png");
        }
        public void exitClick(object sender, EventArgs e)
        {
            this.Close();
        }

        public void mpslctHover(object sender, EventArgs e)
        {
            mpslctSelect.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/mpslctPlay2.png");
        }
        public void mpslctLeave(object sender, EventArgs e)
        {
            mpslctSelect.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/mpslctPlay.png");
        }
        /*
         * Creates the instance of the game from the map that was selected
         */
        public void mpslctClick(object sender, EventArgs e)
        {
            toggleMpSlctVisibility();
            Map.MapName = allMapsNoTxt[mapSelected];
            TDGame potato = new TDGame();
            potato.Show();
        }
        public void lvlSizeHover(object sender, EventArgs e)
        {
            lvlSizeSelect.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/createBtn2.png");
        }
        public void lvlSizeLeave(object sender, EventArgs e)
        {
            lvlSizeSelect.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Menu/createBtn.png");
        }
        /*
         * Creates the instance of the map creator from the sizes selected
         */
        public void lvlSizeClick(object sender, EventArgs e)
        {
            toggleLvlSizeVisibility();
            TDMapCreator.maxX = lvlSizeX;
            TDMapCreator.maxY = lvlSizeY;
            TDMapCreator potato = new TDMapCreator();
            potato.Show();
        }
    }
}
