using System;
using System.Drawing;
using System.Windows.Forms;


namespace Equipment_Interface_Demo
{
    public partial class PlcMonitorControl : UserControl
    {
        private readonly RichTextBox _console;
        private readonly Button _btnClear;
        private readonly Label _title;

        public PlcMonitorControl()
        {
            BackColor = Color.FromArgb(30, 30, 30);

            _title = new Label
            {
                Text = "PLC I/O MONITOR",
                Dock = DockStyle.Top,
                Height = 36,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(45, 45, 48),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            _btnClear = new Button
            {
                Text = "CLEAR",
                Dock = DockStyle.Bottom,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White
            };

            _btnClear.FlatAppearance.BorderSize = 0;
            _btnClear.Click += delegate { Clear(); };

            _console = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                WordWrap = false,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(20, 20, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Consolas", 9.5F),
                DetectUrls = false
            };

            Controls.Add(_console);
            Controls.Add(_btnClear);
            Controls.Add(_title);
        }

        public void Write(string address, object value)
        {
            Append("WRITE", address, value, Color.DeepSkyBlue);
        }

        public void Read(string address, object value)
        {
            Append("READ", address, value, Color.LightGreen);
        }

        public void Event(string message)
        {
            AppendMessage("EVENT", message, Color.Gold);
        }

        public void Error(string message)
        {
            AppendMessage("ERROR", message, Color.OrangeRed);
        }

        public void Info(string message)
        {
            AppendMessage("INFO", message, Color.LightGray);
        }

        public void Clear()
        {
            if (_console.InvokeRequired)
            {
                _console.BeginInvoke(new Action(Clear));
                return;
            }

            _console.Clear();
        }

        private void Append(
            string type,
            string address,
            object value,
            Color color)
        {
            string text = string.Format(
                "{0:HH:mm:ss.fff}  {1,-6} {2,-10} {3}",
                DateTime.Now,
                type,
                address,
                FormatValue(value));

            AppendLine(text, color);
        }

        private void AppendMessage(
            string type,
            string message,
            Color color)
        {
            string text = string.Format(
                "{0:HH:mm:ss.fff}  {1,-6} {2}",
                DateTime.Now,
                type,
                message);

            AppendLine(text, color);
        }

        private void AppendLine(string text, Color color)
        {
            if (_console.InvokeRequired)
            {
                _console.BeginInvoke(
                    new Action<string, Color>(AppendLine),
                    text,
                    color);

                return;
            }

            _console.SelectionStart = _console.TextLength;
            _console.SelectionLength = 0;
            _console.SelectionColor = color;

            _console.AppendText(text + Environment.NewLine);

            _console.SelectionColor = _console.ForeColor;

            // 자동 스크롤
            _console.SelectionStart = _console.TextLength;
            _console.ScrollToCaret();
        }

        private string FormatValue(object value)
        {
            if (value == null)
                return "NULL";

            if (value is bool)
                return (bool)value ? "ON" : "OFF";

            return value.ToString();
        }
    }
}
