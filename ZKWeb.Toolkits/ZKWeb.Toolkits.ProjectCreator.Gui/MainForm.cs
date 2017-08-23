using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZKWeb.Toolkits.ProjectCreator.Gui.Properties;
using ZKWeb.Toolkits.ProjectCreator.Gui.Utils;
using ZKWeb.Toolkits.ProjectCreator.Model;

namespace ZKWeb.Toolkits.ProjectCreator.Gui {
	public partial class MainForm : Form {
		public MainForm() {
			InitializeComponent();
			onDatabaseCheckedChanged(null, null);
			onORMCheckedChanged(null, null);
		}

		private void onORMCheckedChanged(object sender, EventArgs e) {
			var orm = GetSelectedORM();
			var availableDatabases = CreateProjectParameters.AvailableDatabases[orm];
			foreach (var rb in panelDatabase.Controls.OfType<RadioButton>()) {
				rb.Enabled = availableDatabases.Contains(rb.Tag?.ToString());
			}
			panelDatabase.Controls.OfType<RadioButton>()
				.OrderBy(c => c.TabIndex).First(rb => rb.Enabled).Checked = true;
		}

		private void onDatabaseCheckedChanged(object sender, EventArgs e) {
			if (rbMSSQL.Checked) {
				tbConnectionString.Text = "Server=127.0.0.1;Database=test_db;User Id=test_user;Password=123456;";
			} else if (rbMySQL.Checked) {
				tbConnectionString.Text = "Server=127.0.0.1;Port=3306;Database=test_db;User Id=test_user;Password=123456;";
			} else if (rbPostgreSQL.Checked) {
				tbConnectionString.Text = "Server=127.0.0.1;Port=5432;Database=test_db;User Id=test_user;Password=123456;";
			} else if (rbSQLite.Checked) {
				tbConnectionString.Text = "Data Source={{App_Data}}/test.db;";
			} else if (rbInMemory.Checked) {
				tbConnectionString.Text = "";
			} else if (rbMongoDB.Checked) {
				tbConnectionString.Text = "mongodb://test_user:123456@127.0.0.1:27017/test_db";
			} else {
				tbConnectionString.Text = "";
			}
		}

		private string GetSelectedProjectType() {
			var radio = panelProjectType.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
			return radio?.Tag?.ToString();
		}

		private string GetSelectedORM() {
			var radio = panelORM.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
			return radio?.Tag?.ToString();
		}

		private string GetSelectedDatabase() {
			var radio = panelDatabase.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
			return radio?.Tag?.ToString();
		}

		private void btnTest_Click(object sender, EventArgs e) {
			var database = GetSelectedDatabase();
			var connectionString = tbConnectionString.Text;
			try {
				DatabaseUtils.TestConnectionString(database, connectionString);
				MessageBox.Show(Resources.TestSuccessfully);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void btnBrowseDefaultPlugins_Click(object sender, EventArgs e) {
			var dialog = new OpenFileDialog();
			dialog.Filter = "|plugin.collection*.json";
			if (dialog.ShowDialog() == DialogResult.OK) {
				tbUseDefaultPlugins.Text = dialog.FileName;
			}
		}

		private void btnBrowseOutputDirectory_Click(object sender, EventArgs e) {
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == DialogResult.OK) {
				tbOutputDirectory.Text = dialog.SelectedPath;
			}
		}

		private async void btnCreateProject_Click(object sender, EventArgs e) {
			btnCreateProject.Enabled = false;
			try {
				var parameters = new CreateProjectParameters();
				parameters.ProjectType = GetSelectedProjectType();
				parameters.ProjectName = tbProjectName.Text;
				parameters.ProjectDescription = tbDescription.Text;
				parameters.ORM = GetSelectedORM();
				parameters.Database = GetSelectedDatabase();
				parameters.ConnectionString = tbConnectionString.Text;
				parameters.UseDefaultPlugins = tbUseDefaultPlugins.Text;
				parameters.OutputDirectory = tbOutputDirectory.Text;
				var creator = new ProjectCreator(parameters);
				await Task.Run(() => creator.CreateProject());
				MessageBox.Show(Resources.CreatedSuccessfully);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			} finally {
				btnCreateProject.Enabled = true;
			}
		}
	}
}
