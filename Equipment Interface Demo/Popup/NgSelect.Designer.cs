namespace Equipment_Interface_Demo.Popup
{
    partial class NgSelect
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
            label3 = new Label();
            txtkeyIn = new TextBox();
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
            tableLayoutPanel.Size = new Size(324, 145);
            tableLayoutPanel.TabIndex = 0;
            // 
            // Main
            // 
            Main.Controls.Add(label3);
            Main.Controls.Add(txtkeyIn);
            Main.Controls.Add(label1);
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
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("맑은 고딕", 8.25F);
            label3.Location = new Point(15, 16);
            label3.Name = "label3";
            label3.Size = new Size(44, 13);
            label3.TabIndex = 29;
            label3.Text = "INPUT :";
            // 
            // txtkeyIn
            // 
            txtkeyIn.Font = new Font("맑은 고딕", 8.25F);
            txtkeyIn.Location = new Point(65, 13);
            txtkeyIn.Name = "txtkeyIn";
            txtkeyIn.Size = new Size(148, 22);
            txtkeyIn.TabIndex = 28;
            // 
            // label1
            // 
            label1.BackColor = Color.Yellow;
            label1.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label1.Location = new Point(212, 50);
            label1.Name = "label1";
            label1.RightToLeft = RightToLeft.No;
            label1.Size = new Size(96, 29);
            label1.TabIndex = 27;
            label1.Tag = "3";
            label1.Text = "강제배출";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Click += label9_Click;
            // 
            // label2
            // 
            label2.BackColor = Color.Yellow;
            label2.Font = new Font("맑은 고딕", 12F, FontStyle.Bold, GraphicsUnit.Point, 129);
            label2.Location = new Point(110, 50);
            label2.Name = "label2";
            label2.RightToLeft = RightToLeft.No;
            label2.Size = new Size(96, 29);
            label2.TabIndex = 26;
            label2.Tag = "2";
            label2.Text = "Retry";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.Click += label9_Click;
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.DialogResult = DialogResult.Cancel;
            okButton.Location = new Point(65, 101);
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
            label9.Location = new Point(8, 50);
            label9.Name = "label9";
            label9.RightToLeft = RightToLeft.No;
            label9.Size = new Size(96, 29);
            label9.TabIndex = 5;
            label9.Tag = "1";
            label9.Text = "Key-In";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            label9.Click += label9_Click;
            // 
            // NgSelect
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(344, 165);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "NgSelect";
            Padding = new Padding(10);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select";
            TopMost = true;
            tableLayoutPanel.ResumeLayout(false);
            Main.ResumeLayout(false);
            Main.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button okButton;
        private Panel Main;
        private Label label9;
        private Label label2;
        private Label label1;
        private TextBox txtkeyIn;
        private Label label3;
    }
}
