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
            this.btnViewParticipants = new Button();
            this.btnCancelEvent = new Button();
            this.dgvEvents = new DataGridView();
            this.cmbStatus = new ComboBox();

            this.SuspendLayout();

            // Labels for the create form
            var lblTitle = new Label { Text = "Title:", Location = new Point(10, 15), Size = new Size(80, 20) };
            var lblDesc = new Label { Text = "Description:", Location = new Point(10, 45), Size = new Size(80, 20) };
            var lblDate = new Label { Text = "Date:", Location = new Point(10, 75), Size = new Size(80, 20) };
            var lblLoc = new Label { Text = "Location:", Location = new Point(10, 105), Size = new Size(80, 20) };
            var lblCap = new Label { Text = "Capacity:", Location = new Point(10, 135), Size = new Size(80, 20) };
            var lblStatus = new Label { Text = "Status:", Location = new Point(10, 320), Size = new Size(80, 20) };

            // Input fields for creating a new event
            this.txtTitle.Location = new Point(100, 12); this.txtTitle.Size = new Size(250, 23);
            this.txtDescription.Location = new Point(100, 42); this.txtDescription.Size = new Size(250, 23);
            this.dtpDateTime.Location = new Point(100, 72); this.dtpDateTime.Size = new Size(250, 23);
            this.txtLocation.Location = new Point(100, 102); this.txtLocation.Size = new Size(250, 23);
            this.txtCapacity.Location = new Point(100, 132); this.txtCapacity.Size = new Size(250, 23);

            // Create and Refresh buttons
            this.btnCreate.Text = "Create Event";
            this.btnCreate.Location = new Point(100, 165);
            this.btnCreate.Size = new Size(120, 30);
            this.btnCreate.Click += new EventHandler(this.btnCreate_Click);

            this.btnRefresh.Text = "Refresh List";
            this.btnRefresh.Location = new Point(230, 165);
            this.btnRefresh.Size = new Size(120, 30);
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            // Events DataGridView
            this.dgvEvents.Location = new Point(10, 205);
            this.dgvEvents.Size = new Size(760, 100);
            this.dgvEvents.AllowUserToAddRows = false;
            this.dgvEvents.ReadOnly = true;
            this.dgvEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvEvents.MultiSelect = false;

            // Status filter combo and button
            this.cmbStatus.Location = new Point(100, 317);
            this.cmbStatus.Size = new Size(120, 23);
            this.btnFilter.Text = "Filter";
            this.btnFilter.Location = new Point(230, 315);
            this.btnFilter.Size = new Size(80, 25);
            this.btnFilter.Click += new EventHandler(this.btnFilter_Click);

            // View Participants button — opens ParticipantsForm for the selected row
            this.btnViewParticipants.Text = "View Participants";
            this.btnViewParticipants.Location = new Point(320, 315);
            this.btnViewParticipants.Size = new Size(130, 25);
            this.btnViewParticipants.BackColor = Color.SteelBlue;
            this.btnViewParticipants.ForeColor = Color.White;
            this.btnViewParticipants.Click += new EventHandler(this.btnViewParticipants_Click);

            // Cancel Event button — cancels the selected event via the API
            this.btnCancelEvent.Text = "Cancel Event";
            this.btnCancelEvent.Location = new Point(460, 315);
            this.btnCancelEvent.Size = new Size(110, 25);
            this.btnCancelEvent.BackColor = Color.Firebrick;
            this.btnCancelEvent.ForeColor = Color.White;
            this.btnCancelEvent.Click += new EventHandler(this.btnCancelEvent_Click);

            this.ClientSize = new Size(800, 360);
            this.Controls.AddRange(new Control[] {
                lblTitle, lblDesc, lblDate, lblLoc, lblCap, lblStatus,
                txtTitle, txtDescription, dtpDateTime, txtLocation, txtCapacity,
                btnCreate, btnRefresh,
                dgvEvents,
                cmbStatus, btnFilter, btnViewParticipants, btnCancelEvent
            });
            this.Text = "Event Management";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private TextBox txtTitle, txtDescription, txtLocation, txtCapacity;
        private DateTimePicker dtpDateTime;
        private Button btnCreate, btnRefresh, btnFilter, btnViewParticipants, btnCancelEvent;
        private DataGridView dgvEvents;
        private ComboBox cmbStatus;
    }
}
