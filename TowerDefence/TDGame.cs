using System;
using System.Windows.Forms;

namespace TowerDefence
{
    public partial class TDGame : Form
    {
        /*
         * The form the game is printed on
         */
        public TDGame()
        {
            InitializeComponent();
        }
        /*
         * Just calls the main class upon initialising
         */
        private void TDGame_Load(object sender, EventArgs e)
        {
            mainClass myMain = new mainClass(this);
        }
    }
}
