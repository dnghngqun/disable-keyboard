namespace LaptopKeyboardDisabler;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblAppTitle;
    private System.Windows.Forms.Label lblAppSubtitle;

    private System.Windows.Forms.Panel pnlStatusCard;
    private System.Windows.Forms.Label lblStatusBadge;
    private System.Windows.Forms.Label lblStatusDesc;
    private System.Windows.Forms.Button btnToggle;

    private System.Windows.Forms.Panel pnlDriverWarning;
    private System.Windows.Forms.Label lblDriverWarningTitle;
    private System.Windows.Forms.Label lblDriverWarningDesc;
    private System.Windows.Forms.Button btnInstallDriver;

    private System.Windows.Forms.Panel pnlNormalControls;
    private System.Windows.Forms.Label lblDeviceTitle;
    private System.Windows.Forms.ComboBox cboDevices;
    private System.Windows.Forms.Button btnIdentify;
    private System.Windows.Forms.CheckBox chkStartup;
    private System.Windows.Forms.CheckBox chkAutoDisableOnBoot;
    private System.Windows.Forms.CheckBox chkHotkey;
    private System.Windows.Forms.Label lblHotkeyStatus;
    private System.Windows.Forms.Button btnUninstallDriver;
    private System.Windows.Forms.Button btnSetMousePriority;

    private System.Windows.Forms.Button btnMinimizeToTray;
    private System.Windows.Forms.NotifyIcon notifyIcon;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        this.pnlHeader = new System.Windows.Forms.Panel();
        this.lblAppTitle = new System.Windows.Forms.Label();
        this.lblAppSubtitle = new System.Windows.Forms.Label();

        this.pnlStatusCard = new System.Windows.Forms.Panel();
        this.lblStatusBadge = new System.Windows.Forms.Label();
        this.lblStatusDesc = new System.Windows.Forms.Label();
        this.btnToggle = new System.Windows.Forms.Button();

        this.pnlDriverWarning = new System.Windows.Forms.Panel();
        this.lblDriverWarningTitle = new System.Windows.Forms.Label();
        this.lblDriverWarningDesc = new System.Windows.Forms.Label();
        this.btnInstallDriver = new System.Windows.Forms.Button();

        this.pnlNormalControls = new System.Windows.Forms.Panel();
        this.lblDeviceTitle = new System.Windows.Forms.Label();
        this.cboDevices = new System.Windows.Forms.ComboBox();
        this.btnIdentify = new System.Windows.Forms.Button();
        this.chkStartup = new System.Windows.Forms.CheckBox();
        this.chkAutoDisableOnBoot = new System.Windows.Forms.CheckBox();
        this.chkHotkey = new System.Windows.Forms.CheckBox();
        this.lblHotkeyStatus = new System.Windows.Forms.Label();
        this.btnUninstallDriver = new System.Windows.Forms.Button();
        this.btnSetMousePriority = new System.Windows.Forms.Button();

        this.btnMinimizeToTray = new System.Windows.Forms.Button();
        this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);

        this.pnlHeader.SuspendLayout();
        this.pnlStatusCard.SuspendLayout();
        this.pnlDriverWarning.SuspendLayout();
        this.pnlNormalControls.SuspendLayout();
        this.SuspendLayout();

        // 
        // pnlHeader
        // 
        this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
        this.pnlHeader.Controls.Add(this.lblAppTitle);
        this.pnlHeader.Controls.Add(this.lblAppSubtitle);
        this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlHeader.Location = new System.Drawing.Point(0, 0);
        this.pnlHeader.Name = "pnlHeader";
        this.pnlHeader.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
        this.pnlHeader.Size = new System.Drawing.Size(684, 75);
        this.pnlHeader.TabIndex = 0;

        // 
        // lblAppTitle
        // 
        this.lblAppTitle.AutoSize = true;
        this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
        this.lblAppTitle.ForeColor = System.Drawing.Color.White;
        this.lblAppTitle.Location = new System.Drawing.Point(18, 12);
        this.lblAppTitle.Name = "lblAppTitle";
        this.lblAppTitle.Size = new System.Drawing.Size(268, 28);
        this.lblAppTitle.TabIndex = 0;
        this.lblAppTitle.Text = "Laptop Keyboard Disabler";

        // 
        // lblAppSubtitle
        // 
        this.lblAppSubtitle.AutoSize = true;
        this.lblAppSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblAppSubtitle.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
        this.lblAppSubtitle.Location = new System.Drawing.Point(20, 43);
        this.lblAppSubtitle.Name = "lblAppSubtitle";
        this.lblAppSubtitle.Size = new System.Drawing.Size(435, 15);
        this.lblAppSubtitle.TabIndex = 1;
        this.lblAppSubtitle.Text = "Tắt/Bật phím laptop tức thì không cần restart • Phím tắt toàn cục: Ctrl + Alt + K";

        // 
        // pnlStatusCard
        // 
        this.pnlStatusCard.BackColor = System.Drawing.Color.White;
        this.pnlStatusCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pnlStatusCard.Controls.Add(this.lblStatusBadge);
        this.pnlStatusCard.Controls.Add(this.lblStatusDesc);
        this.pnlStatusCard.Controls.Add(this.btnToggle);
        this.pnlStatusCard.Location = new System.Drawing.Point(20, 85);
        this.pnlStatusCard.Name = "pnlStatusCard";
        this.pnlStatusCard.Size = new System.Drawing.Size(644, 130);
        this.pnlStatusCard.TabIndex = 1;

        // 
        // lblStatusBadge
        // 
        this.lblStatusBadge.AutoSize = true;
        this.lblStatusBadge.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
        this.lblStatusBadge.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
        this.lblStatusBadge.ForeColor = System.Drawing.Color.White;
        this.lblStatusBadge.Location = new System.Drawing.Point(20, 14);
        this.lblStatusBadge.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
        this.lblStatusBadge.Name = "lblStatusBadge";
        this.lblStatusBadge.Size = new System.Drawing.Size(206, 27);
        this.lblStatusBadge.TabIndex = 0;
        this.lblStatusBadge.Text = "BÀN PHÍM LAPTOP: ĐANG BẬT";

        // 
        // lblStatusDesc
        // 
        this.lblStatusDesc.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lblStatusDesc.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
        this.lblStatusDesc.Location = new System.Drawing.Point(20, 46);
        this.lblStatusDesc.Name = "lblStatusDesc";
        this.lblStatusDesc.Size = new System.Drawing.Size(602, 22);
        this.lblStatusDesc.TabIndex = 1;
        this.lblStatusDesc.Text = "Bàn phím laptop đang nhận tín hiệu bình thường.";

        // 
        // btnToggle
        // 
        this.btnToggle.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
        this.btnToggle.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnToggle.FlatAppearance.BorderSize = 0;
        this.btnToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnToggle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
        this.btnToggle.ForeColor = System.Drawing.Color.White;
        this.btnToggle.Location = new System.Drawing.Point(20, 72);
        this.btnToggle.Name = "btnToggle";
        this.btnToggle.Size = new System.Drawing.Size(602, 44);
        this.btnToggle.TabIndex = 2;
        this.btnToggle.Text = "🔴 TẮT BÀN PHÍM LAPTOP (DISABLE)";
        this.btnToggle.UseVisualStyleBackColor = false;
        this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);

        // 
        // pnlDriverWarning
        // 
        this.pnlDriverWarning.BackColor = System.Drawing.Color.FromArgb(254, 252, 232); // Amber light
        this.pnlDriverWarning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pnlDriverWarning.Controls.Add(this.lblDriverWarningTitle);
        this.pnlDriverWarning.Controls.Add(this.lblDriverWarningDesc);
        this.pnlDriverWarning.Controls.Add(this.btnInstallDriver);
        this.pnlDriverWarning.Location = new System.Drawing.Point(20, 225);
        this.pnlDriverWarning.Name = "pnlDriverWarning";
        this.pnlDriverWarning.Size = new System.Drawing.Size(644, 290);
        this.pnlDriverWarning.TabIndex = 2;

        // 
        // lblDriverWarningTitle
        // 
        this.lblDriverWarningTitle.AutoSize = true;
        this.lblDriverWarningTitle.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold);
        this.lblDriverWarningTitle.ForeColor = System.Drawing.Color.FromArgb(161, 98, 7);
        this.lblDriverWarningTitle.Location = new System.Drawing.Point(20, 15);
        this.lblDriverWarningTitle.Name = "lblDriverWarningTitle";
        this.lblDriverWarningTitle.Size = new System.Drawing.Size(370, 21);
        this.lblDriverWarningTitle.TabIndex = 0;
        this.lblDriverWarningTitle.Text = "⚠️ Cần Cài Đặt Driver Hỗ Trợ (Chỉ 1 Lần Duy Nhất)";

        // 
        // lblDriverWarningDesc
        // 
        this.lblDriverWarningDesc.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.lblDriverWarningDesc.ForeColor = System.Drawing.Color.FromArgb(113, 63, 18);
        this.lblDriverWarningDesc.Location = new System.Drawing.Point(20, 48);
        this.lblDriverWarningDesc.Name = "lblDriverWarningDesc";
        this.lblDriverWarningDesc.Size = new System.Drawing.Size(602, 140);
        this.lblDriverWarningDesc.TabIndex = 1;
        this.lblDriverWarningDesc.Text = "Do cơ chế bảo vệ của Windows cấm phần mềm vô hiệu hóa trực tiếp bàn phím laptop lúc đang chạy, ứng dụng sử dụng driver bộ lọc phần cứng Interception (mã nguồn mở an toàn).\n\n" +
            "• Chặn tín hiệu phím laptop trực tiếp ở cấp độ nhân hệ điều hành.\n" +
            "• Bàn phím rời USB gõ mượt mà 100%, không bị ảnh hưởng.\n" +
            "• Bật / Tắt tức thì trong 0 giây, KHÔNG BAO GIỜ CẦN RESTART NỮA.\n\n" +
            "👉 Hãy nhấn nút bên dưới để cài driver (chỉ cần khởi động lại máy 1 lần sau khi cài):";

        // 
        // btnInstallDriver
        // 
        this.btnInstallDriver.BackColor = System.Drawing.Color.FromArgb(217, 119, 6);
        this.btnInstallDriver.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnInstallDriver.FlatAppearance.BorderSize = 0;
        this.btnInstallDriver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnInstallDriver.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
        this.btnInstallDriver.ForeColor = System.Drawing.Color.White;
        this.btnInstallDriver.Location = new System.Drawing.Point(20, 205);
        this.btnInstallDriver.Name = "btnInstallDriver";
        this.btnInstallDriver.Size = new System.Drawing.Size(602, 48);
        this.btnInstallDriver.TabIndex = 2;
        this.btnInstallDriver.Text = "📥 CÀI ĐẶT DRIVER INTERCEPTION NGAY";
        this.btnInstallDriver.UseVisualStyleBackColor = false;
        this.btnInstallDriver.Click += new System.EventHandler(this.btnInstallDriver_Click);

        // 
        // pnlNormalControls
        // 
        this.pnlNormalControls.BackColor = System.Drawing.Color.White;
        this.pnlNormalControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pnlNormalControls.Controls.Add(this.lblDeviceTitle);
        this.pnlNormalControls.Controls.Add(this.cboDevices);
        this.pnlNormalControls.Controls.Add(this.btnIdentify);
        this.pnlNormalControls.Controls.Add(this.chkStartup);
        this.pnlNormalControls.Controls.Add(this.chkAutoDisableOnBoot);
        this.pnlNormalControls.Controls.Add(this.chkHotkey);
        this.pnlNormalControls.Controls.Add(this.lblHotkeyStatus);
        this.pnlNormalControls.Controls.Add(this.btnUninstallDriver);
        this.pnlNormalControls.Controls.Add(this.btnSetMousePriority);
        this.pnlNormalControls.Location = new System.Drawing.Point(20, 225);
        this.pnlNormalControls.Name = "pnlNormalControls";
        this.pnlNormalControls.Size = new System.Drawing.Size(644, 290);
        this.pnlNormalControls.TabIndex = 3;

        // 
        // lblDeviceTitle
        // 
        this.lblDeviceTitle.AutoSize = true;
        this.lblDeviceTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblDeviceTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
        this.lblDeviceTitle.Location = new System.Drawing.Point(18, 14);
        this.lblDeviceTitle.Name = "lblDeviceTitle";
        this.lblDeviceTitle.Size = new System.Drawing.Size(183, 19);
        this.lblDeviceTitle.TabIndex = 0;
        this.lblDeviceTitle.Text = "Bàn phím Laptop mục tiêu:";

        // 
        // cboDevices
        // 
        this.cboDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cboDevices.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        this.cboDevices.FormattingEnabled = true;
        this.cboDevices.Location = new System.Drawing.Point(20, 38);
        this.cboDevices.Name = "cboDevices";
        this.cboDevices.Size = new System.Drawing.Size(370, 25);
        this.cboDevices.TabIndex = 1;
        this.cboDevices.SelectedIndexChanged += new System.EventHandler(this.cboDevices_SelectedIndexChanged);

        // 
        // btnIdentify
        // 
        this.btnIdentify.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
        this.btnIdentify.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnIdentify.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
        this.btnIdentify.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnIdentify.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.btnIdentify.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
        this.btnIdentify.Location = new System.Drawing.Point(400, 36);
        this.btnIdentify.Name = "btnIdentify";
        this.btnIdentify.Size = new System.Drawing.Size(222, 28);
        this.btnIdentify.TabIndex = 2;
        this.btnIdentify.Text = "🎯 Nhận Diện Bằng 1 Phím";
        this.btnIdentify.UseVisualStyleBackColor = false;
        this.btnIdentify.Click += new System.EventHandler(this.btnIdentify_Click);

        // 
        // chkStartup
        // 
        this.chkStartup.AutoSize = true;
        this.chkStartup.Cursor = System.Windows.Forms.Cursors.Hand;
        this.chkStartup.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkStartup.Location = new System.Drawing.Point(20, 85);
        this.chkStartup.Name = "chkStartup";
        this.chkStartup.Size = new System.Drawing.Size(435, 19);
        this.chkStartup.TabIndex = 3;
        this.chkStartup.Text = "🚀 Khởi động cùng Windows (Tự động chạy quyền Admin, không hỏi popup UAC)";
        this.chkStartup.UseVisualStyleBackColor = true;
        this.chkStartup.CheckedChanged += new System.EventHandler(this.chkStartup_CheckedChanged);

        // 
        // chkAutoDisableOnBoot
        // 
        this.chkAutoDisableOnBoot.AutoSize = true;
        this.chkAutoDisableOnBoot.Cursor = System.Windows.Forms.Cursors.Hand;
        this.chkAutoDisableOnBoot.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkAutoDisableOnBoot.Location = new System.Drawing.Point(20, 115);
        this.chkAutoDisableOnBoot.Name = "chkAutoDisableOnBoot";
        this.chkAutoDisableOnBoot.Size = new System.Drawing.Size(325, 19);
        this.chkAutoDisableOnBoot.TabIndex = 4;
        this.chkAutoDisableOnBoot.Text = "🔒 Luôn tự động tắt phím laptop mỗi khi ứng dụng khởi chạy";
        this.chkAutoDisableOnBoot.UseVisualStyleBackColor = true;
        this.chkAutoDisableOnBoot.CheckedChanged += new System.EventHandler(this.chkAutoDisableOnBoot_CheckedChanged);

        // 
        // chkHotkey
        // 
        this.chkHotkey.AutoSize = true;
        this.chkHotkey.Checked = true;
        this.chkHotkey.CheckState = System.Windows.Forms.CheckState.Checked;
        this.chkHotkey.Cursor = System.Windows.Forms.Cursors.Hand;
        this.chkHotkey.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.chkHotkey.Location = new System.Drawing.Point(20, 145);
        this.chkHotkey.Name = "chkHotkey";
        this.chkHotkey.Size = new System.Drawing.Size(248, 19);
        this.chkHotkey.TabIndex = 5;
        this.chkHotkey.Text = "⌨️ Bật phím tắt toàn cục: Ctrl + Alt + K";
        this.chkHotkey.UseVisualStyleBackColor = true;
        this.chkHotkey.CheckedChanged += new System.EventHandler(this.chkHotkey_CheckedChanged);

        // 
        // lblHotkeyStatus
        // 
        this.lblHotkeyStatus.AutoSize = true;
        this.lblHotkeyStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
        this.lblHotkeyStatus.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
        this.lblHotkeyStatus.Location = new System.Drawing.Point(275, 147);
        this.lblHotkeyStatus.Name = "lblHotkeyStatus";
        this.lblHotkeyStatus.Size = new System.Drawing.Size(200, 15);
        this.lblHotkeyStatus.TabIndex = 6;
        this.lblHotkeyStatus.Text = "(Nhấn Ctrl+Alt+K ở bất kỳ đâu để đổi)";

        // 
        // btnUninstallDriver
        // 
        this.btnUninstallDriver.BackColor = System.Drawing.Color.FromArgb(254, 242, 242);
        this.btnUninstallDriver.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnUninstallDriver.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(254, 202, 202);
        this.btnUninstallDriver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnUninstallDriver.Font = new System.Drawing.Font("Segoe UI", 8.5F);
        this.btnUninstallDriver.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
        this.btnUninstallDriver.Location = new System.Drawing.Point(20, 245);
        this.btnUninstallDriver.Name = "btnUninstallDriver";
        this.btnUninstallDriver.Size = new System.Drawing.Size(220, 28);
        this.btnUninstallDriver.TabIndex = 7;
        this.btnUninstallDriver.Text = "🗑 Gỡ cài đặt Driver Interception";
        this.btnUninstallDriver.UseVisualStyleBackColor = false;
        this.btnUninstallDriver.Click += new System.EventHandler(this.btnUninstallDriver_Click);
        // 
        // btnSetMousePriority
        // 
        this.btnSetMousePriority.BackColor = System.Drawing.Color.FromArgb(240, 253, 244);
        this.btnSetMousePriority.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnSetMousePriority.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(187, 247, 208);
        this.btnSetMousePriority.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnSetMousePriority.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.btnSetMousePriority.ForeColor = System.Drawing.Color.FromArgb(21, 128, 61);
        this.btnSetMousePriority.Location = new System.Drawing.Point(20, 185);
        this.btnSetMousePriority.Name = "btnSetMousePriority";
        this.btnSetMousePriority.Size = new System.Drawing.Size(320, 34);
        this.btnSetMousePriority.TabIndex = 6;
        this.btnSetMousePriority.Text = "⚡ Ưu Tiên Driver Chuột Cao Nhất (High Priority)";
        this.btnSetMousePriority.UseVisualStyleBackColor = false;
        this.btnSetMousePriority.Click += new System.EventHandler(this.btnSetMousePriority_Click);

        // 
        // btnMinimizeToTray
        // 
        this.btnMinimizeToTray.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
        this.btnMinimizeToTray.Cursor = System.Windows.Forms.Cursors.Hand;
        this.btnMinimizeToTray.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
        this.btnMinimizeToTray.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnMinimizeToTray.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.btnMinimizeToTray.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
        this.btnMinimizeToTray.Location = new System.Drawing.Point(490, 528);
        this.btnMinimizeToTray.Name = "btnMinimizeToTray";
        this.btnMinimizeToTray.Size = new System.Drawing.Size(174, 32);
        this.btnMinimizeToTray.TabIndex = 4;
        this.btnMinimizeToTray.Text = "Thu Nhỏ Xuống Khay 🗕";
        this.btnMinimizeToTray.UseVisualStyleBackColor = false;
        this.btnMinimizeToTray.Click += new System.EventHandler(this.btnMinimizeToTray_Click);

        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(684, 575);
        this.Controls.Add(this.btnMinimizeToTray);
        this.Controls.Add(this.pnlNormalControls);
        this.Controls.Add(this.pnlDriverWarning);
        this.Controls.Add(this.pnlStatusCard);
        this.Controls.Add(this.pnlHeader);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Laptop Keyboard Disabler";

        this.pnlHeader.ResumeLayout(false);
        this.pnlHeader.PerformLayout();
        this.pnlStatusCard.ResumeLayout(false);
        this.pnlStatusCard.PerformLayout();
        this.pnlDriverWarning.ResumeLayout(false);
        this.pnlDriverWarning.PerformLayout();
        this.pnlNormalControls.ResumeLayout(false);
        this.pnlNormalControls.PerformLayout();
        this.ResumeLayout(false);
    }
}
