using System.Collections.Generic;
using ZKWeb.Templating.DynamicContents;
using ZKWebStandard.Testing;

namespace ZKWeb.Tests.Templating.DynamicContents {
	[Tests]
	class TemplateAreaExtensionsTest {
		public void Add() {
			var widgets = new List<TemplateWidget>();
			widgets.Add("plugin_a.widgets/a");
			widgets.Add("plugin_a.widgets/b");
			widgets.Add("plugin_a.widgets/c", new Dictionary<string, object>() { { "a", 1 } });
			widgets.AddBefore("plugin_a.widgets/a", "plugin_a.widgets/x");
			widgets.AddAfter("plugin_a.widgets/a", "plugin_a.widgets/ax");
			Assert.Equals(widgets.Count, 5);
			Assert.Equals(widgets[0].Path, "plugin_a.widgets/x");
			Assert.Equals(widgets[0].Args, null);
			Assert.Equals(widgets[1].Path, "plugin_a.widgets/a");
			Assert.Equals(widgets[1].Args, null);
			Assert.Equals(widgets[2].Path, "plugin_a.widgets/ax");
			Assert.Equals(widgets[2].Args, null);
			Assert.Equals(widgets[3].Path, "plugin_a.widgets/b");
			Assert.Equals(widgets[3].Args, null);
			Assert.Equals(widgets[4].Path, "plugin_a.widgets/c");
			Assert.Equals(((Dictionary<string, object>)widgets[4].Args)["a"], 1);
		}
	}
}
