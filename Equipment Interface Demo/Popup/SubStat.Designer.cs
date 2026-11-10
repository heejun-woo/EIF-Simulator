namespace Equipment_Interface_Demo.Popup
{
    partial class SubStat
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel = new TableLayoutPanel();
            Main = new Panel();
            groupBox4 = new GroupBox();
            label13 = new Label();
            label15 = new Label();
            groupBox3 = new GroupBox();
            label10 = new Label();
            label4 = new Label();
            label5 = new Label();
            label8 = new Label();
            label9 = new Label();
            okButton = new Button();
            groupBox2 = new GroupBox();
            label6 = new Label();
            label7 = new Label();
            groupBox1 = new GroupBox();
            label2 = new Label();
            label3 = new Label();
            label1 = new Label();
            lbEqpStat = new Label();
            pnShutDown = new Panel();
            button1 = new Button();
            groupBox8 = new GroupBox();
            label22 = new Label();
            label24 = new Label();
            label25 = new Label();
            tableLayoutPanel.SuspendLayout();
            Main.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            pnShutDown.SuspendLayout();
            groupBox8.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.Controls.Add(Main, 0, 0);
            tableLayoutPanel.Controls.Add(pnShutDown, 0, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(10, 10);
            tableLayoutPanel.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.Size = new Size(669, 317);
            tableLayoutPanel.TabIndex = 0;
            // 
            // Main
            // 
            Main.Controls.Add(groupBox4);
            Main.Controls.Add(groupBox3);
            Main.Controls.Add(okButton);
            Main.Controls.Add(groupBox2);
            Main.Controls.Add(groupBox1);
            Main.Dock = DockStyle.Fill;
            Main.Location = new Point(3, 3);
            Main.Name = "Main";
            Main.Size = new Size(663, 311);
            Main.TabIndex = 25;
            Main.Visible = false;
            // 
            // groupBox4
            // 
            groupBox4.BackColor = SystemColors.Control;
            groupBox4.Controls.Add(label13);
            groupBox4.Controls.Add(label15);
            groupBox4.FlatStyle = FlatStyle.Flat;
            groupBox4.Location = new Point(510, 40);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(135, 221);
            groupBox4.TabIndex = 26;
            groupBox4.TabStop = false;
            groupBox4.Text = "BM/PD";
            // 
            // label13
            // 
            label13.BackColor = Color.FromArgb(255, 192, 255);
            label13.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label13.Location = new Point(19, 79);
            label13.Name = "label13";
            label13.RightToLeft = RightToLeft.No;
            label13.Size = new Size(100, 41);
            label13.TabIndex = 7;
            label13.Text = "UT Down";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            label15.BackColor = Color.FromArgb(255, 192, 255);
            label15.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label15.Location = new Point(19, 28);
            label15.Name = "label15";
            label15.RightToLeft = RightToLeft.No;
            label15.Size = new Size(100, 41);
            label15.TabIndex = 5;
            label15.Text = "PM";
            label15.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBox3
            // 
            groupBox3.BackColor = SystemColors.Control;
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(label9);
            groupBox3.FlatStyle = FlatStyle.Flat;
            groupBox3.Location = new Point(265, 40);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(239, 221);
            groupBox3.TabIndex = 25;
            groupBox3.TabStop = false;
            groupBox3.Text = "비가동";
            // 
            // label10
            // 
            label10.BackColor = Color.FromArgb(255, 255, 128);
            label10.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label10.Location = new Point(19, 130);
            label10.Name = "label10";
            label10.RightToLeft = RightToLeft.No;
            label10.Size = new Size(100, 41);
            label10.TabIndex = 9;
            label10.Text = "재료 교체";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.BackColor = Color.FromArgb(255, 255, 128);
            label4.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label4.Location = new Point(125, 79);
            label4.Name = "label4";
            label4.RightToLeft = RightToLeft.No;
            label4.Size = new Size(100, 41);
            label4.TabIndex = 8;
            label4.Text = "CM";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.BackColor = Color.FromArgb(255, 255, 128);
            label5.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label5.Location = new Point(19, 79);
            label5.Name = "label5";
            label5.RightToLeft = RightToLeft.No;
            label5.Size = new Size(100, 41);
            label5.TabIndex = 7;
            label5.Text = "UT Down";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.BackColor = Color.FromArgb(255, 255, 128);
            label8.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label8.Location = new Point(125, 28);
            label8.Name = "label8";
            label8.RightToLeft = RightToLeft.No;
            label8.Size = new Size(100, 41);
            label8.TabIndex = 6;
            label8.Text = "생산 준비";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.BackColor = Color.FromArgb(255, 255, 128);
            label9.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label9.Location = new Point(19, 28);
            label9.Name = "label9";
            label9.RightToLeft = RightToLeft.No;
            label9.Size = new Size(100, 41);
            label9.TabIndex = 5;
            label9.Text = "PM";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.DialogResult = DialogResult.Cancel;
            okButton.Location = new Point(265, 267);
            okButton.Margin = new Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new Size(163, 38);
            okButton.TabIndex = 24;
            okButton.Text = "Close";
            okButton.Click += okButton_Click;
            // 
            // groupBox2
            // 
            groupBox2.BackColor = SystemColors.Control;
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(label7);
            groupBox2.FlatStyle = FlatStyle.Flat;
            groupBox2.Location = new Point(20, 180);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(239, 81);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "비부하";
            // 
            // label6
            // 
            label6.BackColor = Color.FromArgb(192, 255, 192);
            label6.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label6.Location = new Point(125, 28);
            label6.Name = "label6";
            label6.RightToLeft = RightToLeft.No;
            label6.Size = new Size(100, 41);
            label6.TabIndex = 6;
            label6.Text = "원자재 품질";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            label7.BackColor = Color.FromArgb(192, 255, 192);
            label7.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label7.Location = new Point(19, 28);
            label7.Name = "label7";
            label7.RightToLeft = RightToLeft.No;
            label7.Size = new Size(100, 41);
            label7.TabIndex = 5;
            label7.Text = "Idle";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = SystemColors.Control;
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(lbEqpStat);
            groupBox1.FlatStyle = FlatStyle.Flat;
            groupBox1.Location = new Point(20, 40);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(239, 134);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "비조업";
            // 
            // label2
            // 
            label2.BackColor = Color.LightSteelBlue;
            label2.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label2.Location = new Point(125, 79);
            label2.Name = "label2";
            label2.RightToLeft = RightToLeft.No;
            label2.Size = new Size(100, 41);
            label2.TabIndex = 8;
            label2.Text = "형교환";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.BackColor = Color.LightSteelBlue;
            label3.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label3.Location = new Point(19, 79);
            label3.Name = "label3";
            label3.RightToLeft = RightToLeft.No;
            label3.Size = new Size(100, 41);
            label3.TabIndex = 7;
            label3.Text = "Test/개발";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.BackColor = Color.LightSteelBlue;
            label1.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.Location = new Point(125, 28);
            label1.Name = "label1";
            label1.RightToLeft = RightToLeft.No;
            label1.Size = new Size(100, 41);
            label1.TabIndex = 6;
            label1.Text = "물동 없음";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbEqpStat
            // 
            lbEqpStat.BackColor = Color.LightSteelBlue;
            lbEqpStat.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            lbEqpStat.Location = new Point(19, 28);
            lbEqpStat.Name = "lbEqpStat";
            lbEqpStat.RightToLeft = RightToLeft.No;
            lbEqpStat.Size = new Size(100, 41);
            lbEqpStat.TabIndex = 5;
            lbEqpStat.Text = "Shut Down";
            lbEqpStat.TextAlign = ContentAlignment.MiddleCenter;
            lbEqpStat.Click += lbEqpStat_Click;
            // 
            // pnShutDown
            // 
            pnShutDown.Controls.Add(button1);
            pnShutDown.Controls.Add(groupBox8);
            pnShutDown.Dock = DockStyle.Fill;
            pnShutDown.Location = new Point(0, 0);
            pnShutDown.Name = "pnShutDown";
            pnShutDown.Size = new Size(663, 311);
            pnShutDown.TabIndex = 27;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.DialogResult = DialogResult.Cancel;
            button1.Location = new Point(265, 267);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(163, 38);
            button1.TabIndex = 24;
            button1.Text = "Back";
            // 
            // groupBox8
            // 
            groupBox8.BackColor = SystemColors.Control;
            groupBox8.Controls.Add(label22);
            groupBox8.Controls.Add(label24);
            groupBox8.Controls.Add(label25);
            groupBox8.FlatStyle = FlatStyle.Flat;
            groupBox8.Location = new Point(20, 40);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(625, 221);
            groupBox8.TabIndex = 0;
            groupBox8.TabStop = false;
            groupBox8.Text = "Shut Down";
            // 
            // label22
            // 
            label22.BackColor = Color.LightSteelBlue;
            label22.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label22.Location = new Point(231, 28);
            label22.Name = "label22";
            label22.RightToLeft = RightToLeft.No;
            label22.Size = new Size(100, 41);
            label22.TabIndex = 8;
            label22.Text = "비상상황";
            label22.TextAlign = ContentAlignment.MiddleCenter;
            label22.Click += label22_Click;
            // 
            // label24
            // 
            label24.BackColor = Color.FromArgb(255, 128, 0);
            label24.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label24.Location = new Point(125, 28);
            label24.Name = "label24";
            label24.RightToLeft = RightToLeft.No;
            label24.Size = new Size(100, 41);
            label24.TabIndex = 6;
            label24.Text = "공사";
            label24.TextAlign = ContentAlignment.MiddleCenter;
            label24.Click += label24_Click;
            // 
            // label25
            // 
            label25.BackColor = Color.LightSteelBlue;
            label25.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label25.Location = new Point(19, 28);
            label25.Name = "label25";
            label25.RightToLeft = RightToLeft.No;
            label25.Size = new Size(100, 41);
            label25.TabIndex = 5;
            label25.Text = "작업자 부재";
            label25.TextAlign = ContentAlignment.MiddleCenter;
            label25.Click += label25_Click;
            // 
            // SubStat
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(689, 337);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SubStat";
            Padding = new Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "SubStat";
            TopMost = true;
            tableLayoutPanel.ResumeLayout(false);
            Main.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            pnShutDown.ResumeLayout(false);
            groupBox8.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button okButton;
        private Panel Main;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label6;
        private Label label7;
        private Label label2;
        private Label label3;
        private Label label1;
        private Label lbEqpStat;
        private GroupBox groupBox4;
        private Label label13;
        private Label label15;
        private GroupBox groupBox3;
        private Label label10;
        private Label label4;
        private Label label5;
        private Label label8;
        private Label label9;
        private Panel pnShutDown;
        private Button button1;
        private GroupBox groupBox8;
        private Label label22;
        private Label label24;
        private Label label25;
    }
}
