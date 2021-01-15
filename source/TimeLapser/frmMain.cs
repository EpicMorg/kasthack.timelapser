﻿#if DEBUG
#define TESTING
#endif
using System;
using System.IO;
using System.Windows.Forms;

using Accord.Video.FFMPEG;

namespace kasthack.TimeLapser
{
    public partial class frmMain : Form
    {
        private readonly Recorder _recorder = new Recorder();
        private RecordSettings _settings;

        public frmMain()
        {
            InitializeComponent();
            this.ApplyLocale();
        }

        private void ApplyLocale()
        {
            this.Text = Locale.Locale.ProgramName;
            this.lblFreq.Text = Locale.Locale.IntervalMs;
            this.lblPath.Text = Locale.Locale.OutputPath;
            this.chkRealtime.Text = Locale.Locale.Realtime;
            this.lblFramerate.Text = Locale.Locale.Framerate;
            this.lblSnapper.Text = Locale.Locale.Recorder;
            this.lblFormat.Text = Locale.Locale.Format;
            this.lblBitrate.Text = Locale.Locale.Bitrate;
            this.lblScreen.Text = Locale.Locale.Screen;
            this.chkSplit.Text = Locale.Locale.SplitEveryNMinutes;
            this.lblTime.Text = Locale.Locale.Pending;
            this.btnGo.Text = Locale.Locale.StartRecording;
        }

        private void StartRecordingClicked(object sender, EventArgs e)
        {
            if (_recorder.Recording)
            {
                _recorder.Stop();
                SetRecordingState(false);
            }
            else
            {
                SetRecordingState(true);
                this._settings = new RecordSettings(
                    outputPath: txtPath.Text,
                    captureRectangle: ((ScreenInfo)cmbScreen.SelectedItem).Rect,
                    fps: (int)nudFramerate.Value,
                    interval: (int)nudFreq.Value,
                    codec: (VideoCodec)cmbFormat.SelectedItem,
                    bitrate: (int)budBitrate.Value << 20,
                    splitInterval: chkSplit.Checked ? (double?)nudSplitInterval.Value : null,
                    onFrameWritten: (a) => BeginInvoke((Action)(() => lblTime.Text = string.Format(Locale.Locale.ElapsedFormatStirng, a))),
                    realtime: chkRealtime.Checked,
                    snapperType: (SnapperType)cmbSnapper.SelectedItem
                );
                _recorder.Start(
                    _settings
                );
            }
        }

        private void SetRecordingState(bool recordRunning)
        {
            btnGo.Text = recordRunning ? Locale.Locale.StopRecording : Locale.Locale.StartRecording;
            lblTime.Text = !recordRunning ? Locale.Locale.Pending : string.Empty;
            txtPath.Enabled
                = btnbrs.Enabled
                = nudFreq.Enabled
                = nudFramerate.Enabled
                = cmbFormat.Enabled
                = budBitrate.Enabled
                = cmbScreen.Enabled
                = chkSplit.Enabled
                = nudSplitInterval.Enabled
                = chkRealtime.Enabled
                = !recordRunning;
        }

        private void FormLoad(object sender, EventArgs e)
        {
            cmbSnapper.DataSource = Enum.GetValues(typeof(SnapperType)) as SnapperType[];
            cmbFormat.DataSource = Enum.GetValues(typeof(VideoCodec)) as VideoCodec[];
            cmbScreen.DataSource = Recorder.GetScreenInfos();
            cmbFormat.SelectedIndex = 0;
            cmbScreen.SelectedIndex = cmbScreen.Items.Count - 1;
            txtPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
#if TESTING
            txtPath.Text = Path.Combine(txtPath.Text, "dbg_scr");
            chkSplit.Checked = true;
            nudSplitInterval.Value = 1;
            chkRealtime.Checked = true;
            cmbScreen.SelectedIndex = 1;
#endif
        }
        private void BrowseDirectoryClicked(object sender, EventArgs e) => txtPath.Text = fbdSave.ShowDialog() == DialogResult.OK ? fbdSave.SelectedPath : txtPath.Text;
        private void SplitCheckChanged(object sender, EventArgs e) => nudSplitInterval.Enabled = chkSplit.Checked;

        private void StatusIconClicked(object sender, EventArgs e)
        {
            this.ShowInTaskbar = this.Visible = !(nicon.Visible = false);
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void HandleSizeChanged(object sender, EventArgs e) => this.ShowInTaskbar = WindowState == FormWindowState.Minimized ? this.Visible = !(nicon.Visible = true) : this.ShowInTaskbar;

        private void RealtimeCheckChanged(object sender, EventArgs e) => nudFreq.Enabled = !chkRealtime.Checked;
    }
}