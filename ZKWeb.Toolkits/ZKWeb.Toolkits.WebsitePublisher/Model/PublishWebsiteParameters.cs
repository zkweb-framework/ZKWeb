using System;

namespace ZKWeb.Toolkits.WebsitePublisher.Model {
	/// <summary>
	/// Publish website parameters
	/// </summary>
	public class PublishWebsiteParameters {
		/// <summary>
		/// Website root
		/// </summary>
		public string WebRoot { get; set; }
		/// <summary>
		/// Output name
		/// </summary>
		public string OutputName { get; set; }
		/// <summary>
		/// Output directory
		/// </summary>
		public string OutputDirectory { get; set; }

		/// <summary>
		/// Check parameters
		/// </summary>
		public void Check() {
			if (string.IsNullOrEmpty(WebRoot)) {
				throw new ArgumentException("WebRoot can't be empty");
			} else if (string.IsNullOrEmpty(OutputName)) {
				throw new ArgumentException("OutputName can't be empty");
			} else if (string.IsNullOrEmpty(OutputDirectory)) {
				throw new ArgumentException("OutputDirectory can't be empty");
			}
		}
	}
}
