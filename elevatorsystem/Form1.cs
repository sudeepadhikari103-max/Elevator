// State Pattern Implementation for Elevator System
// Button UI Implementation

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;
using elevatorsystem.Database;

namespace elevatorsystem
{

    public partial class Elevatorsystem : Form
    {

        private readonly Queue<int> _requestQueue = new();

        private readonly ElevatorLogService _logService = new();

        private readonly System.Windows.Forms.Timer _moveTimer = new();

        private readonly System.Windows.Forms.Timer _doorTimer = new();

        private readonly System.Windows.Forms.Timer _dbDebounceTimer = new();

        private readonly BackgroundWorker _logWorker = new();

        private enum DoorPhase { Closed, Opening, Open, Closing }
        private DoorPhase _doorPhase = DoorPhase.Closed;

        private int _currentFloor = 1;
        private int _targetFloor  = 1;
        private int _displayFloor = 1;
        private int _dir = 0;

        private bool _isMoving;

        private float _moveProgress = 0f;

        private int _startTop, _destTop;

        private float _doorOpenRatio = 0f;

        private int _doorHoldMs;

        private bool _themeApplied;

        private const int Floor2Y = 38;
        private const int Floor1Y = 328;

        private readonly Color _bgForm      = Color.FromArgb(15,  23,  42);
        private readonly Color _panelBg     = Color.FromArgb(24,  32,  47);
        private readonly Color _accentBlue  = Color.FromArgb(56,  189, 248);
        private readonly Color _accentActive= Color.FromArgb(37,  99,  235);
        private readonly Color _colorGreen  = Color.FromArgb(52,  211, 153);
        private readonly Color _colorAmber  = Color.FromArgb(251, 191, 36);
        private readonly Color _textWhite   = Color.FromArgb(248, 250, 252);
        private readonly Color _textMuted   = Color.FromArgb(148, 163, 184);
        private readonly Color _border      = Color.FromArgb(51,  65,  85);

        private const int   DoorAnimMs = 600;
        private const int   DoorHoldMs = 1500;
        private const float MoveSpeed  = 0.024f;

        public Elevatorsystem()
        {

            InitializeComponent();

            _logWorker.DoWork             += LogWorker_DoWork;
            _logWorker.RunWorkerCompleted += LogWorker_RunWorkerCompleted;

            ConfigureControls();
            ConfigureTimers();
            ConfigureGrid();
            InitializeElevator();
        }

        private void ConfigureControls()
        {

            lblelevator1.Visible = false;
            label2.Visible       = false;
            label3.Visible       = false;

            lblfloor1.Click += (s, e) => RequestFloor(1);
            lblfloor2.Click += (s, e) => RequestFloor(2);

            button3.Click += (s, e) => RequestFloor(1);
            button4.Click += (s, e) => RequestFloor(2);

            btnDoorClose.Click += (s, e) => CloseDoors();

            btnDoorOpen.Click += (s, e) => OpenDoors();

            btnRefreshDb.Click      += (s, e) => LoadLogs();
            btnClearDb.Click        += (s, e) => ClearLogs();
            txtSearch.TextChanged   += (s, e) => LoadLogs();
            button5.Click           += (s, e) => ShowLogSummary();

            pnlshaft.Paint    += PnlShaft_Paint;
            pnlElevator.Paint += PnlElevator_Paint;

            SetDoubleBuffered(pnlshaft);
            SetDoubleBuffered(pnlElevator);
            SetDoubleBuffered(pnlcontrols);
            SetDoubleBuffered(pnldatabase);
            SetDoubleBuffered(dgvLogs);
        }

        private void ConfigureTimers()
        {

            _moveTimer.Interval = 25;
            _moveTimer.Tick    += MoveTimer_Tick;

            _doorTimer.Interval = 20;
            _doorTimer.Tick    += DoorTimer_Tick;

            _dbDebounceTimer.Interval = 200;
            _dbDebounceTimer.Tick    += (s, e) =>
            {
                _dbDebounceTimer.Stop();
                LoadLogs();
            };
        }

        private void ConfigureGrid()
        {
            dgvLogs.AutoGenerateColumns = false;
            dgvLogs.Columns.Clear();

            dgvLogs.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Timestamp",
                HeaderText       = "TIME",
                Width            = 65,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvLogs.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "EventType",
                HeaderText       = "EVENT",
                Width            = 70,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter,
                                     Font = new Font("Segoe UI", 8.5f, FontStyle.Bold) }
            });

            dgvLogs.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Floor",
                HeaderText       = "FL",
                Width            = 30,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvLogs.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Details",
                HeaderText       = "DESCRIPTION",
                AutoSizeMode     = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvLogs.CellFormatting += DgvLogs_CellFormatting;
        }

        private void DgvLogs_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {

            if (e.RowIndex < 0 || e.RowIndex >= dgvLogs.Rows.Count || e.CellStyle == null)
                return;

            if (e.ColumnIndex == 1)
            {
                string? type = dgvLogs.Rows[e.RowIndex].Cells[1].Value?.ToString()?.ToUpperInvariant();

                e.CellStyle.ForeColor = type switch
                {
                    "ARRIVAL" => _colorGreen,
                    "MOVE"    => _colorAmber,
                    "DOOR"    => _accentBlue,
                    _         => _textWhite
                };
            }
        }

        private void InitializeElevator()
        {
            _currentFloor = 1;
            _targetFloor  = 1;
            _displayFloor = 1;
            _isMoving     = false;

            pnlElevator.Size = new Size(200, 185);
            pnlElevator.Top  = Floor1Y;
            pnlElevator.Left = (pnlshaft.Width - pnlElevator.Width) / 2;

            UpdateStatusUI();
            AppendLog("SYSTEM", 1, "ONLINE", "Elevator started at Floor 1.");
            LoadLogs();
        }

        private bool IsPending(int floor) =>
            _requestQueue.Contains(floor) || (_isMoving && _targetFloor == floor);

        private void RequestFloor(int floor)
        {

            if (floor < 1 || floor > 2) return;

            if (_isMoving && floor == _targetFloor) return;

            if (_isMoving || _doorPhase != DoorPhase.Closed)
            {

                if (!_isMoving && floor == _currentFloor)
                {
                    if (_doorPhase == DoorPhase.Open)
                        _doorHoldMs = 0;
                    else if (_doorPhase == DoorPhase.Closing)
                        _doorPhase = DoorPhase.Opening;
                    return;
                }

                if (!_requestQueue.Contains(floor))
                {
                    _requestQueue.Enqueue(floor);
                    AppendLog("REQUEST", floor, "QUEUED", $"Floor {floor} request queued.");
                    UpdateStatusUI();
                }
                return;
            }

            if (_currentFloor == floor)
            {
                AppendLog("DOOR", floor, "OPENING", $"Already at Floor {floor}. Opening doors.");
                StartDoors();
                return;
            }

            _targetFloor   = floor;
            _isMoving      = true;
            _moveProgress  = 0f;
            _startTop      = pnlElevator.Top;
            _destTop       = floor == 2 ? Floor2Y : Floor1Y;
            _dir           = floor > _currentFloor ? 1 : -1;

            AppendLog("MOVE", floor, _dir > 0 ? "ASCENDING" : "DESCENDING",
                $"Moving from Floor {_currentFloor} to Floor {floor}.");
            UpdateStatusUI();
            _moveTimer.Start();
        }

        private void MoveTimer_Tick(object? sender, EventArgs e)
        {
            _moveProgress += MoveSpeed;

            if (_moveProgress >= 1f)
            {
                pnlElevator.Top  = _destTop;
                _moveTimer.Stop();
                _isMoving        = false;
                _dir             = 0;
                _currentFloor    = _targetFloor;
                _displayFloor    = _currentFloor;

                try { SystemSounds.Asterisk.Play(); } catch { }

                AppendLog("ARRIVAL", _currentFloor, "ARRIVED", $"Arrived at Floor {_currentFloor}.");
                UpdateStatusUI();
                StartDoors();
                return;
            }

            pnlElevator.Top = _startTop + (int)((_destTop - _startTop) * Ease(_moveProgress));

            int shown = _moveProgress >= 0.5f ? _targetFloor : _currentFloor;
            if (shown != _displayFloor)
            {
                _displayFloor = shown;
                UpdateStatusUI();
            }

            pnlshaft.Invalidate();
            pnlElevator.Invalidate();
        }

        private void StartDoors()
        {
            _doorPhase = DoorPhase.Opening;
            UpdateStatusUI();
            _doorTimer.Start();
        }

        private void CloseDoors()
        {

            if (_isMoving) return;

            if (_doorPhase == DoorPhase.Open)
            {

                _doorHoldMs = DoorHoldMs;
                AppendLog("DOOR", _currentFloor, "CLOSING",
                    $"Door close requested at Floor {_currentFloor}.");
                UpdateStatusUI();
            }
            else if (_doorPhase == DoorPhase.Opening)
            {

                _doorPhase = DoorPhase.Closing;
                AppendLog("DOOR", _currentFloor, "CLOSING",
                    $"Door close requested at Floor {_currentFloor}.");
                UpdateStatusUI();
            }

        }

        private void OpenDoors()
        {

            if (_isMoving) return;

            if (_doorPhase == DoorPhase.Closed)
            {

                AppendLog("DOOR", _currentFloor, "OPENING",
                    $"Door open requested at Floor {_currentFloor}.");
                StartDoors();
            }
            else if (_doorPhase == DoorPhase.Closing)
            {

                _doorPhase = DoorPhase.Opening;
                AppendLog("DOOR", _currentFloor, "OPENING",
                    $"Door open requested at Floor {_currentFloor}.");
                UpdateStatusUI();
                _doorTimer.Start();
            }
            else if (_doorPhase == DoorPhase.Opening || _doorPhase == DoorPhase.Open)
            {

                _doorHoldMs = 0;
                AppendLog("DOOR", _currentFloor, "OPEN",
                    $"Door open extended at Floor {_currentFloor}.");
                UpdateStatusUI();
                _doorTimer.Start();
            }
        }

        private void DoorTimer_Tick(object? sender, EventArgs e)
        {

            float step = _doorTimer.Interval / (float)DoorAnimMs;

            switch (_doorPhase)
            {

                case DoorPhase.Opening:
                    _doorOpenRatio += step;
                    if (_doorOpenRatio >= 1f)
                    {
                        _doorOpenRatio = 1f;
                        _doorPhase     = DoorPhase.Open;
                        _doorHoldMs    = 0;
                        AppendLog("DOOR", _currentFloor, "OPEN",
                            $"Doors open at Floor {_currentFloor}.");
                        UpdateStatusUI();
                    }
                    break;

                case DoorPhase.Open:
                    _doorHoldMs += _doorTimer.Interval;
                    if (_doorHoldMs >= DoorHoldMs)
                    {
                        _doorPhase = DoorPhase.Closing;
                        UpdateStatusUI();
                    }
                    break;

                case DoorPhase.Closing:
                    _doorOpenRatio -= step;
                    if (_doorOpenRatio <= 0f)
                    {
                        _doorOpenRatio = 0f;
                        _doorPhase     = DoorPhase.Closed;
                        _doorTimer.Stop();
                        AppendLog("DOOR", _currentFloor, "CLOSED",
                            $"Doors closed at Floor {_currentFloor}.");
                        UpdateStatusUI();

                        if (_requestQueue.Count > 0)
                            RequestFloor(_requestQueue.Dequeue());
                    }
                    break;
            }

            pnlElevator.Invalidate();
        }

        private static float Ease(float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            return t * t * (3f - 2f * t);
        }

        private void UpdateStatusUI()
        {

            string arrow  = _isMoving ? (_dir > 0 ? " ▲" : " ▼") : "";
            label4.Text   = $"Floor {_displayFloor}{arrow}";

            if (_isMoving)
            {
                lblStatusCard.Text      = _dir > 0 ? "Moving Up ▲" : "Moving Down ▼";
                lblStatusCard.ForeColor = _colorAmber;
            }
            else if (_doorPhase != DoorPhase.Closed)
            {
                lblStatusCard.Text      = $"Doors {_doorPhase}";
                lblStatusCard.ForeColor = _accentBlue;
            }
            else
            {
                lblStatusCard.Text      = "Idle / Ready";
                lblStatusCard.ForeColor = _colorGreen;
            }

            label5.Text = $"Floor 1: {FloorStatus(1)}";
            label8.Text = $"Floor 2: {FloorStatus(2)}";

            UpdateButtonState(lblfloor1, IsPending(1));
            UpdateButtonState(lblfloor2, IsPending(2));
            UpdateButtonState(button3,   IsPending(1));
            UpdateButtonState(button4,   IsPending(2));

            pnlshaft.Invalidate();
            pnlElevator.Invalidate();
        }

        private string FloorStatus(int floor)
        {
            if (_isMoving)
                return floor == _targetFloor ? "Arriving" : "Idle";
            if (floor == _currentFloor)
                return _doorPhase == DoorPhase.Closed ? "Car here" : "Doors open";
            return IsPending(floor) ? "Called" : "Ready";
        }

        private void UpdateButtonState(Button btn, bool active)
        {
            btn.BackColor = active ? _accentActive : Color.FromArgb(30, 41, 59);
            btn.ForeColor = active ? Color.White    : _textWhite;
        }

        private void AppendLog(string eventType, int floor, string status, string details)
        {
            try
            {
                if (!_logWorker.IsBusy)
                {

                    _logWorker.RunWorkerAsync(new LogItem
                    {
                        EventType = eventType,
                        Floor     = floor,
                        Status    = status,
                        Details   = details
                    });
                }
                else
                {

                    _logService.LogEvent(eventType, floor, status, details);
                    TriggerDbReload();
                }
            }
            catch
            {

                _logService.LogEvent(eventType, floor, status, details);
                TriggerDbReload();
            }
        }

        private void LogWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is LogItem item)
                _logService.LogEvent(item.EventType, item.Floor, item.Status, item.Details);
        }

        private void LogWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            TriggerDbReload();
        }

        private void TriggerDbReload()
        {
            if (!IsHandleCreated) return;
            if (InvokeRequired)
            {

                BeginInvoke(new Action(TriggerDbReload));
                return;
            }
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                string search = txtSearch?.Text?.Trim() ?? "";
                DataTable dt  = _logService.GetLogsDataTable(search, null);

                if (dt.Columns.Contains("Timestamp"))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string? ts = row["Timestamp"]?.ToString();
                        if (!string.IsNullOrEmpty(ts) && DateTime.TryParse(ts, out DateTime parsed))
                            row["Timestamp"] = parsed.ToString("HH:mm:ss");
                    }
                }

                dgvLogs.DataSource = dt;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadLogs Error] {ex.Message}");
            }
        }

        private void ClearLogs()
        {
            if (MessageBox.Show("Clear all SQLite elevator activity logs?", "Clear Logs",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _logService.ClearLogs();
                AppendLog("SYSTEM", _currentFloor, "CLEARED", "Database cleared.");
                LoadLogs();
            }
        }

        private void ShowLogSummary()
        {
            var (total, trips) = _logService.GetStatistics();
            MessageBox.Show(
                $"Elevator SQLite Statistics:\n\n" +
                $"• Total Events Logged: {total}\n" +
                $"• Completed Trips:     {trips}\n" +
                $"• Current Floor:       {_currentFloor}",
                "Elevator Telemetry",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private sealed class LogItem
        {
            public string EventType { get; set; } = "INFO";
            public int    Floor     { get; set; } = 1;
            public string Status    { get; set; } = "Info";
            public string Details   { get; set; } = string.Empty;
        }

        private void PnlShaft_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode        = SmoothingMode.AntiAlias;
            g.TextRenderingHint    = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int w = pnlshaft.Width;
            int h = pnlshaft.Height;

            Rectangle shaftRect = new Rectangle(0, 0, w - 1, h - 1);
            using (LinearGradientBrush bg = new LinearGradientBrush(shaftRect,
                Color.FromArgb(11, 15, 25), Color.FromArgb(17, 24, 39), 90f))
            {
                g.FillRectangle(bg, shaftRect);
            }

            int railLeft  = pnlElevator.Left  - 8;
            int railRight = pnlElevator.Right + 8;

            using (Pen trackShadow = new Pen(Color.FromArgb(20, 28,  42), 5f))
            {
                g.DrawLine(trackShadow, railLeft,  20, railLeft,  h - 20);
                g.DrawLine(trackShadow, railRight, 20, railRight, h - 20);
            }
            using (Pen trackBack = new Pen(Color.FromArgb(40, 52, 70), 3f))
            {
                g.DrawLine(trackBack, railLeft,  20, railLeft,  h - 20);
                g.DrawLine(trackBack, railRight, 20, railRight, h - 20);
            }
            using (Pen trackShine = new Pen(Color.FromArgb(90, 115, 145), 1.2f))
            {
                g.DrawLine(trackShine, railLeft  - 1, 20, railLeft  - 1, h - 20);
                g.DrawLine(trackShine, railRight + 1, 20, railRight + 1, h - 20);
            }

            int carCenterX = pnlElevator.Left + pnlElevator.Width / 2;

            using (Pen cableGlow = new Pen(Color.FromArgb(50, 56, 189, 248), 3f))
                g.DrawLine(cableGlow, carCenterX, 0, carCenterX, pnlElevator.Top);

            using (Pen cablePen = new Pen(Color.FromArgb(160, 175, 195), 1.5f))
            {
                g.DrawLine(cablePen, carCenterX - 1, 0, carCenterX - 1, pnlElevator.Top);
                g.DrawLine(cablePen, carCenterX + 1, 0, carCenterX + 1, pnlElevator.Top);
            }

            DrawFloorLanding(g, 2, Floor2Y, w);
            DrawFloorLanding(g, 1, Floor1Y, w);

            using (Pen frame = new Pen(_border, 1.2f))
                g.DrawRectangle(frame, shaftRect);
        }

        private void DrawFloorLanding(Graphics g, int floor, int floorTopY, int width)
        {

            int  landingY   = floorTopY + pnlElevator.Height;
            bool isCarHere  = !_isMoving && _currentFloor == floor;

            Color glowColor = isCarHere
                ? Color.FromArgb(140, 56, 189, 248)
                : Color.FromArgb(40,  71, 85,  105);

            using (Pen lineGlow = new Pen(glowColor, 3.5f))
                g.DrawLine(lineGlow, 14, landingY, width - 14, landingY);

            using (Pen line = new Pen(isCarHere ? _accentBlue : Color.FromArgb(71, 85, 105), 1.2f))
                g.DrawLine(line, 14, landingY, width - 14, landingY);

            Rectangle badgeRect = new Rectangle(14, floorTopY - 26, 92, 22);
            using (GraphicsPath badgePath = RoundedRect(badgeRect, 6))
            {

                Color badgeBg = isCarHere
                    ? Color.FromArgb(40, 56, 189, 248)
                    : Color.FromArgb(18, 255, 255, 255);

                using (SolidBrush b = new SolidBrush(badgeBg))
                    g.FillPath(b, badgePath);

                Color badgeBorder = isCarHere ? _accentBlue : Color.FromArgb(60, 148, 163, 184);
                using (Pen p = new Pen(badgeBorder, 1f))
                    g.DrawPath(p, badgePath);

                Color dotColor = isCarHere ? _colorGreen
                               : IsPending(floor) ? _colorAmber
                               : Color.FromArgb(100, 116, 139);

                using (SolidBrush dot = new SolidBrush(dotColor))
                    g.FillEllipse(dot, badgeRect.X + 8, badgeRect.Y + 6, 8, 8);

                Rectangle textRect = new Rectangle(badgeRect.X + 22, badgeRect.Y,
                    badgeRect.Width - 24, badgeRect.Height);
                TextRenderer.DrawText(g, $"Floor {floor}",
                    new Font("Segoe UI", 8.5F, FontStyle.Bold), textRect,
                    isCarHere ? Color.White : _textMuted,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
        }

        private void PnlElevator_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int w = pnlElevator.Width;
            int h = pnlElevator.Height;

            Rectangle chassis = new Rectangle(0, 0, w - 1, h - 1);
            using (GraphicsPath chassisPath = RoundedRect(chassis, 12))
            {
                using (Pen glowPen = new Pen(Color.FromArgb(50, 56, 189, 248), 2.5f))
                    g.DrawPath(glowPen, chassisPath);

                using (LinearGradientBrush chassisBrush = new LinearGradientBrush(chassis,
                    Color.FromArgb(32, 44, 64), Color.FromArgb(16, 23, 36), 90f))
                {
                    g.FillPath(chassisBrush, chassisPath);
                }

                using (Pen borderPen = new Pen(_accentBlue, 1.4f))
                    g.DrawPath(borderPen, chassisPath);
            }

            Rectangle displayRect = new Rectangle(14, 8, w - 28, 22);
            using (GraphicsPath dispPath = RoundedRect(displayRect, 5))
            {
                using (SolidBrush dispBg = new SolidBrush(Color.FromArgb(10, 15, 24)))
                    g.FillPath(dispBg, dispPath);

                using (Pen dispBorder = new Pen(Color.FromArgb(51, 65, 85), 1f))
                    g.DrawPath(dispBorder, dispPath);

                string arrow    = _isMoving ? (_dir > 0 ? "▲ " : "▼ ") : "● ";
                string dispText = $"{arrow}Floor {_displayFloor}";
                Color  dispColor = _isMoving ? _colorAmber
                                 : (_doorPhase != DoorPhase.Closed ? _accentBlue : _colorGreen);

                TextRenderer.DrawText(g, dispText, new Font("Segoe UI", 9F, FontStyle.Bold),
                    displayRect, dispColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            Rectangle portal = new Rectangle(12, 35, w - 24, h - 48);
            using (GraphicsPath portalPath = RoundedRect(portal, 6))
            {
                using (LinearGradientBrush interiorLight = new LinearGradientBrush(portal,
                    Color.FromArgb(241, 245, 249), Color.FromArgb(203, 213, 225), 90f))
                {
                    g.FillPath(interiorLight, portalPath);
                }

                using (SolidBrush spot = new SolidBrush(Color.FromArgb(180, 255, 255, 225)))
                    g.FillEllipse(spot, portal.X + portal.Width / 2 - 25, portal.Y + 3, 50, 14);

                using (Pen wallSlat = new Pen(Color.FromArgb(180, 195, 215), 1f))
                {
                    g.DrawLine(wallSlat,
                        portal.X + portal.Width / 4, portal.Y + 16,
                        portal.X + portal.Width / 4, portal.Bottom - 6);
                    g.DrawLine(wallSlat,
                        portal.Right - portal.Width / 4, portal.Y + 16,
                        portal.Right - portal.Width / 4, portal.Bottom - 6);
                }

                float open   = Ease(_doorOpenRatio);
                int   half   = portal.Width / 2;
                int   offset = (int)(open * (half - 2));

                Region prevClip = g.Clip;
                g.SetClip(portalPath);

                Rectangle leftDoor = new Rectangle(portal.X - offset, portal.Y, half, portal.Height);
                using (LinearGradientBrush doorBgL = new LinearGradientBrush(portal,
                    Color.FromArgb(51, 65, 85), Color.FromArgb(30, 41, 59), 0f))
                {
                    g.FillRectangle(doorBgL, leftDoor);
                }

                Rectangle rightDoor = new Rectangle(portal.X + half + offset, portal.Y, half, portal.Height);
                using (LinearGradientBrush doorBgR = new LinearGradientBrush(portal,
                    Color.FromArgb(30, 41, 59), Color.FromArgb(51, 65, 85), 0f))
                {
                    g.FillRectangle(doorBgR, rightDoor);
                }

                using (SolidBrush handleBrush = new SolidBrush(Color.FromArgb(220, 230, 242)))
                {
                    g.FillRectangle(handleBrush,
                        leftDoor.Right - 3,  leftDoor.Y  + leftDoor.Height  / 2 - 20, 2, 40);
                    g.FillRectangle(handleBrush,
                        rightDoor.Left + 1,  rightDoor.Y + rightDoor.Height / 2 - 20, 2, 40);
                }

                using (Pen shine = new Pen(Color.FromArgb(35, 255, 255, 255), 1.5f))
                {
                    if (leftDoor.Width  > 12)
                        g.DrawLine(shine,
                            leftDoor.X  + 8, leftDoor.Bottom  - 12,
                            leftDoor.Right  - 8, leftDoor.Y   + 12);
                    if (rightDoor.Width > 12)
                        g.DrawLine(shine,
                            rightDoor.X + 8, rightDoor.Bottom - 12,
                            rightDoor.Right - 8, rightDoor.Y  + 12);
                }

                g.Clip = prevClip;

                using (Pen frame = new Pen(Color.FromArgb(71, 85, 105), 1.2f))
                    g.DrawPath(frame, portalPath);
            }

            using (SolidBrush ledStrip = new SolidBrush(Color.FromArgb(140, 56, 189, 248)))
                g.FillRectangle(ledStrip, 16, h - 8, w - 32, 2);
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {

            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));

            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X,           rect.Y,            d, d, 180, 90);
            path.AddArc(rect.Right - d,   rect.Y,            d, d, 270, 90);
            path.AddArc(rect.Right - d,   rect.Bottom - d,   d, d,   0, 90);
            path.AddArc(rect.X,           rect.Bottom - d,   d, d,  90, 90);
            path.CloseFigure();
            return path;
        }

        private static void SetDoubleBuffered(Control c)
        {
            typeof(Control)
                .GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic)?
                .SetValue(c, true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (_themeApplied) return;
            _themeApplied = true;

            RoundCorners(pnlshaft,     10);
            RoundCorners(pnlcontrols,  10);
            RoundCorners(pnldatabase,  10);
            RoundCorners(label4,        8);
            RoundCorners(lblStatusCard, 6);
            RoundCorners(lblfloor1,     8);
            RoundCorners(lblfloor2,     8);
            RoundCorners(button3,       8);
            RoundCorners(button4,       8);
            RoundCorners(button5,       6);
            RoundCorners(btnRefreshDb,  6);
            RoundCorners(btnClearDb,    6);
            RoundCorners(txtSearch,     6);
            RoundCorners(btnDoorOpen,   8);
            RoundCorners(btnDoorClose,  8);

            LoadLogs();
        }

        private void RoundCorners(Control c, int radius)
        {
            void Apply()
            {
                if (c.Width < 2 || c.Height < 2) return;
                using GraphicsPath p = RoundedRect(new Rectangle(0, 0, c.Width, c.Height), radius);
                c.Region = new Region(p);
            }
            Apply();
            c.SizeChanged += (s, e) => Apply();
        }

        private void label2_Click(object sender, EventArgs e)   { }
        private void label2_Click_1(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e)   { }
        private void label5_Click_1(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
    }
}
