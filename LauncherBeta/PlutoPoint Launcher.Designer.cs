using PlutoPoint_Launcher.Properties;
using System.Runtime.Remoting.Activation;
using System.Security.Policy;
using System.Windows.Forms;

// Copyright © Charlie Howard 2025 All rights reserved.

namespace PlutoPoint_Launcher
{
    partial class installerForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.loadingIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadingIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // loadingIcon
            // 
            this.loadingIcon.Image = global::PlutoPoint_Launcher.Properties.Resources.loading;
            this.loadingIcon.Location = new System.Drawing.Point(-201, -77);
            this.loadingIcon.Name = "loadingIcon";
            this.loadingIcon.Size = new System.Drawing.Size(788, 440);
            this.loadingIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.loadingIcon.TabIndex = 4;
            this.loadingIcon.Image = Resources.loading;
            this.loadingIcon.TabStop = false;
            // 
            // installerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.ClientSize = new System.Drawing.Size(390, 329);
            this.Controls.Add(this.loadingIcon);
            this.Icon = global::PlutoPoint_Launcher.Properties.Resources.computerRepairCentreIcon;
            this.Name = "installerForm";
            this.Text = "Computer Repair Centre Installer";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.loadingIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.PictureBox loadingIcon;
    }
}

