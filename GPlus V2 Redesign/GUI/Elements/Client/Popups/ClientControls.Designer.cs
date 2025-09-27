namespace GPlus.GUI.Elements.Client.Popups
{
    partial class ClientControls
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
            components = new System.ComponentModel.Container();
            lostSeparator1 = new ReaLTaiizor.Controls.LostSeparator();
            _lblUsername = new ReaLTaiizor.Controls.CrownLabel();
            lostSeparator2 = new ReaLTaiizor.Controls.LostSeparator();
            _btnGotoServer = new ReaLTaiizor.Controls.ParrotButton();
            _btnDisconnect = new ReaLTaiizor.Controls.ParrotButton();
            _btnRemoveClient = new ReaLTaiizor.Controls.ParrotButton();
            _btnServersDropDown = new GPlus.GUI.Elements.Controls.MenuButton();
            _contextServerItems = new ContextMenuStrip(components);
            sToolStripMenuItem = new ToolStripMenuItem();
            dToolStripMenuItem = new ToolStripMenuItem();
            fToolStripMenuItem = new ToolStripMenuItem();
            lostSeparator3 = new ReaLTaiizor.Controls.LostSeparator();
            _lblOnWebsocket = new ReaLTaiizor.Controls.CrownLabel();
            _lblProcessID = new ReaLTaiizor.Controls.CrownLabel();
            _lblCurrentServer = new ReaLTaiizor.Controls.CrownLabel();
            _btnTestLua = new ReaLTaiizor.Controls.ParrotButton();
            _contextServerItems.SuspendLayout();
            SuspendLayout();
            // 
            // lostSeparator1
            // 
            lostSeparator1.BackColor = Color.FromArgb(36, 36, 36);
            lostSeparator1.ForeColor = Color.FromArgb(36, 36, 36);
            lostSeparator1.Horizontal = false;
            lostSeparator1.Location = new Point(120, 43);
            lostSeparator1.Name = "lostSeparator1";
            lostSeparator1.Size = new Size(175, 3);
            lostSeparator1.TabIndex = 0;
            lostSeparator1.Text = "lostSeparator1";
            // 
            // _lblUsername
            // 
            _lblUsername.ForeColor = Color.FromArgb(220, 220, 220);
            _lblUsername.Location = new Point(120, 17);
            _lblUsername.Name = "_lblUsername";
            _lblUsername.Size = new Size(175, 23);
            _lblUsername.TabIndex = 1;
            _lblUsername.Text = "Username Placeholder";
            _lblUsername.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lostSeparator2
            // 
            lostSeparator2.BackColor = Color.FromArgb(36, 36, 36);
            lostSeparator2.ForeColor = Color.FromArgb(36, 36, 36);
            lostSeparator2.Horizontal = false;
            lostSeparator2.Location = new Point(3, 128);
            lostSeparator2.Name = "lostSeparator2";
            lostSeparator2.Size = new Size(419, 3);
            lostSeparator2.TabIndex = 2;
            lostSeparator2.Text = "lostSeparator2";
            // 
            // _btnGotoServer
            // 
            _btnGotoServer.BackgroundColor = Color.FromArgb(36, 36, 36);
            _btnGotoServer.ButtonImage = null;
            _btnGotoServer.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
            _btnGotoServer.ButtonText = "Goto Server";
            _btnGotoServer.ClickBackColor = Color.FromArgb(195, 195, 195);
            _btnGotoServer.ClickTextColor = Color.FromArgb(22, 22, 22);
            _btnGotoServer.CornerRadius = 5;
            _btnGotoServer.Horizontal_Alignment = StringAlignment.Center;
            _btnGotoServer.HoverBackgroundColor = Color.FromArgb(22, 22, 22);
            _btnGotoServer.HoverTextColor = Color.Gainsboro;
            _btnGotoServer.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Left;
            _btnGotoServer.Location = new Point(19, 96);
            _btnGotoServer.Name = "_btnGotoServer";
            _btnGotoServer.Size = new Size(123, 26);
            _btnGotoServer.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _btnGotoServer.TabIndex = 8;
            _btnGotoServer.TextColor = Color.Gainsboro;
            _btnGotoServer.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _btnGotoServer.Vertical_Alignment = StringAlignment.Center;
            // 
            // _btnDisconnect
            // 
            _btnDisconnect.BackgroundColor = Color.FromArgb(36, 36, 36);
            _btnDisconnect.ButtonImage = null;
            _btnDisconnect.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
            _btnDisconnect.ButtonText = "Disconnect";
            _btnDisconnect.ClickBackColor = Color.FromArgb(195, 195, 195);
            _btnDisconnect.ClickTextColor = Color.FromArgb(22, 22, 22);
            _btnDisconnect.CornerRadius = 5;
            _btnDisconnect.Horizontal_Alignment = StringAlignment.Center;
            _btnDisconnect.HoverBackgroundColor = Color.FromArgb(22, 22, 22);
            _btnDisconnect.HoverTextColor = Color.Gainsboro;
            _btnDisconnect.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Left;
            _btnDisconnect.Location = new Point(148, 96);
            _btnDisconnect.Name = "_btnDisconnect";
            _btnDisconnect.Size = new Size(123, 26);
            _btnDisconnect.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _btnDisconnect.TabIndex = 9;
            _btnDisconnect.TextColor = Color.Gainsboro;
            _btnDisconnect.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _btnDisconnect.Vertical_Alignment = StringAlignment.Center;
            // 
            // _btnRemoveClient
            // 
            _btnRemoveClient.BackgroundColor = Color.FromArgb(36, 36, 36);
            _btnRemoveClient.ButtonImage = null;
            _btnRemoveClient.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
            _btnRemoveClient.ButtonText = "Remove client";
            _btnRemoveClient.ClickBackColor = Color.FromArgb(195, 195, 195);
            _btnRemoveClient.ClickTextColor = Color.FromArgb(22, 22, 22);
            _btnRemoveClient.CornerRadius = 5;
            _btnRemoveClient.Horizontal_Alignment = StringAlignment.Center;
            _btnRemoveClient.HoverBackgroundColor = Color.FromArgb(22, 22, 22);
            _btnRemoveClient.HoverTextColor = Color.Gainsboro;
            _btnRemoveClient.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Left;
            _btnRemoveClient.Location = new Point(277, 96);
            _btnRemoveClient.Name = "_btnRemoveClient";
            _btnRemoveClient.Size = new Size(123, 26);
            _btnRemoveClient.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _btnRemoveClient.TabIndex = 10;
            _btnRemoveClient.TextColor = Color.Gainsboro;
            _btnRemoveClient.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _btnRemoveClient.Vertical_Alignment = StringAlignment.Center;
            // 
            // _btnServersDropDown
            // 
            _btnServersDropDown.BackColor = Color.FromArgb(32, 32, 32);
            _btnServersDropDown.FlatAppearance.BorderSize = 0;
            _btnServersDropDown.FlatStyle = FlatStyle.Flat;
            _btnServersDropDown.ForeColor = Color.Gainsboro;
            _btnServersDropDown.Location = new Point(120, 52);
            _btnServersDropDown.Menu = _contextServerItems;
            _btnServersDropDown.Name = "_btnServersDropDown";
            _btnServersDropDown.Size = new Size(175, 23);
            _btnServersDropDown.TabIndex = 11;
            _btnServersDropDown.Text = "Server Placeholder";
            _btnServersDropDown.UseVisualStyleBackColor = false;
            // 
            // _contextServerItems
            // 
            _contextServerItems.AutoSize = false;
            _contextServerItems.BackColor = Color.FromArgb(26, 26, 26);
            _contextServerItems.Items.AddRange(new ToolStripItem[] { sToolStripMenuItem, dToolStripMenuItem, fToolStripMenuItem });
            _contextServerItems.Name = "contextMenuStrip1";
            _contextServerItems.ShowImageMargin = false;
            _contextServerItems.Size = new Size(203, 92);
            // 
            // sToolStripMenuItem
            // 
            sToolStripMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            sToolStripMenuItem.ForeColor = Color.White;
            sToolStripMenuItem.Name = "sToolStripMenuItem";
            sToolStripMenuItem.Size = new Size(56, 22);
            sToolStripMenuItem.Text = "s";
            // 
            // dToolStripMenuItem
            // 
            dToolStripMenuItem.Name = "dToolStripMenuItem";
            dToolStripMenuItem.Size = new Size(56, 22);
            dToolStripMenuItem.Text = "d";
            // 
            // fToolStripMenuItem
            // 
            fToolStripMenuItem.Name = "fToolStripMenuItem";
            fToolStripMenuItem.Size = new Size(56, 22);
            fToolStripMenuItem.Text = "f";
            // 
            // lostSeparator3
            // 
            lostSeparator3.BackColor = Color.FromArgb(36, 36, 36);
            lostSeparator3.ForeColor = Color.FromArgb(36, 36, 36);
            lostSeparator3.Horizontal = false;
            lostSeparator3.Location = new Point(3, 211);
            lostSeparator3.Name = "lostSeparator3";
            lostSeparator3.Size = new Size(419, 3);
            lostSeparator3.TabIndex = 12;
            lostSeparator3.Text = "lostSeparator3";
            // 
            // _lblOnWebsocket
            // 
            _lblOnWebsocket.ForeColor = Color.FromArgb(220, 220, 220);
            _lblOnWebsocket.Location = new Point(3, 134);
            _lblOnWebsocket.Name = "_lblOnWebsocket";
            _lblOnWebsocket.Size = new Size(175, 23);
            _lblOnWebsocket.TabIndex = 13;
            _lblOnWebsocket.Text = "Is Registered: No";
            _lblOnWebsocket.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _lblProcessID
            // 
            _lblProcessID.ForeColor = Color.FromArgb(220, 220, 220);
            _lblProcessID.Location = new Point(3, 157);
            _lblProcessID.Name = "_lblProcessID";
            _lblProcessID.Size = new Size(175, 23);
            _lblProcessID.TabIndex = 14;
            _lblProcessID.Text = "Process ID: 0";
            _lblProcessID.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _lblCurrentServer
            // 
            _lblCurrentServer.ForeColor = Color.FromArgb(220, 220, 220);
            _lblCurrentServer.Location = new Point(3, 180);
            _lblCurrentServer.Name = "_lblCurrentServer";
            _lblCurrentServer.Size = new Size(175, 23);
            _lblCurrentServer.TabIndex = 15;
            _lblCurrentServer.Text = "Current Server: 127.0.0.1";
            _lblCurrentServer.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _btnTestLua
            // 
            _btnTestLua.BackgroundColor = Color.FromArgb(36, 36, 36);
            _btnTestLua.ButtonImage = null;
            _btnTestLua.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
            _btnTestLua.ButtonText = "Test Lua";
            _btnTestLua.ClickBackColor = Color.FromArgb(195, 195, 195);
            _btnTestLua.ClickTextColor = Color.FromArgb(22, 22, 22);
            _btnTestLua.CornerRadius = 5;
            _btnTestLua.Horizontal_Alignment = StringAlignment.Center;
            _btnTestLua.HoverBackgroundColor = Color.FromArgb(22, 22, 22);
            _btnTestLua.HoverTextColor = Color.Gainsboro;
            _btnTestLua.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Left;
            _btnTestLua.Location = new Point(277, 137);
            _btnTestLua.Name = "_btnTestLua";
            _btnTestLua.Size = new Size(123, 26);
            _btnTestLua.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            _btnTestLua.TabIndex = 16;
            _btnTestLua.TextColor = Color.Gainsboro;
            _btnTestLua.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            _btnTestLua.Vertical_Alignment = StringAlignment.Center;
            _btnTestLua.Click += _btnTestLua_Click;
            // 
            // ClientControls
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(26, 26, 26);
            Controls.Add(_btnTestLua);
            Controls.Add(_lblCurrentServer);
            Controls.Add(_lblProcessID);
            Controls.Add(_lblOnWebsocket);
            Controls.Add(lostSeparator3);
            Controls.Add(_btnServersDropDown);
            Controls.Add(_btnRemoveClient);
            Controls.Add(_btnDisconnect);
            Controls.Add(_btnGotoServer);
            Controls.Add(lostSeparator2);
            Controls.Add(_lblUsername);
            Controls.Add(lostSeparator1);
            Name = "ClientControls";
            Size = new Size(422, 424);
            Load += ClientControls_Load;
            _contextServerItems.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.LostSeparator lostSeparator1;
        private ReaLTaiizor.Controls.CrownLabel _lblUsername;
        private ReaLTaiizor.Controls.LostSeparator lostSeparator2;
        private ReaLTaiizor.Controls.ParrotButton _btnGotoServer;
        private ReaLTaiizor.Controls.ParrotButton _btnDisconnect;
        private ReaLTaiizor.Controls.ParrotButton _btnRemoveClient;
        private Controls.MenuButton _btnServersDropDown;
        private ContextMenuStrip _contextServerItems;
        private ToolStripMenuItem sToolStripMenuItem;
        private ToolStripMenuItem dToolStripMenuItem;
        private ToolStripMenuItem fToolStripMenuItem;
        private ReaLTaiizor.Controls.LostSeparator lostSeparator3;
        private ReaLTaiizor.Controls.CrownLabel _lblOnWebsocket;
        private ReaLTaiizor.Controls.CrownLabel _lblProcessID;
        private ReaLTaiizor.Controls.CrownLabel _lblCurrentServer;
        private ReaLTaiizor.Controls.ParrotButton _btnTestLua;
    }
}
