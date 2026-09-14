using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace LaptopKeyboardDisabler;

public partial class MainForm : Form
{
    private readonly InterceptionManager _interception = new();
    private HotkeyHelper? _hotkey;
    private bool _reallyExit = false;

    public MainForm(bool startMinimized = false)
    {
        InitializeComponent();
        InitializeCustomTheme();
        InitializeTray();

        this.Load += MainForm_Load;
        this.FormClosing += MainForm_FormClosing;

        if (startMinimized)
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }
    }

    private void InitializeTray()
    {
        notifyIcon.Text = "Laptop Keyboard Disabler";
        notifyIcon.Visible = true;
        notifyIcon.DoubleClick += (s, e) => ShowAndRestore();

        var menu = new ContextMenuStrip();

        var titleItem = new ToolStripMenuItem("Laptop Keyboard Disabler")
        {
            Enabled = false,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
        };
        menu.Items.Add(titleItem);
        menu.Items.Add(new ToolStripSeparator());

        var toggleItem = new ToolStripMenuItem("Bật / Tắt phím Laptop", null, (s, e) => ToggleLaptopKeyboard())
        {
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Name = "menuToggle"
        };
        menu.Items.Add(toggleItem);

        var startupItem = new ToolStripMenuItem("Khởi động cùng Windows", null, (s, e) =>
        {
            chkStartup.Checked = !chkStartup.Checked;
        })
        {
            Name = "menuStartup"
        };
        menu.Items.Add(startupItem);

        menu.Items.Add(new ToolStripSeparator());

        var showItem = new ToolStripMenuItem("Mở bảng điều khiển", null, (s, e) => ShowAndRestore());
        menu.Items.Add(showItem);

        var exitItem = new ToolStripMenuItem("Thoát hoàn toàn", null, (s, e) => ExitApplication())
        {
            ForeColor = Color.IndianRed
        };
        menu.Items.Add(exitItem);

        notifyIcon.ContextMenuStrip = menu;
    }

    private void InitializeCustomTheme()
    {
        this.BackColor = Color.FromArgb(245, 247, 250);
        this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        // Register Hotkey
        try
        {
            _hotkey = new HotkeyHelper(this.Handle);
            _hotkey.HotkeyPressed += () =>
            {
                if (chkHotkey.Checked && _interception.IsDriverInstalled)
                {
                    ToggleLaptopKeyboard();
                }
            };
            _hotkey.Register(Keys.K, ctrl: true, alt: true);
        }
        catch (Exception ex)
        {
            lblHotkeyStatus.Text = "Lỗi đăng ký Ctrl+Alt+K: " + ex.Message;
        }

        // Check Startup status
        chkStartup.Checked = StartupManager.IsStartupEnabled();
        chkAutoDisableOnBoot.Checked = _interception.Config.IsDisabledOnStartup;
        chkHotkey.Checked = _interception.Config.HotkeyEnabled;

        _interception.StatusChanged += (disabled) =>
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)UpdateUIState);
            }
        };

        _interception.DeviceIdentified += (deviceId, devName) =>
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    btnIdentify.Text = "🎯 Nhận Diện Bằng 1 Phím";
                    btnIdentify.BackColor = Color.FromArgb(241, 245, 249);
                    RefreshDeviceList();
                    MessageBox.Show($"Đã nhận diện thành công!\n\nBàn phím Laptop là Device #{deviceId}\n({devName})", 
                                    "Nhận diện bàn phím", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
        };

        // Start Interception if driver is installed
        if (_interception.IsDriverInstalled)
        {
            bool started = _interception.Start();
            if (!started)
            {
                lblStatusBadge.Text = "LỖI KHỞI TẠO DRIVER";
                lblStatusBadge.BackColor = Color.FromArgb(231, 76, 60);
                lblStatusDesc.Text = "Driver đã cài nhưng chưa nạp được vào nhân. Vui lòng restart máy nếu bạn vừa cài driver.";
            }
        }

        RefreshDeviceList();
        UpdateUIState();

        if (this.WindowState == FormWindowState.Minimized)
        {
            this.Hide();
            notifyIcon.ShowBalloonTip(2000, "Laptop Keyboard Disabler", "Ứng dụng đang chạy ngầm trong khay hệ thống.", ToolTipIcon.Info);
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (_hotkey != null && _hotkey.ProcessMessage(ref m))
        {
            return;
        }
        base.WndProc(ref m);
    }

    private void RefreshDeviceList()
    {
        cboDevices.Items.Clear();
        if (_interception.IsDriverInstalled)
        {
            var devices = _interception.GetKeyboardDevices();
            foreach (var d in devices)
            {
                cboDevices.Items.Add(d);
                if (d.DeviceId == _interception.LaptopDeviceId)
                {
                    cboDevices.SelectedItem = d;
                }
            }

            if (cboDevices.SelectedItem == null && cboDevices.Items.Count > 0)
            {
                cboDevices.SelectedIndex = 0;
            }
        }
    }

    private void UpdateUIState()
    {
        bool installed = _interception.IsDriverInstalled;

        if (!installed)
        {
            // Driver not installed state
            pnlDriverWarning.Visible = true;
            pnlNormalControls.Visible = false;
            btnToggle.Visible = false;

            lblStatusBadge.Text = "CHƯA CÀI DRIVER HỖ TRỢ";
            lblStatusBadge.BackColor = Color.FromArgb(243, 156, 18); // Amber
            lblStatusBadge.ForeColor = Color.White;
            lblStatusDesc.Text = "Để tắt phím laptop mà không ảnh hưởng đến phím rời USB, cần cài driver hỗ trợ 1 lần duy nhất.";

            UpdateTrayAppearance(isLaptopActive: true, driverInstalled: false);
            return;
        }

        // Driver is installed!
        pnlDriverWarning.Visible = false;
        pnlNormalControls.Visible = true;
        btnToggle.Visible = true;

        bool isLaptopDisabled = _interception.IsLaptopDisabled;

        if (isLaptopDisabled)
        {
            lblStatusBadge.Text = "BÀN PHÍM LAPTOP: ĐÃ TẮT 🔴";
            lblStatusBadge.BackColor = Color.FromArgb(231, 76, 60); // Red
            lblStatusBadge.ForeColor = Color.White;
            lblStatusDesc.Text = $"Bàn phím laptop (Device #{_interception.LaptopDeviceId}) đang bị ngắt hoàn toàn. Bàn phím rời USB gõ 100% mượt mà.";

            btnToggle.Text = "🟢 BẬT LẠI BÀN PHÍM LAPTOP (ENABLE)";
            btnToggle.BackColor = Color.FromArgb(39, 174, 96);
            btnToggle.ForeColor = Color.White;
        }
        else
        {
            lblStatusBadge.Text = "BÀN PHÍM LAPTOP: ĐANG BẬT 🟢";
            lblStatusBadge.BackColor = Color.FromArgb(39, 174, 96); // Green
            lblStatusBadge.ForeColor = Color.White;
            lblStatusDesc.Text = "Bàn phím laptop đang nhận tín hiệu bình thường cùng với bàn phím rời.";

            btnToggle.Text = "🔴 TẮT BÀN PHÍM LAPTOP (DISABLE)";
            btnToggle.BackColor = Color.FromArgb(231, 76, 60);
            btnToggle.ForeColor = Color.White;
        }

        UpdateTrayAppearance(isLaptopActive: !isLaptopDisabled, driverInstalled: true);
    }

    private void UpdateTrayAppearance(bool isLaptopActive, bool driverInstalled)
    {
        using (var bmp = new Bitmap(16, 16))
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            Color circleColor;
            if (!driverInstalled)
                circleColor = Color.FromArgb(243, 156, 18);
            else if (isLaptopActive)
                circleColor = Color.FromArgb(46, 204, 113);
            else
                circleColor = Color.FromArgb(231, 76, 60);

            using (var brush = new SolidBrush(circleColor))
            {
                g.FillEllipse(brush, 1, 1, 14, 14);
            }
            using (var pen = new Pen(Color.White, 1.5f))
            {
                g.DrawEllipse(pen, 1, 1, 14, 14);
                if (isLaptopActive)
                {
                    using var f = new Font("Arial", 8, FontStyle.Bold);
                    g.DrawString("K", f, Brushes.White, 3, 1);
                }
                else
                {
                    g.DrawLine(pen, 4, 4, 12, 12);
                    g.DrawLine(pen, 12, 4, 4, 12);
                }
            }

            IntPtr hIcon = bmp.GetHicon();
            notifyIcon.Icon = Icon.FromHandle(hIcon);
        }

        if (notifyIcon.ContextMenuStrip != null)
        {
            var item = notifyIcon.ContextMenuStrip.Items["menuToggle"];
            if (item != null)
            {
                item.Enabled = driverInstalled;
                item.Text = isLaptopActive ? "🔴 Tắt bàn phím Laptop" : "🟢 Bật lại bàn phím Laptop";
            }
            var startItem = notifyIcon.ContextMenuStrip.Items["menuStartup"] as ToolStripMenuItem;
            if (startItem != null) startItem.Checked = chkStartup.Checked;
        }
    }

    private void ToggleLaptopKeyboard()
    {
        if (!_interception.IsDriverInstalled)
        {
            MessageBox.Show("Vui lòng cài đặt driver hỗ trợ trước khi thực hiện thao tác này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _interception.ToggleLaptopKeyboard();

        if (_interception.IsLaptopDisabled)
        {
            notifyIcon.ShowBalloonTip(2000, "Bàn phím Laptop", "Đã TẮT bàn phím laptop thành công!\n(Phím tắt hoàn tác: Ctrl + Alt + K)", ToolTipIcon.Warning);
        }
        else
        {
            notifyIcon.ShowBalloonTip(2000, "Bàn phím Laptop", "Đã BẬT lại bàn phím laptop thành công!", ToolTipIcon.Info);
        }
    }

    private void btnToggle_Click(object? sender, EventArgs e)
    {
        ToggleLaptopKeyboard();
    }

    private void btnInstallDriver_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "Cài đặt driver Interception để cho phép tắt phím laptop trong 0 giây không cần restart.\n\n" +
            "Quá trình này cần quyền Administrator và sẽ yêu cầu KHỞI ĐỘNG LẠI MÁY 1 LẦN sau khi cài xong.\n\n" +
            "Bạn có muốn tiến hành cài đặt ngay không?",
            "Xác nhận cài đặt Driver",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        Cursor.Current = Cursors.WaitCursor;
        bool success = InterceptionManager.InstallDriver();
        Cursor.Current = Cursors.Default;

        if (success)
        {
            var rebootNow = MessageBox.Show(
                "✅ ĐÃ CÀI ĐẶT DRIVER THÀNH CÔNG!\n\n" +
                "Bạn cần Khởi động lại máy tính (Restart) một lần để Windows nạp driver.\n\n" +
                "Bạn có muốn KHỞI ĐỘNG LẠI MÁY NGAY BÂY GIỜ không?",
                "Khởi động lại máy",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);

            if (rebootNow == DialogResult.Yes)
            {
                Process.Start("shutdown.exe", "/r /t 5 /c \"Khoi dong lai de kich hoat driver Interception...\"");
                Application.Exit();
            }
        }
        else
        {
            MessageBox.Show("Cài đặt driver thất bại hoặc bạn đã hủy hộp thoại UAC.", "Lỗi cài đặt", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        UpdateUIState();
    }

    private void btnSetMousePriority_Click(object? sender, EventArgs e)
    {
        try
        {
            Program.SetMouseHighestPriority();
            MessageBox.Show(
                "✅ ĐÃ THIẾT LẬP MỨC ƯU TIÊN CAO NHẤT CHO DRIVER CHUỘT!\n\n" +
                "1. mouclass & mouhid: Khởi động cùng kernel (System Start, Group Pointer).\n" +
                "2. Ngắt phần cứng (Hardware IRQ): DevicePriority = 3 (High) cho Touchpad & Chuột.\n" +
                "3. Luồng bắt phím: Chế độ Normal cân bằng tài nguyên, không chiếm CPU của chuột.\n\n" +
                "Đã áp dụng ngay lập tức vào Registry Windows!",
                "Ưu tiên chuột cao nhất",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnUninstallDriver_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "Bạn có chắc chắn muốn gỡ cài đặt driver Interception khỏi máy tính không?\n(Sẽ cần khởi động lại máy để gỡ bỏ hoàn toàn)",
            "Xác nhận gỡ driver",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes) return;

        _interception.Stop();
        bool success = InterceptionManager.UninstallDriver();
        if (success)
        {
            MessageBox.Show("Đã gửi lệnh gỡ cài đặt driver. Vui lòng khởi động lại máy để hoàn tất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        UpdateUIState();
    }

    private void btnIdentify_Click(object? sender, EventArgs e)
    {
        btnIdentify.Text = "⏳ Đang chờ... Hãy nhấn 1 phím trên laptop";
        btnIdentify.BackColor = Color.FromArgb(254, 240, 138); // Yellow highlight
        _interception.StartIdentifyingDevice();
    }

    private void cboDevices_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cboDevices.SelectedItem is KeyboardDeviceItem item)
        {
            _interception.SetLaptopDeviceId(item.DeviceId);
        }
    }

    private void chkStartup_CheckedChanged(object? sender, EventArgs e)
    {
        bool enable = chkStartup.Checked;
        var (success, msg) = StartupManager.SetStartup(enable);
        if (!success)
        {
            MessageBox.Show(msg, "Thông báo Startup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            chkStartup.CheckedChanged -= chkStartup_CheckedChanged;
            chkStartup.Checked = StartupManager.IsStartupEnabled();
            chkStartup.CheckedChanged += chkStartup_CheckedChanged;
        }
        else
        {
            notifyIcon.ShowBalloonTip(2000, "Khởi động cùng Windows", msg, ToolTipIcon.Info);
        }
    }

    private void chkAutoDisableOnBoot_CheckedChanged(object? sender, EventArgs e)
    {
        _interception.Config.IsDisabledOnStartup = chkAutoDisableOnBoot.Checked;
        _interception.SaveConfig();
    }

    private void chkHotkey_CheckedChanged(object? sender, EventArgs e)
    {
        _interception.Config.HotkeyEnabled = chkHotkey.Checked;
        _interception.SaveConfig();
    }

    private void btnMinimizeToTray_Click(object? sender, EventArgs e)
    {
        this.Hide();
        notifyIcon.ShowBalloonTip(1500, "Laptop Keyboard Disabler", "Ứng dụng đã thu nhỏ xuống khay hệ thống. Double-click icon để mở lại.", ToolTipIcon.Info);
    }

    private void ShowAndRestore()
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.ShowInTaskbar = true;
        this.BringToFront();
        this.Activate();
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!_reallyExit && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.Hide();
            notifyIcon.ShowBalloonTip(1500, "Laptop Keyboard Disabler", "Ứng dụng vẫn chạy ngầm dưới khay hệ thống.", ToolTipIcon.Info);
        }
    }

    private void ExitApplication()
    {
        if (_interception.IsLaptopDisabled)
        {
            var result = MessageBox.Show(
                "Bàn phím laptop đang bị TẮT!\n\nBạn có muốn BẬT LẠI bàn phím laptop trước khi thoát hoàn toàn không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Cancel) return;

            if (result == DialogResult.Yes)
            {
                _interception.SetLaptopKeyboardDisabled(false);
            }
        }

        _reallyExit = true;
        _hotkey?.Dispose();
        _interception.Dispose();
        notifyIcon.Visible = false;
        Application.Exit();
    }
}
