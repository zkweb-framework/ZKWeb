using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using ZKWeb.Utils.Extensions;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Utils.Tests.Extensions {
	[UnitTest]
	class HtmlTextWriterExtensionsTest {
		public void AddAttributes() {
			var writer = new HtmlTextWriter(new StringWriter());
			writer.AddAttributes(new[] {
				new KeyValuePair<string, string>("a", "1"),
				new KeyValuePair<string, string>("b", "2")
			});
			writer.RenderBeginTag("div");
			writer.RenderEndTag();
			var html = writer.InnerWriter.ToString();
			Assert.IsTrueWith(html.Contains("a=\"1\""), html);
			Assert.IsTrueWith(html.Contains("b=\"2\""), html);
		}
	}
}
