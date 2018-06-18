using System;
using System.Windows.Forms;

namespace Piko
{
    public partial class SetSpeed : Form
    {
        public double Speed = 1;

        public SetSpeed() => InitializeComponent();

        private void Button1_Click(object sender, EventArgs e)
        {
            Speed = double.Parse(textBox1.Text);
            Close();
        }

        private void Button2_Click(object sender, EventArgs e) => Close();
    }
}
