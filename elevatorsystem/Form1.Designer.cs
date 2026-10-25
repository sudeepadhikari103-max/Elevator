// Button UI Implementation

using System;
using System.Drawing;
using System.Windows.Forms;

namespace elevatorsystem
{
    partial class Elevatorsystem
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            label1 = new Label();
            pnlshaft = new Panel();
            label3 = new Label();
            pnlElevator = new Panel();
            lblelevator1 = new Label();
            label2 = new Label();
            label7 = new Label();
            pnlcontrols = new Panel();
            lblStatusCard = new Label();
            label8 = new Label();
            label5 = new Label();
            labelHall = new Label();
            labelCar = new Label();
            button4 = new Button();
            button3 = new Button();
            lblfloor2 = new Button();
            lblfloor1 = new Button();
            label6 = new Label();
            pnlcontrol = new Label();
            label4 = new Label();
            pnldatabase = new Panel();
            dgvLogs = new DataGridView();
            pnlDbTop = new Panel();
            btnClearDb = new Button();
            btnRefreshDb = new Button();
            txtSearch = new TextBox();
            lblDbTitle = new Label();
            pnlHeader = new Panel();
            lblHeaderSub = new Label();
            button5 = new Button();
            btnDoorClose = new Button();
            btnDoorOpen = new Button();
            pnlshaft.SuspendLayout();
            pnlElevator.SuspendLayout();
            pnlcontrols.SuspendLayout();
            pnldatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLogs).BeginInit();
            pnlDbTop.SuspendLayout();
            pnlHeader.SuspendLayout();
            SuspendLayout();

            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(56, 189, 248);
            label1.Location = new Point(16, 10);
            label1.Name = "label1";
            label1.Size = new Size(174, 30);
            label1.TabIndex = 0;
            label1.Text = "Elevator System";

            pnlHeader.BackColor = Color.FromArgb(17, 24, 39);
            pnlHeader.Controls.Add(button5);
            pnlHeader.Controls.Add(lblHeaderSub);
            pnlHeader.Controls.Add(label1);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1020, 52);
            pnlHeader.TabIndex = 0;

            lblHeaderSub.AutoSize = true;
            lblHeaderSub.Font = new Font("Segoe UI", 9F);
            lblHeaderSub.ForeColor = Color.FromArgb(148, 163, 184);
            lblHeaderSub.Location = new Point(196, 16);
            lblHeaderSub.Name = "lblHeaderSub";
            lblHeaderSub.Size = new Size(177, 20);
            lblHeaderSub.TabIndex = 1;
            lblHeaderSub.Text = "•  2 Floors  •  SQLite Logs";

            button5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button5.BackColor = Color.FromArgb(30, 41, 59);
            button5.FlatAppearance.BorderSize = 0;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            button5.ForeColor = Color.FromArgb(203, 213, 225);
            button5.Location = new Point(910, 10);
            button5.Name = "button5";
            button5.Size = new Size(95, 32);
            button5.TabIndex = 2;
            button5.Text = "Summary";
            button5.UseVisualStyleBackColor = false;

            pnlshaft.BackColor = Color.FromArgb(24, 32, 47);
            pnlshaft.Controls.Add(label3);
            pnlshaft.Controls.Add(pnlElevator);
            pnlshaft.Controls.Add(label2);
            pnlshaft.Controls.Add(label7);
            pnlshaft.Location = new Point(16, 64);
            pnlshaft.Name = "pnlshaft";
            pnlshaft.Size = new Size(330, 536);
            pnlshaft.TabIndex = 1;
            pnlshaft.Paint += panel1_Paint;

            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(56, 189, 248);
            label3.Location = new Point(14, 25);
            label3.Name = "label3";
            label3.Size = new Size(64, 23);
            label3.TabIndex = 1;
            label3.Text = "Floor 2";

            pnlElevator.Controls.Add(lblelevator1);
            pnlElevator.Location = new Point(65, 328);
            pnlElevator.Name = "pnlElevator";
            pnlElevator.Size = new Size(200, 185);
            pnlElevator.TabIndex = 2;

            lblelevator1.AutoSize = true;
            lblelevator1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblelevator1.ForeColor = Color.FromArgb(248, 250, 252);
            lblelevator1.Location = new Point(38, 8);
            lblelevator1.Name = "lblelevator1";
            lblelevator1.Size = new Size(68, 20);
            lblelevator1.TabIndex = 0;
            lblelevator1.Text = "Elevator";
            lblelevator1.Click += label2_Click;

            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(56, 189, 248);
            label2.Location = new Point(14, 320);
            label2.Name = "label2";
            label2.Size = new Size(64, 23);
            label2.TabIndex = 3;
            label2.Text = "Floor 1";
            label2.Click += label2_Click_1;

            label7.AutoSize = true;
            label7.Location = new Point(0, 0);
            label7.Name = "label7";
            label7.Size = new Size(0, 20);
            label7.TabIndex = 0;

            pnlcontrols.BackColor = Color.FromArgb(24, 32, 47);
            pnlcontrols.Controls.Add(lblStatusCard);
            pnlcontrols.Controls.Add(label8);
            pnlcontrols.Controls.Add(label5);
            pnlcontrols.Controls.Add(labelHall);
            pnlcontrols.Controls.Add(labelCar);
            pnlcontrols.Controls.Add(button4);
            pnlcontrols.Controls.Add(button3);
            pnlcontrols.Controls.Add(lblfloor2);
            pnlcontrols.Controls.Add(lblfloor1);
            pnlcontrols.Controls.Add(btnDoorClose);
            pnlcontrols.Controls.Add(btnDoorOpen);
            pnlcontrols.Controls.Add(btnDoorClose);
            pnlcontrols.Controls.Add(label6);
            pnlcontrols.Controls.Add(pnlcontrol);
            pnlcontrols.Controls.Add(label4);
            pnlcontrols.Location = new Point(360, 64);
            pnlcontrols.Name = "pnlcontrols";
            pnlcontrols.Size = new Size(250, 536);
            pnlcontrols.TabIndex = 2;

            lblStatusCard.BackColor = Color.FromArgb(17, 24, 39);
            lblStatusCard.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblStatusCard.ForeColor = Color.FromArgb(52, 211, 153);
            lblStatusCard.Location = new Point(16, 68);
            lblStatusCard.Name = "lblStatusCard";
            lblStatusCard.Size = new Size(218, 28);
            lblStatusCard.TabIndex = 2;
            lblStatusCard.Text = "Idle";
            lblStatusCard.TextAlign = ContentAlignment.MiddleCenter;

            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 8.5F);
            label8.ForeColor = Color.FromArgb(148, 163, 184);
            label8.Location = new Point(18, 400);
            label8.Name = "label8";
            label8.Size = new Size(106, 20);
            label8.TabIndex = 11;
            label8.Text = "Floor 2: Ready";

            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 8.5F);
            label5.ForeColor = Color.FromArgb(148, 163, 184);
            label5.Location = new Point(18, 376);
            label5.Name = "label5";
            label5.Size = new Size(106, 20);
            label5.TabIndex = 10;
            label5.Text = "Floor 1: Ready";
            label5.Click += label5_Click_1;

            labelHall.AutoSize = true;
            labelHall.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelHall.ForeColor = Color.FromArgb(203, 213, 225);
            labelHall.Location = new Point(16, 252);
            labelHall.Name = "labelHall";
            labelHall.Size = new Size(103, 20);
            labelHall.TabIndex = 7;
            labelHall.Text = "Hall Call Box:";

            labelCar.AutoSize = true;
            labelCar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            labelCar.ForeColor = Color.FromArgb(203, 213, 225);
            labelCar.Location = new Point(16, 104);
            labelCar.Name = "labelCar";
            labelCar.Size = new Size(111, 20);
            labelCar.TabIndex = 4;
            labelCar.Text = "Cabin Buttons:";

            button4.BackColor = Color.FromArgb(30, 41, 59);
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            button4.ForeColor = Color.FromArgb(248, 250, 252);
            button4.Location = new Point(16, 324);
            button4.Name = "button4";
            button4.Size = new Size(218, 40);
            button4.TabIndex = 9;
            button4.Text = "Call Floor 2  ▲";
            button4.UseVisualStyleBackColor = false;

            button3.BackColor = Color.FromArgb(30, 41, 59);
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            button3.ForeColor = Color.FromArgb(248, 250, 252);
            button3.Location = new Point(16, 276);
            button3.Name = "button3";
            button3.Size = new Size(218, 40);
            button3.TabIndex = 8;
            button3.Text = "Call Floor 1  ▼";
            button3.UseVisualStyleBackColor = false;

            lblfloor2.BackColor = Color.FromArgb(30, 41, 59);
            lblfloor2.FlatAppearance.BorderSize = 0;
            lblfloor2.FlatStyle = FlatStyle.Flat;
            lblfloor2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblfloor2.ForeColor = Color.FromArgb(248, 250, 252);
            lblfloor2.Location = new Point(132, 128);
            lblfloor2.Name = "lblfloor2";
            lblfloor2.Size = new Size(102, 68);
            lblfloor2.TabIndex = 6;
            lblfloor2.Text = "2";
            lblfloor2.UseVisualStyleBackColor = false;

            lblfloor1.BackColor = Color.FromArgb(30, 41, 59);
            lblfloor1.FlatAppearance.BorderSize = 0;
            lblfloor1.FlatStyle = FlatStyle.Flat;
            lblfloor1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblfloor1.ForeColor = Color.FromArgb(248, 250, 252);
            lblfloor1.Location = new Point(16, 128);
            lblfloor1.Name = "lblfloor1";
            lblfloor1.Size = new Size(102, 68);
            lblfloor1.TabIndex = 5;
            lblfloor1.Text = "1";
            lblfloor1.UseVisualStyleBackColor = false;

            btnDoorClose.BackColor = Color.FromArgb(30, 41, 59);
            btnDoorClose.FlatAppearance.BorderSize = 0;
            btnDoorClose.FlatStyle = FlatStyle.Flat;
            btnDoorClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnDoorClose.ForeColor = Color.FromArgb(248, 113, 113);
            btnDoorClose.Location = new Point(132, 204);
            btnDoorClose.Name = "btnDoorClose";
            btnDoorClose.Size = new Size(102, 38);
            btnDoorClose.TabIndex = 12;
            btnDoorClose.Text = "►◄ Close";
            btnDoorClose.UseVisualStyleBackColor = false;

            btnDoorOpen.BackColor = Color.FromArgb(30, 41, 59);
            btnDoorOpen.FlatAppearance.BorderSize = 0;
            btnDoorOpen.FlatStyle = FlatStyle.Flat;
            btnDoorOpen.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnDoorOpen.ForeColor = Color.FromArgb(56, 189, 248);
            btnDoorOpen.Location = new Point(16, 204);
            btnDoorOpen.Name = "btnDoorOpen";
            btnDoorOpen.Size = new Size(102, 38);
            btnDoorOpen.TabIndex = 13;
            btnDoorOpen.Text = "◄► Open";
            btnDoorOpen.UseVisualStyleBackColor = false;

            label6.AutoSize = true;
            label6.Location = new Point(0, 0);
            label6.Name = "label6";
            label6.Size = new Size(0, 20);
            label6.TabIndex = 3;

            pnlcontrol.AutoSize = true;
            pnlcontrol.Location = new Point(0, 0);
            pnlcontrol.Name = "pnlcontrol";
            pnlcontrol.Size = new Size(0, 20);
            pnlcontrol.TabIndex = 0;
            pnlcontrol.Click += label5_Click;

            label4.BackColor = Color.FromArgb(17, 24, 39);
            label4.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(56, 189, 248);
            label4.Location = new Point(16, 16);
            label4.Name = "label4";
            label4.Size = new Size(218, 46);
            label4.TabIndex = 1;
            label4.Text = "Floor 1";
            label4.TextAlign = ContentAlignment.MiddleCenter;

            pnldatabase.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnldatabase.BackColor = Color.FromArgb(24, 32, 47);
            pnldatabase.Controls.Add(dgvLogs);
            pnldatabase.Controls.Add(pnlDbTop);
            pnldatabase.Location = new Point(624, 64);
            pnldatabase.Name = "pnldatabase";
            pnldatabase.Size = new Size(380, 536);
            pnldatabase.TabIndex = 3;

            dgvLogs.AllowUserToAddRows = false;
            dgvLogs.AllowUserToDeleteRows = false;
            dgvLogs.AllowUserToResizeRows = false;
            dgvLogs.BackgroundColor = Color.FromArgb(17, 24, 39);
            dgvLogs.BorderStyle = BorderStyle.None;
            dgvLogs.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(148, 163, 184);
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(148, 163, 184);
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvLogs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvLogs.ColumnHeadersHeight = 34;
            dgvLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(20, 28, 42);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 8.8F);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(226, 232, 240);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(37, 99, 235);
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvLogs.DefaultCellStyle = dataGridViewCellStyle2;
            dgvLogs.Dock = DockStyle.Fill;
            dgvLogs.EnableHeadersVisualStyles = false;
            dgvLogs.GridColor = Color.FromArgb(30, 41, 59);
            dgvLogs.Location = new Point(0, 50);
            dgvLogs.MultiSelect = false;
            dgvLogs.Name = "dgvLogs";
            dgvLogs.ReadOnly = true;
            dgvLogs.RowHeadersVisible = false;
            dgvLogs.RowHeadersWidth = 51;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(24, 32, 47);
            dgvLogs.RowsDefaultCellStyle = dataGridViewCellStyle3;
            dgvLogs.RowTemplate.Height = 28;
            dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLogs.Size = new Size(380, 486);
            dgvLogs.TabIndex = 1;

            pnlDbTop.BackColor = Color.FromArgb(24, 32, 47);
            pnlDbTop.Controls.Add(btnClearDb);
            pnlDbTop.Controls.Add(btnRefreshDb);
            pnlDbTop.Controls.Add(txtSearch);
            pnlDbTop.Controls.Add(lblDbTitle);
            pnlDbTop.Dock = DockStyle.Top;
            pnlDbTop.Location = new Point(0, 0);
            pnlDbTop.Name = "pnlDbTop";
            pnlDbTop.Size = new Size(380, 50);
            pnlDbTop.TabIndex = 0;

            btnClearDb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearDb.BackColor = Color.FromArgb(30, 41, 59);
            btnClearDb.FlatAppearance.BorderSize = 0;
            btnClearDb.FlatStyle = FlatStyle.Flat;
            btnClearDb.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnClearDb.ForeColor = Color.FromArgb(248, 113, 113);
            btnClearDb.Location = new Point(314, 11);
            btnClearDb.Name = "btnClearDb";
            btnClearDb.Size = new Size(56, 28);
            btnClearDb.TabIndex = 3;
            btnClearDb.Text = "Clear";
            btnClearDb.UseVisualStyleBackColor = false;

            btnRefreshDb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefreshDb.BackColor = Color.FromArgb(30, 41, 59);
            btnRefreshDb.FlatAppearance.BorderSize = 0;
            btnRefreshDb.FlatStyle = FlatStyle.Flat;
            btnRefreshDb.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            btnRefreshDb.ForeColor = Color.FromArgb(52, 211, 153);
            btnRefreshDb.Location = new Point(248, 11);
            btnRefreshDb.Name = "btnRefreshDb";
            btnRefreshDb.Size = new Size(60, 28);
            btnRefreshDb.TabIndex = 2;
            btnRefreshDb.Text = "Refresh";
            btnRefreshDb.UseVisualStyleBackColor = false;

            txtSearch.BackColor = Color.FromArgb(17, 24, 39);
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Font = new Font("Segoe UI", 8.5F);
            txtSearch.ForeColor = Color.FromArgb(241, 245, 249);
            txtSearch.Location = new Point(105, 12);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search...";
            txtSearch.Size = new Size(135, 26);
            txtSearch.TabIndex = 1;

            lblDbTitle.AutoSize = true;
            lblDbTitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblDbTitle.ForeColor = Color.FromArgb(241, 245, 249);
            lblDbTitle.Location = new Point(10, 14);
            lblDbTitle.Name = "lblDbTitle";
            lblDbTitle.Size = new Size(89, 21);
            lblDbTitle.TabIndex = 0;
            lblDbTitle.Text = "SQLite Log";

            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            ClientSize = new Size(1020, 616);
            Controls.Add(pnldatabase);
            Controls.Add(pnlcontrols);
            Controls.Add(pnlshaft);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MinimumSize = new Size(980, 580);
            Name = "Elevatorsystem";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Elevator System";
            Load += Form1_Load;
            pnlshaft.ResumeLayout(false);
            pnlshaft.PerformLayout();
            pnlElevator.ResumeLayout(false);
            pnlElevator.PerformLayout();
            pnlcontrols.ResumeLayout(false);
            pnlcontrols.PerformLayout();
            pnldatabase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLogs).EndInit();
            pnlDbTop.ResumeLayout(false);
            pnlDbTop.PerformLayout();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Panel pnlHeader;
        private Label lblHeaderSub;
        private Panel pnlshaft;
        private Panel pnlElevator;
        private Label lblelevator1;
        private Label label3;
        private Label label2;
        private Label label7;
        private Panel pnlcontrols;
        private Label label4;
        private Label lblStatusCard;
        private Label labelCar;
        private Label labelHall;
        private Button lblfloor1;
        private Button lblfloor2;
        private Button button3;
        private Button button4;
        private Label label5;
        private Label label8;
        private Label label6;
        private Label pnlcontrol;
        private Panel pnldatabase;
        private Panel pnlDbTop;
        private Label lblDbTitle;
        private TextBox txtSearch;
        private Button btnRefreshDb;
        private Button btnClearDb;
        private Button button5;
        private DataGridView dgvLogs;
        private Button btnDoorClose;
        private Button btnDoorOpen;
    }
}
