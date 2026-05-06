namespace VirtualEventScheduler.Desktop.Forms
{
    partial class EventManagementForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtTitle = new TextBox();
            this.txtDescription = new TextBox();
            this.txtLocation = new TextBox();
            this.txtCapacity = new TextBox();
            this.dtpDateTime = new DateTimePicker();
            this.btnCreate = new Button();
            this.btnRefresh = new Button();
            this.btnFilter = new Button();
            this.dgvEvents = new DataGridView();
            this.cmbStatus = new ComboBox();

            this.SuspendLayout();

            var lblTitle = new Label { Text = "Title:", Location = new Point(10, 15), Size = new Size(80, 20) };
            var lblDesc = new Label { Text = "Description:", Location = new Point(10, 45), Size = new Size(80, 20) };
            var lblDate = new Label { Text = "Date:", Location = new Point(10, 75), Size = new Size(80, 20) };
            var lblLoc = new Label { Text = "Location:", Location = new Point(10, 105), Size = new Size(80, 20) };
            var lblCap = new Label { Text = "Capacity:", Location = new Point(10, 135), Size = new Size(80, 20) };
            var lblStatus = new Label { Text = "Status:", Location = new Point(10, 320), Size = new Size(80, 20) };

            this.txtTitle.Location = new Point(100, 12); this.txtTitle.Size = new Size(250, 23);
            this.txtDescription.Location = new Point(100, 42); this.txtDescription.Size = new Size(250, 23);
            this.dtpDateTime.Location = new Point(100, 72); this.dtpDateTime.Size = new Size(250, 23);
            this.txtLocation.Location = new Point(100, 102); this.txtLocation.Size = new Size(250, 23);
            this.txtCapacity.Location = new Point(100, 132); this.txtCapacity.Size = new Size(250, 23);

            this.btnCreate.Text = "Create Event"; this.btnCreate.Location = new Point(100, 165); this.btnCreate.Size = new Size(120, 30);
            this.btnCreate.Click += new EventHandler(this.btnCreate_Click);

            this.btnRefresh.Text = "Refresh List"; this.btnRefresh.Location = new Point(230, 165); this.btnRefresh.Size = new Size(120, 30);
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            this.dgvEvents.Location = new Point(10, 205); this.dgvEvents.Size = new Size(760, 100);
            this.dgvEvents.AllowUserToAddRows = false; this.dgvEvents.ReadOnly = true;

            this.cmbStatus.Location = new Point(100, 317); this.cmbStatus.Size = new Size(120, 23);
            this.btnFilter.Text = "Filter"; this.btnFilter.Location = new Point(230, 315); this.btnFilter.Size = new Size(80, 25);
            this.btnFilter.Click += new EventHandler(this.btnFilter_Click);

            this.ClientSize = new Size(790, 360);
            this.Controls.AddRange(new Control[] { lblTitle, lblDesc, lblDate, lblLoc, lblCap, lblStatus, txtTitle, txtDescription, dtpDateTime, txtLocation, txtCapacity, btnCreate, btnRefresh, dgvEvents, cmbStatus, btnFilter });
            this.Text = "Event Management";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private TextBox txtTitle, txtDescription, txtLocation, txtCapacity;
        private DateTimePicker dtpDateTime;
        private Button btnCreate, btnRefresh, btnFilter;
        private DataGridView dgvEvents;
        private ComboBox cmbStatus;
    }
}
