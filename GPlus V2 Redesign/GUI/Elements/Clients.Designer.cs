namespace GPlus_V2_Redesign.GUI.Elements
{
    partial class Clients
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ListViewGroup listViewGroup1 = new ListViewGroup("Unconnected", HorizontalAlignment.Left);
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "Test" }, -1, Color.White, Color.FromArgb(32, 32, 32), null);
            _btnCreateClient = new ReaLTaiizor.Controls.SpaceButton();
            _listClients = new ReaLTaiizor.Controls.PoisonListView();
            _listColumn = new ColumnHeader();
            _ucCreateClient = new CreateClient();
            SuspendLayout();
            // 
            // _btnCreateClient
            // 
            _btnCreateClient.Customization = "Kioq/zIyMv8yMjL/Kioq/y8vL/8nJyf//v7+/yMjI/8qKir/";
            _btnCreateClient.Dock = DockStyle.Bottom;
            _btnCreateClient.Font = new Font("Verdana", 8F);
            _btnCreateClient.Image = null;
            _btnCreateClient.Location = new Point(0, 403);
            _btnCreateClient.Name = "_btnCreateClient";
            _btnCreateClient.NoRounding = false;
            _btnCreateClient.Size = new Size(599, 24);
            _btnCreateClient.TabIndex = 1;
            _btnCreateClient.Text = "Create Client";
            _btnCreateClient.TextAlignment = HorizontalAlignment.Center;
            _btnCreateClient.Transparent = false;
            _btnCreateClient.Click += _btnCreateClient_Click;
            // 
            // _listClients
            // 
            _listClients.AllowSorting = true;
            _listClients.BackColor = Color.FromArgb(32, 32, 32);
            _listClients.BorderStyle = BorderStyle.None;
            _listClients.Columns.AddRange(new ColumnHeader[] { _listColumn });
            _listClients.Dock = DockStyle.Right;
            _listClients.Font = new Font("Segoe UI", 12F);
            _listClients.ForeColor = Color.White;
            _listClients.FullRowSelect = true;
            listViewGroup1.Header = "Unconnected";
            listViewGroup1.Name = "_listUnconnected";
            _listClients.Groups.AddRange(new ListViewGroup[] { listViewGroup1 });
            _listClients.HideSelection = true;
            listViewItem1.Group = listViewGroup1;
            _listClients.Items.AddRange(new ListViewItem[] { listViewItem1 });
            _listClients.Location = new Point(451, 0);
            _listClients.Name = "_listClients";
            _listClients.OwnerDraw = true;
            _listClients.Size = new Size(148, 403);
            _listClients.TabIndex = 2;
            _listClients.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            _listClients.UseCompatibleStateImageBehavior = false;
            _listClients.UseCustomBackColor = true;
            _listClients.UseCustomForeColor = true;
            _listClients.UseSelectable = true;
            _listClients.UseStyleColors = true;
            // 
            // _ucCreateClient
            // 
            _ucCreateClient.BackColor = Color.FromArgb(32, 32, 32);
            _ucCreateClient.Dock = DockStyle.Fill;
            _ucCreateClient.Location = new Point(0, 0);
            _ucCreateClient.Name = "_ucCreateClient";
            _ucCreateClient.Size = new Size(451, 403);
            _ucCreateClient.TabIndex = 3;
            _ucCreateClient.Load += _ucCreateClient_Load;
            // 
            // Clients
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            Controls.Add(_ucCreateClient);
            Controls.Add(_listClients);
            Controls.Add(_btnCreateClient);
            Name = "Clients";
            Size = new Size(599, 427);
            Load += Clients_Load;
            ResumeLayout(false);
        }

        #endregion
        private ReaLTaiizor.Controls.SpaceButton _btnCreateClient;
        private ReaLTaiizor.Controls.PoisonListView _listClients;
        private ColumnHeader _listColumn;
        private CreateClient _ucCreateClient;
    }
}
