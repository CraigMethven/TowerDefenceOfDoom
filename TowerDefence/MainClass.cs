using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace TowerDefence
{
    /*
     * Class that holds all the component of the game and runs the main loop
     */
    class mainClass
    {
        private Form myForm;
        private Map myMap;

        //Objects for the sidebar
        private Panel sidebar;
        private Label moneyLbl;
        private Label hpLbl;
        private System.Windows.Forms.Timer myTimer;
        private Panel laserBtn;
        private Panel cannonBtn;
        private Panel archerBtn;
        private Panel babyBtn;
        private Panel wizardBtn;
        private Panel batteryBtn;
        private Panel ScreenCover;

        //For the message box
        private Panel popupBox;
        private Label popupTitle;
        private Label popupText;
        private System.Windows.Forms.Timer popupTimer;
        
        private int money = 30;
        private int hp = 20;

        //To keep track of how long the game has went on for (in ticks, a 60th of a second)
        private long ticksElapsed = 0;

        private int fontSize;
        public static string font = "Tw Cen MT";

        private System.Windows.Forms.Timer endingTimer;

        //For the main loops logic
        //Stores an instance of each tower type
        private List<ITower> towerTypes;
        //A queue of what enemies are to spawn
        private Queue<enemySpawn> enemySpawnOrder;
        //The place in the list towerTyoes that the currently selected tower is in
        private int towerTypeSelected = -1;

        //Constructor to get the form
        public mainClass(Form tempForm)
        {
            myForm = tempForm;
            initialise();
        }

        //Main loop for the program being looped through a C# Timer
        public void mainLoop(object sender, EventArgs e)
        {
            myTimer.Enabled = false;
            ticksElapsed++;
            //Fires all towers on the map
            myMap.turretsFire();
            //spawns new enemies every half a second
            if(ticksElapsed % 30 == 0)
            {
                enemyMechanic();
            }
            //starts the loop again
            myTimer.Enabled = true;
        }

        //Moving the enemies around the map and adding the new ones
        public void enemyMechanic()
        {
            //If there's more enemies to spawn them spawn them
            if(enemySpawnOrder.Count > 0)
            {
                enemySpawn tempSpawn = enemySpawnOrder.Dequeue();
                myMap.moveEnemies(tempSpawn.getReg(), tempSpawn.getStr());
            }
            //If there's no enemies left then see if the game is ended, if not then just move the enemies along
            else
            {
                if (myMap.checkGameEnded())
                {
                    endGame(true);
                }
                myMap.moveEnemies(0, 0);
            }
        }

        //displays the end screen (either win or lose)
        public void endGame(bool win)
        {
            ScreenCover.Visible = true;
            
            if (win)
            {
                ScreenCover.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"winScreen.png");
            }
            else
            {
                ScreenCover.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"loseScreen.png");
            }
            myTimer.Enabled = false;
            enemySpawnOrder.Clear();
            endingTimer = new System.Windows.Forms.Timer();
            endingTimer.Interval = 10000;
            endingTimer.Tick += new EventHandler(closeGame);
            endingTimer.Start();
            
        }

        public void closeGame(object sender, EventArgs e)
        {
            endingTimer.Stop();
            myForm.Close();
        }

        //Takes in the input file of enemies that are spawning
        public void setUpEnemyLine()
        {
            enemySpawnOrder = new Queue<enemySpawn>();
            string[] enemyFile = System.IO.File.ReadAllLines(Map.resourcesDirectory + @"enemySpawns.txt");
            int fileLength = enemyFile.Length;

            //initialise variables used in the loop
            string[] splitLine;
            //For all lines of the file
            for (int i = 0; i < fileLength; i++)
            {
                //Split up the x squares
                splitLine = enemyFile[i].Split(' ');

                enemySpawnOrder.Enqueue(new enemySpawn(Int32.Parse(splitLine[0]), Int32.Parse(splitLine[1])));
            }
        }

        /*
         * Sets up all objects the way they should be by default
         */
        public void initialise()
        {
            //Initialise another class's variable
            MapSquare.setForm(myForm, this);
            //set full screen
            fullScreen();

            int padding = 20;

            //Making a load screen while things are being created
            ScreenCover = new Panel();
            ScreenCover.Size = myForm.Size;
            ScreenCover.Location = new Point(0, 0);
            ScreenCover.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"loading.png");
            ScreenCover.BackgroundImageLayout = ImageLayout.Stretch;
            myForm.Controls.Add(ScreenCover);
            ScreenCover.Visible = true;

            //initialise the enemy list
            setUpEnemyLine();

            //setting up the popup box
            popupTimer = new System.Windows.Forms.Timer();
            popupTimer.Tick += new EventHandler(popupTimerMethod);
            int popupX = myForm.Size.Width / 7;
            int popupY = popupX * 2 / 3;

            popupBox = new Panel();
            popupBox.Visible = false;
            popupBox.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"popupbox.png");
            popupBox.BackgroundImageLayout = ImageLayout.Stretch;
            popupBox.Size = new Size(popupX, popupY);
            popupBox.Location = new Point(padding, myForm.Size.Height - padding * 5 - popupY);

            popupText = new Label();
            popupText.Visible = false;
            popupText.Font = new Font(font, popupY / 10);
            popupText.BackColor = Color.Transparent;
            popupText.ForeColor = Color.FromArgb(255,113,207,248);
            popupText.TextAlign = ContentAlignment.MiddleCenter;
            popupText.Size = new Size(popupX - padding * 4, popupY / 2);
            popupText.Location = new Point(padding*3, popupBox.Height / 3 + popupBox.Location.Y + padding);

            popupTitle = new Label();
            popupTitle.Visible = false;
            popupTitle.Font = new Font(font, popupY / 6);
            popupTitle.BackColor = Color.Transparent;
            popupTitle.ForeColor = Color.FromArgb(255, 113, 207, 248);
            popupTitle.TextAlign = ContentAlignment.MiddleCenter;
            popupTitle.Size = new Size(popupX - padding * 4, popupY / 4);
            popupTitle.Location = new Point(padding*3, popupBox.Location.Y+ padding);

            myForm.Controls.Add(popupText);
            myForm.Controls.Add(popupTitle);
            myForm.Controls.Add(popupBox);

            myMap = new Map(this);
            Tower.setMap(myMap);
            int tempInt;

            //Setting HP and money lbl
            moneyLbl = new Label();
            hpLbl = new Label();
            tempInt = myForm.Size.Height * 3;
            tempInt /= 40;
            fontSize = tempInt - (padding*3/2);
            moneyLbl.Size = new Size((myForm.Size.Width / 5) - (padding * 8), tempInt);
            hpLbl.Size = new Size((myForm.Size.Width / 5) - (padding * 8), tempInt);
            tempInt = myForm.Size.Height * 25;
            tempInt /= 40;
            int tempX = myForm.Size.Width * 4;
            tempX /= 5;
            tempX += padding * 4;
            moneyLbl.Location = new Point(tempX, tempInt);
            tempInt = myForm.Size.Height * 33;
            tempInt /= 40;
            //tempX += 30;
            hpLbl.Location = new Point(tempX, tempInt);

            moneyLbl.BackColor = Color.Transparent;
            myForm.Controls.Add(moneyLbl);
            moneyLbl.Visible = true;
            hpLbl.BackColor = Color.Transparent;
            myForm.Controls.Add(hpLbl);
            hpLbl.Visible = true;

            moneyLbl.Font = new Font(font, fontSize);
            moneyLbl.ForeColor = Color.Gold;
            moneyLbl.TextAlign = ContentAlignment.MiddleCenter;
            hpLbl.Font = new Font(font, fontSize);
            hpLbl.ForeColor = Color.Red;
            hpLbl.TextAlign = ContentAlignment.MiddleCenter;
            printMoneyHP();

            //Setting up the tower buttons
            towerTypes = new List<ITower>();
            ITower temp = new Laser();
            towerTypes.Add(temp);
            temp = new Cannon();
            towerTypes.Add(temp);
            temp = new ArcherCircle();
            towerTypes.Add(temp);
            temp = new Baby();
            towerTypes.Add(temp);
            temp = new Wizard();
            towerTypes.Add(temp);
            temp = new FaultyBattery();
            towerTypes.Add(temp);

            int buttonSize = myForm.Size.Height / 8 - (padding);
            int column1 = (myForm.Size.Width * 5) / 6 + padding * 2;
            int column2 = column1 + padding + buttonSize;
            int row1 = (myForm.Size.Height * 3) / 40 + padding;
            int row2 = row1 + padding * 2 + buttonSize;
            int row3 = row2 + padding * 2 + buttonSize;

            //Laser
            laserBtn = new Panel();
            laserBtn.SetBounds(column1, row1, buttonSize, buttonSize);
            laserBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Turrets/laser.png");
            myForm.Controls.Add(setDefaults(laserBtn));
            laserBtn.Click += new EventHandler(laserClicked);
            laserBtn.MouseEnter += new EventHandler(laserHover);
            laserBtn.MouseLeave += new EventHandler(laserLeave);
            //Cannon
            cannonBtn = new Panel();
            cannonBtn.SetBounds(column2, row1, buttonSize, buttonSize);
            cannonBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Turrets/cannon.png");
            myForm.Controls.Add(setDefaults(cannonBtn));
            cannonBtn.Click += new EventHandler(cannonClicked);
            cannonBtn.MouseEnter += new EventHandler(cannonHover);
            cannonBtn.MouseLeave += new EventHandler(cannonLeave);
            //Archer Circle
            archerBtn = new Panel();
            archerBtn.SetBounds(column1, row2, buttonSize, buttonSize);
            archerBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Turrets/archerCircle.png");
            myForm.Controls.Add(setDefaults(archerBtn));
            archerBtn.Click += new EventHandler(archerClicked);
            archerBtn.MouseEnter += new EventHandler(archerHover);
            archerBtn.MouseLeave += new EventHandler(archerLeave);
            //Baby
            babyBtn = new Panel();
            babyBtn.SetBounds(column2, row2, buttonSize, buttonSize);
            babyBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Turrets/baby.png");
            myForm.Controls.Add(setDefaults(babyBtn));
            babyBtn.Click += new EventHandler(babyClicked);
            babyBtn.MouseEnter += new EventHandler(babyHover);
            babyBtn.MouseLeave += new EventHandler(babyLeave);
            //Wizard
            wizardBtn = new Panel();
            wizardBtn.SetBounds(column1, row3, buttonSize, buttonSize);
            wizardBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Turrets/wizard.png");
            myForm.Controls.Add(setDefaults(wizardBtn));
            wizardBtn.Click += new EventHandler(wizardClicked);
            wizardBtn.MouseEnter += new EventHandler(wizardHover);
            wizardBtn.MouseLeave += new EventHandler(wizardLeave);
            //Faulty Battery
            batteryBtn = new Panel();
            batteryBtn.SetBounds(column2, row3, buttonSize, buttonSize);
            batteryBtn.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Turrets/faultyBattery.png");
            myForm.Controls.Add(setDefaults(batteryBtn));
            batteryBtn.Click += new EventHandler(batteryClicked);
            batteryBtn.MouseEnter += new EventHandler(batteryHover);
            batteryBtn.MouseLeave += new EventHandler(batteryLeave);

            //Setting up sidebar
            sidebar = new Panel();
            tempInt = myForm.Size.Width / 5;
            sidebar.Size = new Size(tempInt, myForm.Size.Height);
            tempInt *= 4;
            sidebar.Location = new Point(tempInt, 0);
            sidebar.BackgroundImageLayout = ImageLayout.Stretch;
            sidebar.BackColor = Color.Transparent;
            myForm.Controls.Add(sidebar);
            sidebar.Visible = true;
            sidebar.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"sidebar.png");

            //Turn load screen off
            ScreenCover.Visible = false;

            //Start main loop timer
            myTimer = new System.Windows.Forms.Timer();
            myTimer.Interval = 16;
            myTimer.Tick += new EventHandler(mainLoop);
            myTimer.Start();
        }

        //Returns a default panel
        private Panel setDefaults(Panel myPanel)
        {
            myPanel.Visible = true;
            myPanel.BackColor = Color.Transparent;
            myPanel.BackgroundImageLayout = ImageLayout.Stretch;
            return myPanel;
        }

        //shows a popup with the message inputted for a short time
        public void displayPopup(string title, string tempText, int timeToDisappear = 5000)
        {
            popupText.Visible = true;
            popupTitle.Visible = true;
            popupBox.Visible = true;
            popupTitle.Text = title;
            popupText.Text = tempText;
            popupTimer.Interval = timeToDisappear;
            popupTimer.Start();
        }
        private void popupTimerMethod(object sender, EventArgs e)
        {
            popupDisappear();
        }
        public void popupDisappear()
        {
            popupBox.Visible = false;
            popupText.Visible = false;
            popupTitle.Visible = false;
        }
        //Updated the money and HP label with the corrent numbers
        public void printMoneyHP()
        {
            moneyLbl.Text = "€" + money;
            hpLbl.Text = hp.ToString();
        }

        //Maximises window
        public void fullScreen()
        {
            myForm.WindowState = FormWindowState.Maximized;
            myForm.MinimumSize = myForm.Size;
            myForm.MaximumSize = myForm.Size;
        }

        public void removeMoney(int tempInt)
        {
            money -= tempInt;
            printMoneyHP();
        }

        public void removeHP(int tempInt)
        {
            hp -= tempInt;
            printMoneyHP();
            if (hp <= 0)
            {
                endGame(false);
            }
        }

        public void addMoney(int tempInt)
        {
            money += tempInt;
            printMoneyHP();
        }

        public void resetTowerTypeSelected()
        {
            towerTypeSelected = -1;
        }

        public ITower getTowerTypeSelecter()
        {
            if (towerTypeSelected < 0 || towerTypeSelected > 5)
            {
                //System.Diagnostics.Debug.WriteLine("Returned null as tower type selected is:" + towerTypeSelected);
                return null;
            }
            //System.Diagnostics.Debug.WriteLine("Returned an instance as tower type selected is:" + towerTypeSelected);
            return towerTypes[towerTypeSelected];
        }

        public int getTowerTypeSelectedInt()
        {
            return towerTypeSelected;
        }

        /*
         * For the interaction with the buttons:
         */

        public void laserClicked(object sender, EventArgs e)
        {
            defaultClicked(0);
        }
        public void laserHover(object sender, EventArgs e)
        {
            defaultHover(laserBtn, 0);
            displayPopup("Laser", "Cost: " + towerTypes[0].getPrice() + "\nSpeed: Medium\nNot Strong");
        }
        public void laserLeave(object sender, EventArgs e)
        {
            defaultLeave(laserBtn);
        }

        public void cannonClicked(object sender, EventArgs e)
        {
            defaultClicked(1);
        }
        public void cannonHover(object sender, EventArgs e)
        {
            defaultHover(cannonBtn, 1);
            displayPopup("Cannon", "Cost: "+towerTypes[1].getPrice()+"\nSpeed: Slow\nStrong");
        }
        public void cannonLeave(object sender, EventArgs e)
        {
            defaultLeave(cannonBtn);
        }

        public void archerClicked(object sender, EventArgs e)
        {
            defaultClicked(2);
        }
        public void archerHover(object sender, EventArgs e)
        {
            defaultHover(archerBtn, 2);
            displayPopup("Archer Circle", "Cost: " + towerTypes[2].getPrice() + "\nSpeed: Medium\nNot Strong");
        }
        public void archerLeave(object sender, EventArgs e)
        {
            defaultLeave(archerBtn);
        }

        public void babyClicked(object sender, EventArgs e)
        {
            defaultClicked(3);
        }
        public void babyHover(object sender, EventArgs e)
        {
            defaultHover(babyBtn, 3);
            displayPopup("Baby", "Cost: " + towerTypes[3].getPrice() + "\nSpeed: Fast\nNot Strong");

        }
        public void babyLeave(object sender, EventArgs e)
        {
            defaultLeave(babyBtn);
        }

        public void wizardClicked(object sender, EventArgs e)
        {
            defaultClicked(4);
        }
        public void wizardHover(object sender, EventArgs e)
        {
            defaultHover(wizardBtn, 4);
            displayPopup("Wizard", "Cost: " + towerTypes[4].getPrice() + "\nSpeed: Medium\nStrong");

        }
        public void wizardLeave(object sender, EventArgs e)
        {
            defaultLeave(wizardBtn);
        }

        public void batteryClicked(object sender, EventArgs e)
        {
            defaultClicked(5);
        }
        public void batteryHover(object sender, EventArgs e)
        {
            defaultHover(batteryBtn, 5);
            displayPopup("Battery", "Cost: " + towerTypes[5].getPrice() + "\nSpeed: Super Slow\nStrong");

        }
        public void batteryLeave(object sender, EventArgs e)
        {
            defaultLeave(batteryBtn);
        }
        /*
         * By default is a tower button is selected and you have enough money for it it changed the selected tower
         */
        public void defaultClicked(int towerNum)
        {
            if (money >= towerTypes[towerNum].getPrice())
            {
                //System.Diagnostics.Debug.WriteLine("Tower clicked");
                towerTypeSelected = towerNum;
            }
            else
            {
                displayPopup("Cannot Buy", "Not enough money");
            }
        }
        /*
         * When hovering over a tower in the tower shop highlights it green if you can afford else red
         */
        public void defaultHover(Panel myPanel, int towerInt)
        {
            myPanel.BorderStyle = BorderStyle.FixedSingle;
            if (money >= towerTypes[towerInt].getPrice())
            {
                myPanel.BackColor = Color.FromArgb(50, Color.Green);
            }
            else
            {
                myPanel.BackColor = Color.FromArgb(50, Color.Red);
            }

        }

        public void defaultLeave(Panel myPanel)
        {
            myPanel.BorderStyle = BorderStyle.None;
            myPanel.BackColor = Color.Transparent;
            popupDisappear();
        }
    }
}
