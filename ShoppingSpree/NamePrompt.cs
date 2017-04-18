using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoppingSpree
{
    public partial class NamePrompt : Form
    {
        public string name = "Unnamed";

        public NamePrompt()
        {
            InitializeComponent();
        }

        private void NamePrompt_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            this.Close();
        }
    }
}
