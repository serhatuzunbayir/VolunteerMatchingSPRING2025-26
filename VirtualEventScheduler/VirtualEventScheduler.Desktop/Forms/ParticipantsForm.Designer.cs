namespace VirtualEventScheduler.Desktop.Forms
{
    partial class ParticipantsForm
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
            this.lblEventTitle = new Label();
            this.lblCount = new Label();
            this.dgvParticipants = new DataGridView();
            this.btnRefresh = new Button();
            this.btnClose = new Button();

            this.SuspendLayout();

            // Event title label at the top
            this.lblEventTitle.Location = new Point(10, 10);
            this.lblEventTitle.Size = new Size(560, 24);
            this.lblEventTitle.Font = new Font("Arial", 10, FontStyle.Bold);

            // Participant count label
            this.lblCount.Location = new Point(10, 38);
            this.lblCount.Size = new Size(300, 20);
            this.lblCount.Text = "Loading...";

            // DataGridView for participants
            this.dgvParticipants.Location = new Point(10, 65);
            this.dgvParticipants.Size = new Size(760, 300);
            this.dgvParticipants.AllowUserToAddRows = false;
            this.dgvParticipants.ReadOnly = true;
            this.dgvParticipants.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvParticipants.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Refresh button
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Location = new Point(10, 380);
            this.btnRefresh.Size = new Size(100, 30);
            this.btnRefresh.Click += new EventHandler(this.btnRefresh_Click);

            // Close button
            this.btnClose.Text = "Close";
            this.btnClose.Location = new Point(120, 380);
            this.btnClose.Size = new Size(100, 30);
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            this.ClientSize = new Size(790, 425);
            this.Controls.AddRange(new Control[] { lblEventTitle, lblCount, dgvParticipants, btnRefresh, btnClose });
            this.Text = "Participants";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private Label lblEventTitle;
        private Label lblCount;
        private DataGridView dgvParticipants;
        private Button btnRefresh;
        private Button btnClose;
    }
}
