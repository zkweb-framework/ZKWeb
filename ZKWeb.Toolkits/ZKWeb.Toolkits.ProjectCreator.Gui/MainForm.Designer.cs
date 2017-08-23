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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.lbProjectType = new System.Windows.Forms.Label();
			this.panelProjectType = new System.Windows.Forms.Panel();
			this.rbOwin = new System.Windows.Forms.RadioButton();
			this.rbAspNetCore = new System.Windows.Forms.RadioButton();
			this.rbAspNet = new System.Windows.Forms.RadioButton();
			this.panelORM = new System.Windows.Forms.Panel();
			this.rbMongoDB = new System.Windows.Forms.RadioButton();
			this.rbDapper = new System.Windows.Forms.RadioButton();
			this.rbEFCore = new System.Windows.Forms.RadioButton();
			this.rbNHibernate = new System.Windows.Forms.RadioButton();
			this.lbORM = new System.Windows.Forms.Label();
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
			this.rbDMongoDB = new System.Windows.Forms.RadioButton();
			this.rbInMemory = new System.Windows.Forms.RadioButton();
			this.rbPostgreSQL = new System.Windows.Forms.RadioButton();
			this.rbSQLite = new System.Windows.Forms.RadioButton();
			this.rbMySQL = new System.Windows.Forms.RadioButton();
			this.rbMSSQL = new System.Windows.Forms.RadioButton();
			this.lbDatabase = new System.Windows.Forms.Label();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.lbDescription = new System.Windows.Forms.Label();
			this.lbProjectName = new System.Windows.Forms.Label();
			this.tbProjectName = new System.Windows.Forms.TextBox();
			this.btnCreateProject = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.panelProjectType.SuspendLayout();
			this.panelORM.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panelDefaultPlugins.SuspendLayout();
			this.panelConnectionString.SuspendLayout();
			this.panelDatabase.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbProjectType
			// 
			resources.ApplyResources(this.lbProjectType, "lbProjectType");
			this.lbProjectType.Name = "lbProjectType";
			// 
			// panelProjectType
			// 
			resources.ApplyResources(this.panelProjectType, "panelProjectType");
			this.panelProjectType.Controls.Add(this.rbOwin);
			this.panelProjectType.Controls.Add(this.rbAspNetCore);
			this.panelProjectType.Controls.Add(this.rbAspNet);
			this.panelProjectType.Name = "panelProjectType";
			// 
			// rbOwin
			// 
			resources.ApplyResources(this.rbOwin, "rbOwin");
			this.rbOwin.Name = "rbOwin";
			this.rbOwin.Tag = "Owin";
			this.rbOwin.UseVisualStyleBackColor = true;
			// 
			// rbAspNetCore
			// 
			resources.ApplyResources(this.rbAspNetCore, "rbAspNetCore");
			this.rbAspNetCore.Checked = true;
			this.rbAspNetCore.Name = "rbAspNetCore";
			this.rbAspNetCore.TabStop = true;
			this.rbAspNetCore.Tag = "AspNetCore";
			this.rbAspNetCore.UseVisualStyleBackColor = true;
			// 
			// rbAspNet
			// 
			resources.ApplyResources(this.rbAspNet, "rbAspNet");
			this.rbAspNet.Name = "rbAspNet";
			this.rbAspNet.Tag = "AspNet";
			this.rbAspNet.UseVisualStyleBackColor = true;
			// 
			// panelORM
			// 
			resources.ApplyResources(this.panelORM, "panelORM");
			this.panelORM.Controls.Add(this.rbMongoDB);
			this.panelORM.Controls.Add(this.rbDapper);
			this.panelORM.Controls.Add(this.rbEFCore);
			this.panelORM.Controls.Add(this.rbNHibernate);
			this.panelORM.Name = "panelORM";
			// 
			// rbMongoDB
			// 
			resources.ApplyResources(this.rbMongoDB, "rbMongoDB");
			this.rbMongoDB.Name = "rbMongoDB";
			this.rbMongoDB.Tag = "MongoDB";
			this.rbMongoDB.UseVisualStyleBackColor = true;
			this.rbMongoDB.CheckedChanged += new System.EventHandler(this.onORMCheckedChanged);
			// 
			// rbDapper
			// 
			resources.ApplyResources(this.rbDapper, "rbDapper");
			this.rbDapper.Name = "rbDapper";
			this.rbDapper.Tag = "Dapper";
			this.rbDapper.UseVisualStyleBackColor = true;
			this.rbDapper.CheckedChanged += new System.EventHandler(this.onORMCheckedChanged);
			// 
			// rbEFCore
			// 
			resources.ApplyResources(this.rbEFCore, "rbEFCore");
			this.rbEFCore.Name = "rbEFCore";
			this.rbEFCore.Tag = "EFCore";
			this.rbEFCore.UseVisualStyleBackColor = true;
			this.rbEFCore.CheckedChanged += new System.EventHandler(this.onORMCheckedChanged);
			// 
			// rbNHibernate
			// 
			resources.ApplyResources(this.rbNHibernate, "rbNHibernate");
			this.rbNHibernate.Checked = true;
			this.rbNHibernate.Name = "rbNHibernate";
			this.rbNHibernate.TabStop = true;
			this.rbNHibernate.Tag = "NHibernate";
			this.rbNHibernate.UseVisualStyleBackColor = true;
			this.rbNHibernate.CheckedChanged += new System.EventHandler(this.onORMCheckedChanged);
			// 
			// lbORM
			// 
			resources.ApplyResources(this.lbORM, "lbORM");
			this.lbORM.Name = "lbORM";
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.btnBrowseOutputDirectory, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tbOutputDirectory, 0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// btnBrowseOutputDirectory
			// 
			resources.ApplyResources(this.btnBrowseOutputDirectory, "btnBrowseOutputDirectory");
			this.btnBrowseOutputDirectory.Name = "btnBrowseOutputDirectory";
			this.btnBrowseOutputDirectory.UseVisualStyleBackColor = true;
			this.btnBrowseOutputDirectory.Click += new System.EventHandler(this.btnBrowseOutputDirectory_Click);
			// 
			// tbOutputDirectory
			// 
			resources.ApplyResources(this.tbOutputDirectory, "tbOutputDirectory");
			this.tbOutputDirectory.Name = "tbOutputDirectory";
			// 
			// lbOutputDirectory
			// 
			resources.ApplyResources(this.lbOutputDirectory, "lbOutputDirectory");
			this.lbOutputDirectory.Name = "lbOutputDirectory";
			// 
			// panelDefaultPlugins
			// 
			resources.ApplyResources(this.panelDefaultPlugins, "panelDefaultPlugins");
			this.panelDefaultPlugins.Controls.Add(this.btnBrowseDefaultPlugins, 0, 0);
			this.panelDefaultPlugins.Controls.Add(this.tbUseDefaultPlugins, 0, 0);
			this.panelDefaultPlugins.Name = "panelDefaultPlugins";
			// 
			// btnBrowseDefaultPlugins
			// 
			resources.ApplyResources(this.btnBrowseDefaultPlugins, "btnBrowseDefaultPlugins");
			this.btnBrowseDefaultPlugins.Name = "btnBrowseDefaultPlugins";
			this.btnBrowseDefaultPlugins.UseVisualStyleBackColor = true;
			this.btnBrowseDefaultPlugins.Click += new System.EventHandler(this.btnBrowseDefaultPlugins_Click);
			// 
			// tbUseDefaultPlugins
			// 
			resources.ApplyResources(this.tbUseDefaultPlugins, "tbUseDefaultPlugins");
			this.tbUseDefaultPlugins.Name = "tbUseDefaultPlugins";
			// 
			// lbDefaultPlugins
			// 
			resources.ApplyResources(this.lbDefaultPlugins, "lbDefaultPlugins");
			this.lbDefaultPlugins.Name = "lbDefaultPlugins";
			// 
			// panelConnectionString
			// 
			resources.ApplyResources(this.panelConnectionString, "panelConnectionString");
			this.panelConnectionString.Controls.Add(this.btnTest, 0, 0);
			this.panelConnectionString.Controls.Add(this.tbConnectionString, 0, 0);
			this.panelConnectionString.Name = "panelConnectionString";
			// 
			// btnTest
			// 
			resources.ApplyResources(this.btnTest, "btnTest");
			this.btnTest.Name = "btnTest";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// tbConnectionString
			// 
			resources.ApplyResources(this.tbConnectionString, "tbConnectionString");
			this.tbConnectionString.Name = "tbConnectionString";
			// 
			// lbConnectionString
			// 
			resources.ApplyResources(this.lbConnectionString, "lbConnectionString");
			this.lbConnectionString.Name = "lbConnectionString";
			// 
			// panelDatabase
			// 
			resources.ApplyResources(this.panelDatabase, "panelDatabase");
			this.panelDatabase.Controls.Add(this.rbDMongoDB);
			this.panelDatabase.Controls.Add(this.rbInMemory);
			this.panelDatabase.Controls.Add(this.rbPostgreSQL);
			this.panelDatabase.Controls.Add(this.rbSQLite);
			this.panelDatabase.Controls.Add(this.rbMySQL);
			this.panelDatabase.Controls.Add(this.rbMSSQL);
			this.panelDatabase.Name = "panelDatabase";
			// 
			// rbDMongoDB
			// 
			resources.ApplyResources(this.rbDMongoDB, "rbDMongoDB");
			this.rbDMongoDB.Name = "rbDMongoDB";
			this.rbDMongoDB.Tag = "MongoDB";
			this.rbDMongoDB.UseVisualStyleBackColor = true;
			this.rbDMongoDB.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// rbInMemory
			// 
			resources.ApplyResources(this.rbInMemory, "rbInMemory");
			this.rbInMemory.Name = "rbInMemory";
			this.rbInMemory.Tag = "InMemory";
			this.rbInMemory.UseVisualStyleBackColor = true;
			this.rbInMemory.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// rbPostgreSQL
			// 
			resources.ApplyResources(this.rbPostgreSQL, "rbPostgreSQL");
			this.rbPostgreSQL.Name = "rbPostgreSQL";
			this.rbPostgreSQL.Tag = "PostgreSQL";
			this.rbPostgreSQL.UseVisualStyleBackColor = true;
			this.rbPostgreSQL.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// rbSQLite
			// 
			resources.ApplyResources(this.rbSQLite, "rbSQLite");
			this.rbSQLite.Name = "rbSQLite";
			this.rbSQLite.Tag = "SQLite";
			this.rbSQLite.UseVisualStyleBackColor = true;
			this.rbSQLite.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// rbMySQL
			// 
			resources.ApplyResources(this.rbMySQL, "rbMySQL");
			this.rbMySQL.Name = "rbMySQL";
			this.rbMySQL.Tag = "MySQL";
			this.rbMySQL.UseVisualStyleBackColor = true;
			this.rbMySQL.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// rbMSSQL
			// 
			resources.ApplyResources(this.rbMSSQL, "rbMSSQL");
			this.rbMSSQL.Checked = true;
			this.rbMSSQL.Name = "rbMSSQL";
			this.rbMSSQL.TabStop = true;
			this.rbMSSQL.Tag = "MSSQL";
			this.rbMSSQL.UseVisualStyleBackColor = true;
			this.rbMSSQL.CheckedChanged += new System.EventHandler(this.onDatabaseCheckedChanged);
			// 
			// lbDatabase
			// 
			resources.ApplyResources(this.lbDatabase, "lbDatabase");
			this.lbDatabase.Name = "lbDatabase";
			// 
			// tbDescription
			// 
			resources.ApplyResources(this.tbDescription, "tbDescription");
			this.tbDescription.Name = "tbDescription";
			// 
			// lbDescription
			// 
			resources.ApplyResources(this.lbDescription, "lbDescription");
			this.lbDescription.Name = "lbDescription";
			// 
			// lbProjectName
			// 
			resources.ApplyResources(this.lbProjectName, "lbProjectName");
			this.lbProjectName.Name = "lbProjectName";
			// 
			// tbProjectName
			// 
			resources.ApplyResources(this.tbProjectName, "tbProjectName");
			this.tbProjectName.Name = "tbProjectName";
			// 
			// btnCreateProject
			// 
			resources.ApplyResources(this.btnCreateProject, "btnCreateProject");
			this.btnCreateProject.Name = "btnCreateProject";
			this.btnCreateProject.UseVisualStyleBackColor = true;
			this.btnCreateProject.Click += new System.EventHandler(this.btnCreateProject_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.ForeColor = System.Drawing.Color.Red;
			this.label2.Name = "label2";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.ForeColor = System.Drawing.Color.Red;
			this.label3.Name = "label3";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.ForeColor = System.Drawing.Color.Red;
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.ForeColor = System.Drawing.Color.Red;
			this.label5.Name = "label5";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.ForeColor = System.Drawing.Color.Red;
			this.label6.Name = "label6";
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.ForeColor = System.Drawing.Color.Red;
			this.label7.Name = "label7";
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCreateProject);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.panelORM);
			this.Controls.Add(this.lbOutputDirectory);
			this.Controls.Add(this.lbProjectType);
			this.Controls.Add(this.panelDefaultPlugins);
			this.Controls.Add(this.lbORM);
			this.Controls.Add(this.lbDefaultPlugins);
			this.Controls.Add(this.panelConnectionString);
			this.Controls.Add(this.panelProjectType);
			this.Controls.Add(this.lbConnectionString);
			this.Controls.Add(this.lbProjectName);
			this.Controls.Add(this.panelDatabase);
			this.Controls.Add(this.tbProjectName);
			this.Controls.Add(this.lbDatabase);
			this.Controls.Add(this.lbDescription);
			this.Controls.Add(this.tbDescription);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "MainForm";
			this.panelProjectType.ResumeLayout(false);
			this.panelProjectType.PerformLayout();
			this.panelORM.ResumeLayout(false);
			this.panelORM.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panelDefaultPlugins.ResumeLayout(false);
			this.panelDefaultPlugins.PerformLayout();
			this.panelConnectionString.ResumeLayout(false);
			this.panelConnectionString.PerformLayout();
			this.panelDatabase.ResumeLayout(false);
			this.panelDatabase.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbProjectType;
		private System.Windows.Forms.Panel panelProjectType;
		private System.Windows.Forms.RadioButton rbAspNet;
		private System.Windows.Forms.RadioButton rbAspNetCore;
		private System.Windows.Forms.RadioButton rbOwin;
		private System.Windows.Forms.Label lbProjectName;
		private System.Windows.Forms.TextBox tbProjectName;
		private System.Windows.Forms.Label lbDescription;
		private System.Windows.Forms.TextBox tbDescription;
		private System.Windows.Forms.Label lbDatabase;
		private System.Windows.Forms.Panel panelDatabase;
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
		private System.Windows.Forms.Label lbORM;
		private System.Windows.Forms.Panel panelORM;
		private System.Windows.Forms.RadioButton rbNHibernate;
		private System.Windows.Forms.RadioButton rbEFCore;
		private System.Windows.Forms.RadioButton rbDapper;
		private System.Windows.Forms.RadioButton rbMongoDB;
		private System.Windows.Forms.RadioButton rbMSSQL;
		private System.Windows.Forms.RadioButton rbMySQL;
		private System.Windows.Forms.RadioButton rbSQLite;
		private System.Windows.Forms.RadioButton rbPostgreSQL;
		private System.Windows.Forms.RadioButton rbInMemory;
		private System.Windows.Forms.RadioButton rbDMongoDB;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
	}
}

