namespace ZKWeb.Toolkits.WebsitePublisher.Gui {
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
			this.lbFramework = new System.Windows.Forms.Label();
			this.tbIgnorePattern = new System.Windows.Forms.TextBox();
			this.lbIgnorePattern = new System.Windows.Forms.Label();
			this.btnPublishWebsite = new System.Windows.Forms.Button();
			this.panelOutputDirectory = new System.Windows.Forms.TableLayoutPanel();
			this.btnBrowseOutputDirectory = new System.Windows.Forms.Button();
			this.tbOutputDirectory = new System.Windows.Forms.TextBox();
			this.lbOutputDirectory = new System.Windows.Forms.Label();
			this.tbOutputName = new System.Windows.Forms.TextBox();
			this.lbOutputName = new System.Windows.Forms.Label();
			this.panelWebRoot = new System.Windows.Forms.TableLayoutPanel();
			this.btnBrowseWebRoot = new System.Windows.Forms.Button();
			this.tbWebRoot = new System.Windows.Forms.TextBox();
			this.lbWebRoot = new System.Windows.Forms.Label();
			this.cbFramework = new System.Windows.Forms.ComboBox();
			this.lbConfiguration = new System.Windows.Forms.Label();
			this.cbConfiguration = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.panelOutputDirectory.SuspendLayout();
			this.panelWebRoot.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbFramework
			// 
			resources.ApplyResources(this.lbFramework, "lbFramework");
			this.lbFramework.Name = "lbFramework";
			// 
			// tbIgnorePattern
			// 
			resources.ApplyResources(this.tbIgnorePattern, "tbIgnorePattern");
			this.tbIgnorePattern.Name = "tbIgnorePattern";
			// 
			// lbIgnorePattern
			// 
			resources.ApplyResources(this.lbIgnorePattern, "lbIgnorePattern");
			this.lbIgnorePattern.Name = "lbIgnorePattern";
			// 
			// btnPublishWebsite
			// 
			resources.ApplyResources(this.btnPublishWebsite, "btnPublishWebsite");
			this.btnPublishWebsite.Name = "btnPublishWebsite";
			this.btnPublishWebsite.UseVisualStyleBackColor = true;
			this.btnPublishWebsite.Click += new System.EventHandler(this.btnPublishWebsite_Click);
			// 
			// panelOutputDirectory
			// 
			resources.ApplyResources(this.panelOutputDirectory, "panelOutputDirectory");
			this.panelOutputDirectory.Controls.Add(this.btnBrowseOutputDirectory, 0, 0);
			this.panelOutputDirectory.Controls.Add(this.tbOutputDirectory, 0, 0);
			this.panelOutputDirectory.Name = "panelOutputDirectory";
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
			// tbOutputName
			// 
			resources.ApplyResources(this.tbOutputName, "tbOutputName");
			this.tbOutputName.Name = "tbOutputName";
			// 
			// lbOutputName
			// 
			resources.ApplyResources(this.lbOutputName, "lbOutputName");
			this.lbOutputName.Name = "lbOutputName";
			// 
			// panelWebRoot
			// 
			resources.ApplyResources(this.panelWebRoot, "panelWebRoot");
			this.panelWebRoot.Controls.Add(this.btnBrowseWebRoot, 0, 0);
			this.panelWebRoot.Controls.Add(this.tbWebRoot, 0, 0);
			this.panelWebRoot.Name = "panelWebRoot";
			// 
			// btnBrowseWebRoot
			// 
			resources.ApplyResources(this.btnBrowseWebRoot, "btnBrowseWebRoot");
			this.btnBrowseWebRoot.Name = "btnBrowseWebRoot";
			this.btnBrowseWebRoot.UseVisualStyleBackColor = true;
			this.btnBrowseWebRoot.Click += new System.EventHandler(this.btnBrowseWebRoot_Click);
			// 
			// tbWebRoot
			// 
			resources.ApplyResources(this.tbWebRoot, "tbWebRoot");
			this.tbWebRoot.Name = "tbWebRoot";
			// 
			// lbWebRoot
			// 
			resources.ApplyResources(this.lbWebRoot, "lbWebRoot");
			this.lbWebRoot.Name = "lbWebRoot";
			// 
			// cbFramework
			// 
			resources.ApplyResources(this.cbFramework, "cbFramework");
			this.cbFramework.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("cbFramework.AutoCompleteCustomSource"),
            resources.GetString("cbFramework.AutoCompleteCustomSource1")});
			this.cbFramework.FormattingEnabled = true;
			this.cbFramework.Items.AddRange(new object[] {
            resources.GetString("cbFramework.Items"),
            resources.GetString("cbFramework.Items1")});
			this.cbFramework.Name = "cbFramework";
			// 
			// lbConfiguration
			// 
			resources.ApplyResources(this.lbConfiguration, "lbConfiguration");
			this.lbConfiguration.Name = "lbConfiguration";
			// 
			// cbConfiguration
			// 
			resources.ApplyResources(this.cbConfiguration, "cbConfiguration");
			this.cbConfiguration.FormattingEnabled = true;
			this.cbConfiguration.Items.AddRange(new object[] {
            resources.GetString("cbConfiguration.Items"),
            resources.GetString("cbConfiguration.Items1")});
			this.cbConfiguration.Name = "cbConfiguration";
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
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cbConfiguration);
			this.Controls.Add(this.lbConfiguration);
			this.Controls.Add(this.btnPublishWebsite);
			this.Controls.Add(this.lbFramework);
			this.Controls.Add(this.cbFramework);
			this.Controls.Add(this.tbIgnorePattern);
			this.Controls.Add(this.lbWebRoot);
			this.Controls.Add(this.lbIgnorePattern);
			this.Controls.Add(this.panelWebRoot);
			this.Controls.Add(this.lbOutputName);
			this.Controls.Add(this.panelOutputDirectory);
			this.Controls.Add(this.tbOutputName);
			this.Controls.Add(this.lbOutputDirectory);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.panelOutputDirectory.ResumeLayout(false);
			this.panelOutputDirectory.PerformLayout();
			this.panelWebRoot.ResumeLayout(false);
			this.panelWebRoot.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lbWebRoot;
		private System.Windows.Forms.TableLayoutPanel panelWebRoot;
		private System.Windows.Forms.Button btnBrowseWebRoot;
		private System.Windows.Forms.TextBox tbWebRoot;
		private System.Windows.Forms.Label lbOutputName;
		private System.Windows.Forms.TextBox tbOutputName;
		private System.Windows.Forms.Label lbOutputDirectory;
		private System.Windows.Forms.TableLayoutPanel panelOutputDirectory;
		private System.Windows.Forms.Button btnBrowseOutputDirectory;
		private System.Windows.Forms.TextBox tbOutputDirectory;
		private System.Windows.Forms.Button btnPublishWebsite;
		private System.Windows.Forms.Label lbIgnorePattern;
		private System.Windows.Forms.TextBox tbIgnorePattern;
		private System.Windows.Forms.Label lbFramework;
		private System.Windows.Forms.ComboBox cbFramework;
		private System.Windows.Forms.Label lbConfiguration;
		private System.Windows.Forms.ComboBox cbConfiguration;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
	}
}

