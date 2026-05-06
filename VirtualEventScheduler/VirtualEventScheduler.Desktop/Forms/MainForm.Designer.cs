namespace VirtualEventScheduler.Desktop.Forms
{
    partial class MainForm
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
            this.lblWelcome = new Label();
            this.btnEventManagement = new Button();
            this.btnViewEvents = new Button();
            this.btnLogout = new Button();

            this.SuspendLayout();

            this.lblWelcome.Location = new Point(30, 20);
            this.lblWelcome.Size = new Size(320, 30);
            this.lblWelcome.Font = new Font("Arial", 11, FontStyle.Bold);

            this.btnEventManagement.Text = "Event Management";
            this.btnEventManagement.Location = new Point(100, 70);
            this.btnEventManagement.Size = new Size(180, 35);
            this.btnEventManagement.Click += new EventHandler(this.btnEventManagement_Click);

            this.btnViewEvents.Text = "View All Events";
            this.btnViewEvents.Location = new Point(100, 115);
            this.btnViewEvents.Size = new Size(180, 35);
            this.btnViewEvents.Click += new EventHandler(this.btnViewEvents_Click);

            this.btnLogout.Text = "Logout";
            this.btnLogout.Location = new Point(100, 160);
            this.btnLogout.Size = new Size(180, 35);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

            this.ClientSize = new Size(380, 220);
            this.Controls.AddRange(new Control[] { lblWelcome, btnEventManagement, btnViewEvents, btnLogout });
            this.Text = "Virtual Event Scheduler - Main Menu";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private Label lblWelcome;
        private Button btnEventManagement;
        private Button btnViewEvents;
        private Button btnLogout;
    }
}
