using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZKWeb.Toolkits.WebsitePublisher.Model;

namespace ZKWeb.Toolkits.WebsitePublisher.Gui {
	public partial class MainForm : Form {
		public MainForm() {
			InitializeComponent();
		}

		private void btnBrowseWebRoot_Click(object sender, EventArgs e) {
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == DialogResult.OK) {
				tbWebRoot.Text = dialog.SelectedPath;
				tbOutputName.Text = Path.GetFileName(dialog.SelectedPath).Split('.')[0];
			}
		}

		private void btnBrowseOutputDirectory_Click(object sender, EventArgs e) {
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == DialogResult.OK) {
				tbOutputDirectory.Text = dialog.SelectedPath;
			}
		}

		private async void btnPublishWebsite_Click(object sender, EventArgs e) {
			btnPublishWebsite.Enabled = false;
			try {
				var parameters = new PublishWebsiteParameters();
				parameters.WebRoot = tbWebRoot.Text;
				parameters.OutputName = tbOutputName.Text;
				parameters.OutputDirectory = tbOutputDirectory.Text;
				parameters.IgnorePattern = tbIgnorePattern.Text;
				parameters.Framework = tbFramework.Text;
				var publisher = new WebsitePublisher(parameters);
				await Task.Run(() => publisher.PublishWebsite());
				MessageBox.Show("Success");
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			} finally {
				btnPublishWebsite.Enabled = true;
			}
		}
	}
}
