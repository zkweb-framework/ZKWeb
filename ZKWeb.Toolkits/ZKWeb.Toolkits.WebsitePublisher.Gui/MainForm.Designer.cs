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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
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
			this.btnPublishWebsite = new System.Windows.Forms.Button();
			this.lbIgnorePattern = new System.Windows.Forms.Label();
			this.tbIgnorePattern = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.panelOutputDirectory.SuspendLayout();
			this.panelWebRoot.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.tbIgnorePattern, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.lbIgnorePattern, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.btnPublishWebsite, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.panelOutputDirectory, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.lbOutputDirectory, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.tbOutputName, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.lbOutputName, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panelWebRoot, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.lbWebRoot, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(663, 195);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// panelOutputDirectory
			// 
			this.panelOutputDirectory.ColumnCount = 2;
			this.panelOutputDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.panelOutputDirectory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
			this.panelOutputDirectory.Controls.Add(this.btnBrowseOutputDirectory, 0, 0);
			this.panelOutputDirectory.Controls.Add(this.tbOutputDirectory, 0, 0);
			this.panelOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelOutputDirectory.Location = new System.Drawing.Point(133, 67);
			this.panelOutputDirectory.Name = "panelOutputDirectory";
			this.panelOutputDirectory.RowCount = 1;
			this.panelOutputDirectory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.panelOutputDirectory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.panelOutputDirectory.Size = new System.Drawing.Size(527, 26);
			this.panelOutputDirectory.TabIndex = 19;
			// 
			// btnBrowseOutputDirectory
			// 
			this.btnBrowseOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnBrowseOutputDirectory.Location = new System.Drawing.Point(455, 3);
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
			this.tbOutputDirectory.Size = new System.Drawing.Size(446, 20);
			this.tbOutputDirectory.TabIndex = 10;
			// 
			// lbOutputDirectory
			// 
			this.lbOutputDirectory.AutoSize = true;
			this.lbOutputDirectory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbOutputDirectory.Location = new System.Drawing.Point(3, 64);
			this.lbOutputDirectory.Name = "lbOutputDirectory";
			this.lbOutputDirectory.Size = new System.Drawing.Size(124, 32);
			this.lbOutputDirectory.TabIndex = 18;
			this.lbOutputDirectory.Text = "Output Directory*:";
			this.lbOutputDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbOutputName
			// 
			this.tbOutputName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbOutputName.Location = new System.Drawing.Point(133, 38);
			this.tbOutputName.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
			this.tbOutputName.Name = "tbOutputName";
			this.tbOutputName.Size = new System.Drawing.Size(527, 20);
			this.tbOutputName.TabIndex = 17;
			// 
			// lbOutputName
			// 
			this.lbOutputName.AutoSize = true;
			this.lbOutputName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbOutputName.Location = new System.Drawing.Point(3, 32);
			this.lbOutputName.Name = "lbOutputName";
			this.lbOutputName.Size = new System.Drawing.Size(124, 32);
			this.lbOutputName.TabIndex = 16;
			this.lbOutputName.Text = "Output Name*:";
			this.lbOutputName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelWebRoot
			// 
			this.panelWebRoot.ColumnCount = 2;
			this.panelWebRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.panelWebRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
			this.panelWebRoot.Controls.Add(this.btnBrowseWebRoot, 0, 0);
			this.panelWebRoot.Controls.Add(this.tbWebRoot, 0, 0);
			this.panelWebRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelWebRoot.Location = new System.Drawing.Point(133, 3);
			this.panelWebRoot.Name = "panelWebRoot";
			this.panelWebRoot.RowCount = 1;
			this.panelWebRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.panelWebRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
			this.panelWebRoot.Size = new System.Drawing.Size(527, 26);
			this.panelWebRoot.TabIndex = 15;
			// 
			// btnBrowseWebRoot
			// 
			this.btnBrowseWebRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnBrowseWebRoot.Location = new System.Drawing.Point(455, 3);
			this.btnBrowseWebRoot.Name = "btnBrowseWebRoot";
			this.btnBrowseWebRoot.Size = new System.Drawing.Size(69, 20);
			this.btnBrowseWebRoot.TabIndex = 11;
			this.btnBrowseWebRoot.Text = "Browse";
			this.btnBrowseWebRoot.UseVisualStyleBackColor = true;
			this.btnBrowseWebRoot.Click += new System.EventHandler(this.btnBrowseWebRoot_Click);
			// 
			// tbWebRoot
			// 
			this.tbWebRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbWebRoot.Location = new System.Drawing.Point(3, 3);
			this.tbWebRoot.Name = "tbWebRoot";
			this.tbWebRoot.Size = new System.Drawing.Size(446, 20);
			this.tbWebRoot.TabIndex = 10;
			// 
			// lbWebRoot
			// 
			this.lbWebRoot.AutoSize = true;
			this.lbWebRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbWebRoot.Location = new System.Drawing.Point(3, 0);
			this.lbWebRoot.Name = "lbWebRoot";
			this.lbWebRoot.Size = new System.Drawing.Size(124, 32);
			this.lbWebRoot.TabIndex = 1;
			this.lbWebRoot.Text = "Website Root*:";
			this.lbWebRoot.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnPublishWebsite
			// 
			this.btnPublishWebsite.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnPublishWebsite.Location = new System.Drawing.Point(133, 136);
			this.btnPublishWebsite.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
			this.btnPublishWebsite.Name = "btnPublishWebsite";
			this.btnPublishWebsite.Size = new System.Drawing.Size(527, 51);
			this.btnPublishWebsite.TabIndex = 21;
			this.btnPublishWebsite.Text = "Publish Website";
			this.btnPublishWebsite.UseVisualStyleBackColor = true;
			this.btnPublishWebsite.Click += new System.EventHandler(this.btnPublishWebsite_Click);
			// 
			// lbIgnorePattern
			// 
			this.lbIgnorePattern.AutoSize = true;
			this.lbIgnorePattern.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbIgnorePattern.Location = new System.Drawing.Point(3, 96);
			this.lbIgnorePattern.Name = "lbIgnorePattern";
			this.lbIgnorePattern.Size = new System.Drawing.Size(124, 32);
			this.lbIgnorePattern.TabIndex = 22;
			this.lbIgnorePattern.Text = "Ignore Pattern:";
			this.lbIgnorePattern.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbIgnorePattern
			// 
			this.tbIgnorePattern.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbIgnorePattern.Location = new System.Drawing.Point(133, 102);
			this.tbIgnorePattern.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
			this.tbIgnorePattern.Name = "tbIgnorePattern";
			this.tbIgnorePattern.Size = new System.Drawing.Size(527, 20);
			this.tbIgnorePattern.TabIndex = 23;
			this.tbIgnorePattern.Text = ".*node_modules.*";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(663, 195);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "MainForm";
			this.Text = "ZKWeb Website Publisher";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panelOutputDirectory.ResumeLayout(false);
			this.panelOutputDirectory.PerformLayout();
			this.panelWebRoot.ResumeLayout(false);
			this.panelWebRoot.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
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
	}
}

