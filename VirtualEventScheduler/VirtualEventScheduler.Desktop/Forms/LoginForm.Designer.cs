namespace VirtualEventScheduler.Desktop.Forms
{
    partial class LoginForm
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
            this.txtEmail = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            var lblEmail = new Label();
            var lblPassword = new Label();
            var lblTitle = new Label();

            this.SuspendLayout();

            lblTitle.Text = "Login to System";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(80, 20);
            lblTitle.Size = new Size(200, 30);

            lblEmail.Text = "Email:";
            lblEmail.Location = new Point(30, 70);
            lblEmail.Size = new Size(60, 20);

            this.txtEmail.Location = new Point(100, 67);
            this.txtEmail.Size = new Size(220, 23);

            lblPassword.Text = "Password:";
            lblPassword.Location = new Point(30, 110);
            lblPassword.Size = new Size(70, 20);

            this.txtPassword.Location = new Point(100, 107);
            this.txtPassword.Size = new Size(220, 23);
            this.txtPassword.PasswordChar = '*';

            this.btnLogin.Text = "Login";
            this.btnLogin.Location = new Point(130, 150);
            this.btnLogin.Size = new Size(100, 30);
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            this.ClientSize = new Size(370, 210);
            this.Controls.AddRange(new Control[] { lblTitle, lblEmail, txtEmail, lblPassword, txtPassword, btnLogin });
            this.Text = "Virtual Event Scheduler - Login";
            this.StartPosition = FormStartPosition.CenterScreen;

            this.ResumeLayout(false);
        }

        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnLogin;
    }
}
