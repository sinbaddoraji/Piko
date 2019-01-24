using System;
using System.Windows.Forms;

namespace Piko
{
    public partial class SetSpeed : Form
    {
        private int speed = 1;
        public int Speed
        {
            get => 1000 * speed;
            set => speed = value;
        }

        public SetSpeed() => InitializeComponent();

        private void Button1_Click(object sender, EventArgs e)
        {
            Speed = int.Parse(textBox1.Text);
            Close();
        }

        private void Button2_Click(object sender, EventArgs e) => Close();
    }
}
