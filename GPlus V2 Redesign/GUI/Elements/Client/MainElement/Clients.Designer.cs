namespace GPlus.GUI.Elements
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
            ListViewGroup listViewGroup1 = new ListViewGroup("Unconnected", HorizontalAlignment.Center);
            ListViewItem listViewItem1 = new ListViewItem("ummmmmmmmmp");
            _listClients = new ReaLTaiizor.Controls.PoisonListView();
            _listColumn = new ColumnHeader();
            _btnCreateClient = new ReaLTaiizor.Controls.ParrotButton();
            _ucCreateClient = new CreateClient();
            _ucClientControls = new GPlus.GUI.Elements.Client.Popups.ClientControls();
            SuspendLayout();
            // 
            // _listClients
            // 
            _listClients.AllowSorting = true;
            _listClients.BackColor = Color.FromArgb(32, 32, 32);
            _listClients.BorderStyle = BorderStyle.None;
            _listClients.Columns.AddRange(new ColumnHeader[] { _listColumn });
            _listClients.Font = new Font("Segoe UI", 12F);
            _listClients.ForeColor = Color.White;
            _listClients.FullRowSelect = true;
            listViewGroup1.Header = "Unconnected";
            listViewGroup1.HeaderAlignment = HorizontalAlignment.Center;
            listViewGroup1.Name = "_listUnconnected";
            _listClients.Groups.AddRange(new ListViewGroup[] { listViewGroup1 });
            _listClients.HideSelection = true;
            _listClients.Items.AddRange(new ListViewItem[] { listViewItem1 });
            _listClients.Location = new Point(428, 0);
            _listClients.Name = "_listClients";
            _listClients.OwnerDraw = true;
            _listClients.Size = new Size(171, 385);
            _listClients.TabIndex = 2;
            _listClients.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            _listClients.UseCompatibleStateImageBehavior = false;
            _listClients.UseCustomBackColor = true;
            _listClients.UseCustomForeColor = true;
            _listClients.UseSelectable = true;
            _listClients.UseStyleColors = true;
            _listClients.SelectedIndexChanged += _listClients_SelectedIndexChanged;
            // 
            // _btnCreateClient
            // 
            _btnCreateClient.BackgroundColor = Color.FromArgb(18, 18, 18);
            _btnCreateClient.ButtonImage = null;
            _btnCreateClient.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
            _btnCreateClient.ButtonText = "Create Client";
            _btnCreateClient.ClickBackColor = Color.FromArgb(195, 195, 195);
            _btnCreateClient.ClickTextColor = Color.FromArgb(22, 22, 22);
            _btnCreateClient.CornerRadius = 5;
            _btnCreateClient.Horizontal_Alignment = StringAlignment.Center;
            _btnCreateClient.HoverBackgroundColor = Color.FromArgb(225, 225, 225);
            _btnCreateClient.HoverTextColor = Color.Gainsboro;
            _btnCreateClient.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Left;
            _btnCreateClient.Location = new Point(428, 384);
            _btnCreateClient.Name = "_btnCreateClient";
            _btnCreateClient.Size = new Size(171, 43);
            _btnCreateClient.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _btnCreateClient.TabIndex = 8;
            _btnCreateClient.TextColor = SystemColors.GrayText;
            _btnCreateClient.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _btnCreateClient.Vertical_Alignment = StringAlignment.Center;
            _btnCreateClient.Click += _btnCreateClient_Click;
            // 
            // _ucCreateClient
            // 
            _ucCreateClient.BackColor = Color.FromArgb(32, 32, 32);
            _ucCreateClient.Location = new Point(98, 50);
            _ucCreateClient.Name = "_ucCreateClient";
            _ucCreateClient.Size = new Size(233, 262);
            _ucCreateClient.TabIndex = 9;
            _ucCreateClient.Visible = false;
            // 
            // _ucClientControls
            // 
            _ucClientControls.BackColor = Color.FromArgb(26, 26, 26);
            _ucClientControls.Location = new Point(3, 0);
            _ucClientControls.Name = "_ucClientControls";
            _ucClientControls.Size = new Size(422, 427);
            _ucClientControls.TabIndex = 10;
            // 
            // Clients
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            Controls.Add(_ucClientControls);
            Controls.Add(_ucCreateClient);
            Controls.Add(_btnCreateClient);
            Controls.Add(_listClients);
            Name = "Clients";
            Size = new Size(599, 427);
            Load += Clients_Load;
            ResumeLayout(false);
        }

        #endregion
        private ReaLTaiizor.Controls.PoisonListView _listClients;
        private ColumnHeader _listColumn;
        private ReaLTaiizor.Controls.ParrotButton _btnCreateClient;
        private CreateClient _ucCreateClient;
        private Client.Popups.ClientControls _ucClientControls;
    }
}
