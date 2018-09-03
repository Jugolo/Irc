namespace Irc.Forms
{
    partial class Channel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent(Form1 main)
        {
            this.channelButton1 = new Forms.ChannelButton();
            this.message1 = new Forms.Message();
            this.UserList = new Forms.UserList(main, this);
            this.SuspendLayout();
            // 
            // channelButton1
            // 
            this.channelButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.channelButton1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.channelButton1.Location = new System.Drawing.Point(0, 421);
            this.channelButton1.Name = "channelButton1";
            this.channelButton1.Size = new System.Drawing.Size(1083, 35);
            this.channelButton1.TabIndex = 0;
            // 
            // message1
            // 
            this.message1.BackColor = System.Drawing.SystemColors.Window;
            this.message1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.message1.Location = new System.Drawing.Point(0, 0);
            this.message1.Name = "message1";
            this.message1.Size = new System.Drawing.Size(1083, 421);
            this.message1.TabIndex = 1;
            // 
            // UserList
            // 
            this.UserList.BackColor = System.Drawing.SystemColors.Window;
            this.UserList.Dock = System.Windows.Forms.DockStyle.Right;
            this.UserList.Location = new System.Drawing.Point(1083, 0);
            this.UserList.Name = "UserList";
            this.UserList.Size = new System.Drawing.Size(122, 456);
            this.UserList.TabIndex = 2;
            // 
            // Channel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.message1);
            this.Controls.Add(this.channelButton1);
            this.Controls.Add(this.UserList);
            this.Name = "Channel";
            this.Size = new System.Drawing.Size(1205, 456);
            this.ResumeLayout(false);

        }

        #endregion

        private Forms.ChannelButton channelButton1;
        private Forms.Message message1;
        private Forms.UserList UserList;
    }
}
