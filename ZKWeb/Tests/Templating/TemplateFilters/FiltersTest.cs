using DotLiquid;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZKWeb.Localize;
using ZKWeb.Localize.Interfaces;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Utils.UnitTest;

namespace ZKWeb.Tests.Templating.TemplateFilters {
	[UnitTest]
	class FiltersTest {
		public void Trans() {
			using (Application.OverrideIoc()) {
				Application.Ioc.Unregister<TranslateManager>();
				Application.Ioc.RegisterMany<TranslateManager>(ReuseType.Singleton);
				var translateProviderMock = new Mock<ITranslateProvider>();
				translateProviderMock.Setup(t => t.CanTranslate(It.IsAny<string>())).Returns(true);
				translateProviderMock.Setup(t => t.Translate(It.IsAny<string>()))
					.Returns((string original) => {
						return original == "Original" ? "Translated" : null;
					});
				Application.Ioc.Unregister<ITranslateProvider>();
				Application.Ioc.RegisterInstance(translateProviderMock.Object);
				Assert.Equals(Template.Parse("{{ 'Original' | trans }}").Render(), "Translated");
				Assert.Equals(Template.Parse("{{ 'NotExist' | trans }}").Render(), "NotExist");
			}
		}

		public void Format() {
			Assert.Equals(Template.Parse(
				"{{ 'name is [0], age is [1]' | format: name, age }}")
					.Render(Hash.FromAnonymousObject(new { name = "John", age = 50 })),
				"name is John, age is 50");
		}
	}
}
