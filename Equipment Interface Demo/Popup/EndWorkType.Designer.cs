namespace Equipment_Interface_Demo.Popup
{
    partial class EndWorkType
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
            tableLayoutPanel.Size = new Size(195, 317);
            tableLayoutPanel.TabIndex = 0;
            // 
            // Main
            // 
            Main.Controls.Add(label2);
            Main.Controls.Add(okButton);
            Main.Controls.Add(label9);
            Main.Dock = DockStyle.Fill;
            Main.Location = new Point(3, 3);
            Main.Name = "Main";
            Main.Size = new Size(663, 311);
            Main.TabIndex = 25;
            Main.Visible = false;
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
            label2.Tag = "3";
            label2.Text = "조건 조정";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.Click += label9_Click;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.DialogResult = DialogResult.Cancel;
            okButton.Location = new Point(19, 251);
            okButton.Margin = new Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new Size(163, 38);
            okButton.TabIndex = 24;
            okButton.Text = "Close";
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
            // EndWorkType
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(215, 337);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EndWorkType";
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
    }
}
