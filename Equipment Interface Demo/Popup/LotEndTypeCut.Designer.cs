namespace Equipment_Interface_Demo.Popup
{
    partial class LotEndTypeCut
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
            label5 = new Label();
            label6 = new Label();
            label3 = new Label();
            label4 = new Label();
            label1 = new Label();
            label2 = new Label();
            okButton = new Button();
            label9 = new Label();
            tableLayoutPanel.SuspendLayout();
            Main.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel.Controls.Add(Main, 0, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(10, 10);
            tableLayoutPanel.Margin = new Padding(4, 3, 4, 3);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle());
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.Size = new Size(342, 292);
            tableLayoutPanel.TabIndex = 0;
            // 
            // Main
            // 
            Main.Controls.Add(label5);
            Main.Controls.Add(label6);
            Main.Controls.Add(label3);
            Main.Controls.Add(label4);
            Main.Controls.Add(label1);
            Main.Controls.Add(label2);
            Main.Controls.Add(okButton);
            Main.Controls.Add(label9);
            Main.Location = new Point(3, 3);
            Main.Name = "Main";
            Main.Size = new Size(333, 285);
            Main.TabIndex = 25;
            Main.Visible = false;
            // 
            // label5
            // 
            label5.BackColor = Color.Yellow;
            label5.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label5.Location = new Point(172, 183);
            label5.Name = "label5";
            label5.RightToLeft = RightToLeft.No;
            label5.Size = new Size(147, 29);
            label5.TabIndex = 31;
            label5.Tag = "3108";
            label5.Text = "ETC";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            label5.Click += label9_Click;
            // 
            // label6
            // 
            label6.BackColor = Color.Yellow;
            label6.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label6.Location = new Point(19, 183);
            label6.Name = "label6";
            label6.RightToLeft = RightToLeft.No;
            label6.Size = new Size(147, 29);
            label6.TabIndex = 30;
            label6.Tag = "3107";
            label6.Text = "Empty Bobbin";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            label6.Click += label9_Click;
            // 
            // label3
            // 
            label3.BackColor = Color.Yellow;
            label3.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label3.Location = new Point(172, 144);
            label3.Name = "label3";
            label3.RightToLeft = RightToLeft.No;
            label3.Size = new Size(147, 29);
            label3.TabIndex = 29;
            label3.Tag = "3106";
            label3.Text = "Small Quantity";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            label3.Click += label9_Click;
            // 
            // label4
            // 
            label4.BackColor = Color.Yellow;
            label4.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label4.Location = new Point(19, 144);
            label4.Name = "label4";
            label4.RightToLeft = RightToLeft.No;
            label4.Size = new Size(147, 29);
            label4.TabIndex = 28;
            label4.Tag = "3105";
            label4.Text = "Telescoping";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            label4.Click += label9_Click;
            // 
            // label1
            // 
            label1.BackColor = Color.Yellow;
            label1.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.Location = new Point(172, 102);
            label1.Name = "label1";
            label1.RightToLeft = RightToLeft.No;
            label1.Size = new Size(147, 29);
            label1.TabIndex = 27;
            label1.Tag = "3104";
            label1.Text = "Suface NG";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Click += label9_Click;
            // 
            // label2
            // 
            label2.BackColor = Color.Yellow;
            label2.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label2.Location = new Point(19, 102);
            label2.Name = "label2";
            label2.RightToLeft = RightToLeft.No;
            label2.Size = new Size(147, 29);
            label2.TabIndex = 26;
            label2.Tag = "3103";
            label2.Text = "Tab Wrinkle";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.Click += label9_Click;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.DialogResult = DialogResult.Cancel;
            okButton.Location = new Point(92, 234);
            okButton.Margin = new Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new Size(163, 38);
            okButton.TabIndex = 24;
            okButton.Text = "Select";
            okButton.Click += okButton_Click;
            // 
            // label9
            // 
            label9.BackColor = Color.Yellow;
            label9.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label9.Location = new Point(19, 20);
            label9.Name = "label9";
            label9.RightToLeft = RightToLeft.No;
            label9.Size = new Size(147, 29);
            label9.TabIndex = 5;
            label9.Tag = "1";
            label9.Text = "Normal";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            label9.Click += label9_Click;
            // 
            // LotEndTypeCut
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(362, 312);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LotEndTypeCut";
            Padding = new Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "EndWorkType";
            TopMost = true;
            tableLayoutPanel.ResumeLayout(false);
            Main.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button okButton;
        private Panel Main;
        private Label label9;
        private Label label2;
        private Label label1;
        private Label label5;
        private Label label6;
        private Label label3;
        private Label label4;
    }
}
