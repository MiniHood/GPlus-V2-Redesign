namespace GPlus_V2_Redesign
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            _ControlBox.Location = new Point(Width - _ControlBox.Width, 0);
        }
    }
}
