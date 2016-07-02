namespace ZKWeb.Toolkits.ProjectCreator.Gui {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.lbProjectType = new System.Windows.Forms.Label();
			this.panelProjectType = new System.Windows.Forms.Panel();
			this.rbOwin = new System.Windows.Forms.RadioButton();
			this.rbAspNetCore = new System.Windows.Forms.RadioButton();
			this.rbAspNet = new System.Windows.Forms.RadioButton();
			this.panelMain = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.btnBrowseOutputDirectory = new System.Windows.Forms.Button();
			this.tbOutputDirectory = new System.Windows.Forms.TextBox();
			this.lbOutputDirectory = new System.Windows.Forms.Label();
			this.panelDefaultPlugins = new System.Windows.Forms.TableLayoutPanel();
			this.btnBrowseDefaultPlugins = new System.Windows.Forms.Button();
			this.tbUseDefaultPlugins = new System.Windows.Forms.TextBox();
			this.lbDefaultPlugins = new System.Windows.Forms.Label();
			this.panelConnectionString = new System.Windows.Forms.TableLayoutPanel();
			this.btnTest = new System.Windows.Forms.Button();
			this.tbConnectionString = new System.Windows.Forms.TextBox();
			this.lbConnectionString = new System.Windows.Forms.Label();
			this.panelDatabase = new System.Windows.Forms.Panel();
			this.rbPostgreSQL = new System.Windows.Forms.RadioButton();
			this.rbMySQL = new System.Windows.Forms.RadioButton();
			this.rbMSSQL = new System.Windows.Forms.RadioButton();
			this.rbSQLite = new System.Windows.Forms.RadioButton();
			this.lbDatabase = new System.Windows.Forms.Label();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.lbDescription = new System.Windows.Forms.Label();
			this.lbProjectName = new System.Windows.Forms.Label();
			this.tbProjectName = new System.Windows.Forms.TextBox();
			this.btnCreateProject = new System.Windows.Forms.Button();
			this.panelProjectType.SuspendLayout();
			this.panelMain.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panelDefaultPlugins.SuspendLayout();
			this.panelConnectionString.SuspendLayout();
			this.panelDatabase.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbProjectType
			// 
			this.lbProjectType.AutoSize = true;
			this.lbProjectType.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbProjectType.Location = new System.Drawing.Point(3, 0);
			this.lbProjectType.Name = "lbProjectType";
			this.lbProjectType.Size = new System.Drawing.Size(124, 32);
			this.lbProjectType.TabIndex = 0;
			this.lbProjectType.Text = "Project Type*:";
			this.lbProjectType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelProjectType
			// 
			this.panelProjectType.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelProjectType.Controls.Add(this.rbOwin);
			this.panelProjectType.Controls.Add(this.rbAspNetCore);
			this.panelProjectType.Controls.Add(this.rbAspNet);
			this.panelProjectType.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelProjectType.Location = new System.Drawing.Point(133, 3);
			this.panelProjectType.Name = "panelProjectType";
			this.panelProjectType.Size = new System.Drawing.Size(620, 26);
			this.panelProjectType.TabIndex = 1;
			// 
			// rbOwin
			// 
			this.rbOwin.AutoSize = true;
			this.rbOwin.Location = new System.Drawing.Point(172, 3);
			this.rbOwin.Name = "rbOwin";
			this.rbOwin.Size = new System.Drawing.Size(49, 17);
			this.rbOwin.TabIndex = 2;
			this.rbOwin.Tag = "Owin";
			this.rbOwin.Text = "Owin";
			this.rbOwin.UseVisualStyleBackColor = true;
			// 
			// rbAspNetCore
			// 
			this.rbAspNetCore.AutoSize = true;
			this.rbAspNetCore.Location = new System.Drawing.Point(78, 3);
			this.rbAspNetCore.Name = "rbAspNetCore";
			this.rbAspNetCore.Size = new System.Drawing.Size(88, 17);
			this.rbAspNetCore.TabIndex = 1;
			this.rbAspNetCore.Tag = "AspNetCore";
			this.rbAspNetCore.Text = "Asp.Net Core";
			this.rbAspNetCore.UseVisualStyleBackColor = true;
			// 
			// rbAspNet
			// 
			this.rbAspNet.AutoSize = true;
			this.rbAspNet.Checked = true;
			this.rbAspNet.Location = new System.Drawing.Point(3, 3);
			this.rbAspNet.Name = "rbAspNet";
			this.rbAspNet.Size = new System.Drawing.Size(63, 17);
			this.rbAspNet.TabIndex = 0;
			this.rbAspNet.TabStop = true;
			this.rbAspNet.Tag = "AspNet";
			this.rbAspNet.Text = "Asp.Net";
			this.rbAspNet.UseVisualStyleBackColor = true;
			// 
			// panelMain
			// 
			this.panelMain.ColumnCount = 2;
			this.panelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
			this.panelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.panelMain.Controls.Add(this.tableLayoutPanel1, 1, 6);
			this.panelMain.Controls.Add(this.lbOutputDirectory, 0, 6);
			this.panelMain.Controls.Add(this.panelDefaultPlugins, 1, 5);
			this.panelMain.Controls.Add(this.lbDefaultPlugins, 0, 5);
			this.panelMain.Controls.Add(this.panelConnectionString, 1, 4);
			this.panelMain.Controls.Add(this.lbConnectionString, 0, 4);
			this.panelMain.Controls.Add(this.panelDatabase, 1, 3);
			this.panelMain.Controls.Add(this.lbDatabase, 0, 3);
			this.panelMain.Controls.Add(this.tbDescription, 1, 2);
			this.panelMain.Controls.Add(this.lbDescription, 0, 2);
			this.panelMain.Controls.Add(this.lbProjectName, 0, 1);
			this.panelMain.Controls.Add(this.lbProjectType, 0, 0);
			this.panelMain.Controls.Add(this.panelProjectType, 1, 0);
			this.panelMain.Controls.Add(this.tbProjectName, 1, 1);
			this.panelMain.Controls.Add(this.btnCreateProject, 1, 7);
			this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMain.Location = new System.Drawing.Point(0, 0);
			this.panelMain.Name = "panelMain";
			this.panelMain.RowCount = 8;
			this.panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.panelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.panelMain.Size = new System.Drawing.Size(756, 274);
			this.panelMain.TabIndex = 3;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
			this.tableLayoutPanel1.Controls.Add(this.btnBrowseOutputDirectory, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tbOutputDirectory, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(133, 195);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(620, 26);
			this.tableLayoutPanel1.TabIndex = 16;
			// 
			// btnBrowseOutputDirectory
			// 
			this.btnBrowseOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnBrowseOutputDirectory.Location = new System.Drawing.Point(548, 3);
			this.btnBrowseOutputDirectory.Name = "btnBrowseOutputDirectory";
			this.btnBrowseOutputDirectory.Size = new System.Drawing.Size(69, 20);
			this.btnBrowseOutputDirectory.TabIndex = 11;
			this.btnBrowseOutputDirectory.Text = "Browse";
			this.btnBrowseOutputDirectory.UseVisualStyleBackColor = true;
			this.btnBrowseOutputDirectory.Click += new System.EventHandler(this.btnBrowseOutputDirectory_Click);
			// 
			// tbOutputDirectory
			// 
			this.tbOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbOutputDirectory.Location = new System.Drawing.Point(3, 3);
			this.tbOutputDirectory.Name = "tbOutputDirectory";
			this.tbOutputDirectory.Size = new System.Drawing.Size(539, 20);
			this.tbOutputDirectory.TabIndex = 10;
			// 
			// lbOutputDirectory
			// 
			this.lbOutputDirectory.AutoSize = true;
			this.lbOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbOutputDirectory.Location = new System.Drawing.Point(3, 192);
			this.lbOutputDirectory.Name = "lbOutputDirectory";
			this.lbOutputDirectory.Size = new System.Drawing.Size(124, 32);
			this.lbOutputDirectory.TabIndex = 15;
			this.lbOutputDirectory.Text = "Output Directory*:";
			this.lbOutputDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelDefaultPlugins
			// 
			this.panelDefaultPlugins.ColumnCount = 2;
			this.panelDefaultPlugins.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.panelDefaultPlugins.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
			this.panelDefaultPlugins.Controls.Add(this.btnBrowseDefaultPlugins, 0, 0);
			this.panelDefaultPlugins.Controls.Add(this.tbUseDefaultPlugins, 0, 0);
			this.panelDefaultPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelDefaultPlugins.Location = new System.Drawing.Point(133, 163);
			this.panelDefaultPlugins.Name = "panelDefaultPlugins";
			this.panelDefaultPlugins.RowCount = 1;
			this.panelDefaultPlugins.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.panelDefaultPlugins.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.panelDefaultPlugins.Size = new System.Drawing.Size(620, 26);
			this.panelDefaultPlugins.TabIndex = 14;
			// 
			// btnBrowseDefaultPlugins
			// 
			this.btnBrowseDefaultPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnBrowseDefaultPlugins.Location = new System.Drawing.Point(548, 3);
			this.btnBrowseDefaultPlugins.Name = "btnBrowseDefaultPlugins";
			this.btnBrowseDefaultPlugins.Size = new System.Drawing.Size(69, 20);
			this.btnBrowseDefaultPlugins.TabIndex = 11;
			this.btnBrowseDefaultPlugins.Text = "Browse";
			this.btnBrowseDefaultPlugins.UseVisualStyleBackColor = true;
			this.btnBrowseDefaultPlugins.Click += new System.EventHandler(this.btnBrowseDefaultPlugins_Click);
			// 
			// tbUseDefaultPlugins
			// 
			this.tbUseDefaultPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbUseDefaultPlugins.Location = new System.Drawing.Point(3, 3);
			this.tbUseDefaultPlugins.Name = "tbUseDefaultPlugins";
			this.tbUseDefaultPlugins.Size = new System.Drawing.Size(539, 20);
			this.tbUseDefaultPlugins.TabIndex = 10;
			// 
			// lbDefaultPlugins
			// 
			this.lbDefaultPlugins.AutoSize = true;
			this.lbDefaultPlugins.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbDefaultPlugins.Location = new System.Drawing.Point(3, 160);
			this.lbDefaultPlugins.Name = "lbDefaultPlugins";
			this.lbDefaultPlugins.Size = new System.Drawing.Size(124, 32);
			this.lbDefaultPlugins.TabIndex = 13;
			this.lbDefaultPlugins.Text = "Use Default Plugins:";
			this.lbDefaultPlugins.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelConnectionString
			// 
			this.panelConnectionString.ColumnCount = 2;
			this.panelConnectionString.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.panelConnectionString.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
			this.panelConnectionString.Controls.Add(this.btnTest, 0, 0);
			this.panelConnectionString.Controls.Add(this.tbConnectionString, 0, 0);
			this.panelConnectionString.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelConnectionString.Location = new System.Drawing.Point(133, 131);
			this.panelConnectionString.Name = "panelConnectionString";
			this.panelConnectionString.RowCount = 1;
			this.panelConnectionString.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.panelConnectionString.Size = new System.Drawing.Size(620, 26);
			this.panelConnectionString.TabIndex = 12;
			// 
			// btnTest
			// 
			this.btnTest.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnTest.Location = new System.Drawing.Point(548, 3);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(69, 20);
			this.btnTest.TabIndex = 11;
			this.btnTest.Text = "Test";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// tbConnectionString
			// 
			this.tbConnectionString.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbConnectionString.Location = new System.Drawing.Point(3, 3);
			this.tbConnectionString.Name = "tbConnectionString";
			this.tbConnectionString.Size = new System.Drawing.Size(539, 20);
			this.tbConnectionString.TabIndex = 10;
			// 
			// lbConnectionString
			// 
			this.lbConnectionString.AutoSize = true;
			this.lbConnectionString.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbConnectionString.Location = new System.Drawing.Point(3, 128);
			this.lbConnectionString.Name = "lbConnectionString";
			this.lbConnectionString.Size = new System.Drawing.Size(124, 32);
			this.lbConnectionString.TabIndex = 8;
			this.lbConnectionString.Text = "Connection String*:";
			this.lbConnectionString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelDatabase
			// 
			this.panelDatabase.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelDatabase.Controls.Add(this.rbPostgreSQL);
			this.panelDatabase.Controls.Add(this.rbMySQL);
			this.panelDatabase.Controls.Add(this.rbMSSQL);
			this.panelDatabase.Controls.Add(this.rbSQLite);
			this.panelDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelDatabase.Location = new System.Drawing.Point(133, 99);
			this.panelDatabase.Name = "panelDatabase";
			this.panelDatabase.Size = new System.Drawing.Size(620, 26);
			this.panelDatabase.TabIndex = 7;
			// 
			// rbPostgreSQL
			// 
			this.rbPostgreSQL.AutoSize = true;
			this.rbPostgreSQL.Location = new System.Drawing.Point(144, 3);
			this.rbPostgreSQL.Name = "rbPostgreSQL";
			this.rbPostgreSQL.Size = new System.Drawing.Size(82, 17);
			this.rbPostgreSQL.TabIndex = 2;
			this.rbPostgreSQL.Tag = "postgresql";
			this.rbPostgreSQL.Text = "PostgreSQL";
			this.rbPostgreSQL.UseVisualStyleBackColor = true;
			this.rbPostgreSQL.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// rbMySQL
			// 
			this.rbMySQL.AutoSize = true;
			this.rbMySQL.Location = new System.Drawing.Point(78, 3);
			this.rbMySQL.Name = "rbMySQL";
			this.rbMySQL.Size = new System.Drawing.Size(60, 17);
			this.rbMySQL.TabIndex = 1;
			this.rbMySQL.Tag = "mysql";
			this.rbMySQL.Text = "MySQL";
			this.rbMySQL.UseVisualStyleBackColor = true;
			this.rbMySQL.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// rbMSSQL
			// 
			this.rbMSSQL.AutoSize = true;
			this.rbMSSQL.Location = new System.Drawing.Point(3, 3);
			this.rbMSSQL.Name = "rbMSSQL";
			this.rbMSSQL.Size = new System.Drawing.Size(62, 17);
			this.rbMSSQL.TabIndex = 0;
			this.rbMSSQL.Tag = "mssql";
			this.rbMSSQL.Text = "MSSQL";
			this.rbMSSQL.UseVisualStyleBackColor = true;
			this.rbMSSQL.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// rbSQLite
			// 
			this.rbSQLite.AutoSize = true;
			this.rbSQLite.Checked = true;
			this.rbSQLite.Location = new System.Drawing.Point(227, 3);
			this.rbSQLite.Name = "rbSQLite";
			this.rbSQLite.Size = new System.Drawing.Size(57, 17);
			this.rbSQLite.TabIndex = 3;
			this.rbSQLite.TabStop = true;
			this.rbSQLite.Tag = "sqlite";
			this.rbSQLite.Text = "SQLite";
			this.rbSQLite.UseVisualStyleBackColor = true;
			this.rbSQLite.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// lbDatabase
			// 
			this.lbDatabase.AutoSize = true;
			this.lbDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbDatabase.Location = new System.Drawing.Point(3, 96);
			this.lbDatabase.Name = "lbDatabase";
			this.lbDatabase.Size = new System.Drawing.Size(124, 32);
			this.lbDatabase.TabIndex = 6;
			this.lbDatabase.Text = "Database*:";
			this.lbDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbDescription
			// 
			this.tbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbDescription.Location = new System.Drawing.Point(133, 70);
			this.tbDescription.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.Size = new System.Drawing.Size(620, 20);
			this.tbDescription.TabIndex = 5;
			// 
			// lbDescription
			// 
			this.lbDescription.AutoSize = true;
			this.lbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbDescription.Location = new System.Drawing.Point(3, 64);
			this.lbDescription.Name = "lbDescription";
			this.lbDescription.Size = new System.Drawing.Size(124, 32);
			this.lbDescription.TabIndex = 4;
			this.lbDescription.Text = "Description:";
			this.lbDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbProjectName
			// 
			this.lbProjectName.AutoSize = true;
			this.lbProjectName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbProjectName.Location = new System.Drawing.Point(3, 32);
			this.lbProjectName.Name = "lbProjectName";
			this.lbProjectName.Size = new System.Drawing.Size(124, 32);
			this.lbProjectName.TabIndex = 2;
			this.lbProjectName.Text = "Project Name*:";
			this.lbProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbProjectName
			// 
			this.tbProjectName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbProjectName.Location = new System.Drawing.Point(133, 38);
			this.tbProjectName.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
			this.tbProjectName.Name = "tbProjectName";
			this.tbProjectName.Size = new System.Drawing.Size(620, 20);
			this.tbProjectName.TabIndex = 3;
			this.tbProjectName.Text = "Hello.World";
			// 
			// btnCreateProject
			// 
			this.btnCreateProject.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnCreateProject.Location = new System.Drawing.Point(133, 232);
			this.btnCreateProject.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
			this.btnCreateProject.Name = "btnCreateProject";
			this.btnCreateProject.Size = new System.Drawing.Size(620, 34);
			this.btnCreateProject.TabIndex = 17;
			this.btnCreateProject.Text = "Create Project";
			this.btnCreateProject.UseVisualStyleBackColor = true;
			this.btnCreateProject.Click += new System.EventHandler(this.btnCreateProject_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(756, 274);
			this.Controls.Add(this.panelMain);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ZKWeb Project Creator";
			this.panelProjectType.ResumeLayout(false);
			this.panelProjectType.PerformLayout();
			this.panelMain.ResumeLayout(false);
			this.panelMain.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panelDefaultPlugins.ResumeLayout(false);
			this.panelDefaultPlugins.PerformLayout();
			this.panelConnectionString.ResumeLayout(false);
			this.panelConnectionString.PerformLayout();
			this.panelDatabase.ResumeLayout(false);
			this.panelDatabase.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lbProjectType;
		private System.Windows.Forms.Panel panelProjectType;
		private System.Windows.Forms.RadioButton rbAspNet;
		private System.Windows.Forms.RadioButton rbAspNetCore;
		private System.Windows.Forms.RadioButton rbOwin;
		private System.Windows.Forms.TableLayoutPanel panelMain;
		private System.Windows.Forms.Label lbProjectName;
		private System.Windows.Forms.TextBox tbProjectName;
		private System.Windows.Forms.Label lbDescription;
		private System.Windows.Forms.TextBox tbDescription;
		private System.Windows.Forms.Label lbDatabase;
		private System.Windows.Forms.Panel panelDatabase;
		private System.Windows.Forms.RadioButton rbPostgreSQL;
		private System.Windows.Forms.RadioButton rbMySQL;
		private System.Windows.Forms.RadioButton rbMSSQL;
		private System.Windows.Forms.RadioButton rbSQLite;
		private System.Windows.Forms.Label lbConnectionString;
		private System.Windows.Forms.TableLayoutPanel panelConnectionString;
		private System.Windows.Forms.Button btnTest;
		private System.Windows.Forms.TextBox tbConnectionString;
		private System.Windows.Forms.Label lbDefaultPlugins;
		private System.Windows.Forms.TableLayoutPanel panelDefaultPlugins;
		private System.Windows.Forms.Button btnBrowseDefaultPlugins;
		private System.Windows.Forms.TextBox tbUseDefaultPlugins;
		private System.Windows.Forms.Label lbOutputDirectory;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button btnBrowseOutputDirectory;
		private System.Windows.Forms.TextBox tbOutputDirectory;
		private System.Windows.Forms.Button btnCreateProject;
	}
}

