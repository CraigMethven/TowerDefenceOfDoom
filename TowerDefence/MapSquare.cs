using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace TowerDefence
{
    /*
     * The class that stores and displays all the info about a square on the map
     */
    class MapSquare
    {
        //Display panels
        private Panel thePanel;
        //Panels for the enemies
        private Panel regularPanel;
        private Panel strongPanel;
        private Label regularNum;
        private Label strongNum;

        //static variables used between all classes
        private static int squareSize;
        private static int startingX;
        private static int startingY;
        private static Form myForm;
        private static mainClass myMain;
        private static Map myMap;
        private static List<Image> pathImages;
        private static List<Image> landImages;
        private static List<Image> enemyImages;
        private static List<Image> strongEnemyImages;

        //Variables used for logic
        private ITower myTower = null;
        private int myX;
        private int myY;
        public bool addedToPathList = false;

        //This is either P (Path), L (Land), S (Starting Point) or D (Defend Point)
        private string squareType;

        //For keeping track of enemies in the square
        private int numOfRegularEnemies = 0;
        private int numOfStrongEnemies = 0;

        /*
         * Constructor to set up variables and objects
         */
        public MapSquare(int tempX, int tempY, string type)
        {
            myX = tempX;
            myY = tempY;
            squareType = type;
            //setting up enemy displays
            if(type == "P" || type == "S")
            {
                Random rnd = new Random();

                int numBoxSize = 25;
                int padding = 2;

                regularPanel = setUpPanel();
                regularPanel.Visible = false;
                regularPanel.Size = new Size(squareSize / 2 - padding, squareSize / 2 - padding);
                regularPanel.Location = new Point(regularPanel.Location.X + padding, regularPanel.Location.Y + padding);
                regularPanel.BackgroundImage = enemyImages[rnd.Next(0, pathImages.Count)];

                regularNum = setUpLabel(numBoxSize);
                regularNum.Location = new Point(regularPanel.Location.X + regularPanel.Width - numBoxSize, regularPanel.Location.Y + regularPanel.Height - numBoxSize);

                strongPanel = setUpPanel();
                strongPanel.Visible = false;
                strongPanel.Size = new Size(squareSize / 2 - padding, squareSize / 2 - padding);
                strongPanel.Location = new Point(regularPanel.Location.X + squareSize/2 - padding, regularPanel.Location.Y + squareSize/2 - padding);
                strongPanel.BackgroundImage = strongEnemyImages[rnd.Next(0, strongEnemyImages.Count)];

                strongNum = setUpLabel(numBoxSize);
                strongNum.Location = new Point(strongPanel.Location.X + strongPanel.Width - numBoxSize, strongPanel.Location.Y + strongPanel.Height - numBoxSize);

                myForm.Controls.Add(regularPanel);
                myForm.Controls.Add(strongPanel);
            }
            
            //Setting up background
            thePanel = setUpPanel();
            setDefaultImage();
            myForm.Controls.Add(thePanel);
            thePanel.MouseEnter += new EventHandler(hoverOver);
            thePanel.MouseLeave += new EventHandler(stopHover);
            thePanel.Click += new EventHandler(clicked);
       }

        /*
         * returns a defauly label
         */
        private Label setUpLabel(int size)
        {
            Label myLabel = new Label();
            myLabel.Size = new Size(size, size);
            myLabel.BackColor = Color.Transparent;
            myLabel.ForeColor = Color.Red;
            myLabel.Font = new Font(mainClass.font, size * 4 / 5);
            myLabel.TextAlign = ContentAlignment.MiddleCenter;
            myForm.Controls.Add(myLabel);
            myLabel.Visible = false;
            myLabel.Text = "";
            return myLabel;
        }

        /*
         * When the panel is hovered over and there's a tower in it it displays the range and the number of kills the tower has. Or if empty land displays range the tower selected would have
         */
        public void hoverOver(object sender, EventArgs e)
        {
            if(myMain.getTowerTypeSelecter() is null)
            {
                if (myTower != null && squareType == "L")
                {
                    myTower.highlightAoE(0, myX, myY);
                    myMain.displayPopup("Tower", "Currently has " + myTower.getKills() + " kills");
                    return;
                }
            }
            else
            {
                if(myTower == null && squareType == "L")
                {
                    ITower tempTower = myMain.getTowerTypeSelecter();
                    tempTower.highlightAoE(0, myX, myY);
                }
                
            } 
        }

        /*
         * Removes the highlights that hovering applied
         */
        public void stopHover(object sender, EventArgs e)
        {
            if (myMain.getTowerTypeSelecter() is null)
            {
                if (myTower != null && squareType == "L")
                {
                    myTower.unhighlightAoE(0, myX, myY);
                    myMain.popupDisappear();
                    return;
                }
            }
            else
            {
                if (myTower == null && squareType == "L")
                {
                    //System.Diagnostics.Debug.WriteLine("Price of currently selected turret is: " + myMain.getTowerTypeSelecter().getPrice());
                    ITower tempTower = myMain.getTowerTypeSelecter();
                    tempTower.unhighlightAoE(0, myX, myY);
                }
            }
        }

        /*
         * Method for when a panel is clicked. It is in charge of placing towers
         */
        public void clicked(object sender, EventArgs e)
        {
            if (myMain.getTowerTypeSelecter() is null)
            {
                if (myTower == null && squareType == "L")
                {
                    return;
                }
            }
            else
            {
                if (myTower == null && squareType == "L")
                {
                    //Sets the tower stored in this square
                    int towerType = myMain.getTowerTypeSelectedInt();
                    switch (towerType)
                    {
                        case 0:
                            myTower = new Laser(myY, myX);
                            break;
                        case 1:
                            myTower = new Cannon(myY, myX);
                            break;
                        case 2:
                            myTower = new ArcherCircle(myY, myX);
                            break;
                        case 3:
                            myTower = new Baby(myY, myX);
                            break;
                        case 4:
                            myTower = new Wizard(myY, myX);
                            break;
                        case 5:
                            myTower = new FaultyBattery(myY, myX);
                            break;
                    }

                    //Changes appropriate variables for when a tower is bought and placed
                    myMain.removeMoney(myTower.getPrice());
                    myMap.addTower(myTower);
                    thePanel.BackgroundImage = myTower.getImage();
                    myMain.resetTowerTypeSelected();
                }
            }
        }
        /*
         * Static method to setup the static variables of the class
         */
        public static void setupSizes(int maxX, int maxY)
        {
            //Setting up other parts of the form and class
            myForm.BackgroundImage = Image.FromFile(Map.resourcesDirectory + "background.png");
            pathImages = new List<Image>();
            landImages = new List<Image>();
            //Stores all the possible background images for paths and land in the appropriate list
            for(int i = 1; i <= 7; i++)
            {
                pathImages.Add(Image.FromFile(Map.resourcesDirectory + @"Backgrounds\P" + i.ToString() + ".png"));
                landImages.Add(Image.FromFile(Map.resourcesDirectory + @"Backgrounds\L" + i.ToString() + ".png"));
            }
            enemyImages = new List<Image>();
            strongEnemyImages = new List<Image>();
            //Stores all the possible enemy images in the appropriate list
            for (int i = 1; i <= 7; i++)
            {
                enemyImages.Add(Image.FromFile(Map.resourcesDirectory + @"Enemies\enemy" + i.ToString() + ".png"));
                strongEnemyImages.Add(Image.FromFile(Map.resourcesDirectory + @"Enemies\strongEnemy" + i.ToString() + ".png"));
            }
            //Getting the area the map should be in
            int usableX = myForm.Size.Width;
            usableX *= 4;
            usableX /= 5;
            int usableY = myForm.Size.Height;

            //Calculating and setting the size of the square and their starting position
            if (usableX/maxX > usableY/maxY)
            {
                squareSize = (usableY/maxY)-2;
            }
            else
            {
                squareSize = (usableX/maxX)-2;
            }
            startingY = (usableY - (squareSize * maxY)) / 2;
            startingX = (usableX - (squareSize * maxX)) / 2;
        }

        /*
         * returns a default panel
         */
        public Panel setUpPanel()
        {
            Panel myPanel = new Panel();
            myPanel.Size = new Size(squareSize,squareSize);
            myPanel.Location = new Point(startingX + (squareSize * myX), startingY + (squareSize * myY));
            myPanel.BackgroundImageLayout = ImageLayout.Stretch;
            myPanel.BackColor = Color.Transparent;
            myPanel.Visible = true;
            return myPanel;
        }

        /*
         * Sets the background image for this square
         */
        public void setDefaultImage()
        {
            if (squareType == "D")
            {
                thePanel.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Backgrounds\D.png");
            }
            //Land and paths are randomly selected from their collection
            else if(squareType == "L")
            {
                Random rnd = new Random();
                int tempInt = rnd.Next(0, landImages.Count);
                thePanel.BackgroundImage = landImages[tempInt];
            }
            else if(squareType == "P")
            {
                Random rnd = new Random();
                int tempInt = rnd.Next(0, pathImages.Count);
                thePanel.BackgroundImage = pathImages[tempInt];
            }
            else if(squareType == "S")
            {
                thePanel.BackgroundImage = Image.FromFile(Map.resourcesDirectory + @"Backgrounds\S.png");
            }
        }
        /*
         * Sets the highlight of the square when needed
         */
        public void setBackcolour(Color tempC)
        {
            thePanel.BackColor = Color.FromArgb(50,tempC);
        }
        //Setting up pointers to the other classes
        public static void setForm(Form tempForm, mainClass tempMain)
        {
            myForm = tempForm;
            myMain = tempMain;
        }
        
        /*
         * The method in charge of the damage calcualtions
         */
        public int takeHit(int damage = 1, bool strong = false)
        {
            int numKilled = 0;
            int moneyEarned = 0;
            //Loop for the amount of damage that they do
            for(int i = 0; i < damage; i++)
            {
                //Attack the strong first
                if (strong)
                {
                    if (numOfStrongEnemies > 0)
                    {
                        numOfStrongEnemies--;
                        numKilled++;
                        moneyEarned += 2;
                    }
                    else
                    {
                        strong = false;
                    }
                }
                if(!strong)
                {
                    if (numOfRegularEnemies > 0)
                    {
                        numOfRegularEnemies--;
                        numKilled++;
                        moneyEarned += 1;
                    }
                    //If no enemies left then leave prematurely
                    else
                    {
                        enemyVisible();
                        myMain.addMoney(moneyEarned);
                        return numKilled;
                    }
                }
            }
            //Checks to see if the enemies still need to be visible
            enemyVisible();
            //Adds the money for the kill and returns the number of enemies killed
            myMain.addMoney(moneyEarned);
            return numKilled;
        }

        /*
         * Sets the enemies of this square
         */
        public void setEnemies(int reg, int str)
        {
            numOfRegularEnemies = reg;
            numOfStrongEnemies = str;
            enemyVisible();
        }

        /*
         * Checks to see if the enemies need to be displayed and if so does it
         */
        public void enemyVisible()
        {
            //For regular enemies
            if (numOfRegularEnemies > 0)
            {
                regularNum.Text = numOfRegularEnemies.ToString();
                regularNum.Visible = true;
                regularPanel.Visible = true;
            }
            else
            {
                regularNum.Visible = false;
                regularPanel.Visible = false;
            }
            //For strong enemies
            if (numOfStrongEnemies > 0)
            {
                strongNum.Text = numOfStrongEnemies.ToString();
                strongNum.Visible = true;
                strongPanel.Visible = true;
            }
            else
            {
                strongNum.Visible = false;
                strongPanel.Visible = false;
            }
        }

        /*
         * Getters and setters
         */
        public string getType()
        {
            return squareType;
        }

        public bool getIfEnemy()
        {
            if (numOfRegularEnemies + numOfStrongEnemies > 0)
            {
                return true;
            }
            return false;
        }

        public int getRegEnemies()
        {
            return numOfRegularEnemies;
        }

        public int getStrEnemies()
        {
            return numOfStrongEnemies;
        }

        public static void setMap(Map tempMap)
        {
            myMap = tempMap;
        }

        public int getX()
        {
            return myX;
        }
        public int getY()
        {
            return myY;
        }
    }
}
