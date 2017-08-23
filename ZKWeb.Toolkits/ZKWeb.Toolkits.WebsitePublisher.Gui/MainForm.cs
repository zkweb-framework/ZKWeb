using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZKWeb.Toolkits.WebsitePublisher.Gui.Properties;
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
				var parts = Path.GetFileName(dialog.SelectedPath).Split('.');
				var outputName = string.Join(".", parts.Take(parts.Length - (parts.Length > 1 ? 1 : 0)));
				tbOutputName.Text = outputName;
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
				parameters.Configuration = cbConfiguration.Text;
				parameters.Framework = cbFramework.Text;
				var publisher = new WebsitePublisher(parameters);
				await Task.Run(() => publisher.PublishWebsite());
				MessageBox.Show(Resources.PublishSuccessfully);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			} finally {
				btnPublishWebsite.Enabled = true;
			}
		}
	}
}
