using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Extensions {
	[Tests]
	class ITreeNodeExtensionsTest {
		class TestData {
			public long Id { get; set; }
			public long ParentId { get; set; }
			public string Name { get; set; }

			public TestData(long id, long parentId, string name) {
				this.Id = id;
				this.ParentId = parentId;
				this.Name = name;
			}

			public static ITreeNode<TestData> GetTestTree() {
				var elements = new List<TestData>() {
					new TestData(1, 0, "A"),
					new TestData(2, 1, "AA"),
					new TestData(3, 1, "AB"),
					new TestData(4, 0, "B"),
					new TestData(5, 4, "BA"),
					new TestData(6, 4, "BB"),
					new TestData(7, 6, "BBB"),
				};
				var elementsMapping = elements.ToDictionary(e => e.Id, e => e);
				var tree = TreeUtils.CreateTree(elements,
					e => e, e => elementsMapping.GetOrDefault(e.ParentId));
				return tree;
			}
		}


		public void GetParents() {
			var tree = TestData.GetTestTree();
			var nodeBBB = tree.EnumerateAllNodes().First(n => n.Value?.Name == "BBB");
			var nodeBBBParents = nodeBBB.GetParents().ToList();
			Assert.Equals(nodeBBBParents.Count, 3);
			Assert.Equals(nodeBBBParents[0].Value.Name, "BB");
			Assert.Equals(nodeBBBParents[1].Value.Name, "B");
			Assert.Equals(nodeBBBParents[2], tree);
		}

		public void VisitAllNodes() {
			var tree = TestData.GetTestTree();
			var visitedNames = new List<string>();
			tree.VisitAllNodes(n => visitedNames.Add(n.Value?.Name));
			Assert.Equals(visitedNames.Count, 8);
			Assert.Equals(visitedNames[0], null); // Root
			Assert.Equals(visitedNames[1], "A");
			Assert.Equals(visitedNames[2], "AA");
			Assert.Equals(visitedNames[3], "AB");
			Assert.Equals(visitedNames[4], "B");
			Assert.Equals(visitedNames[5], "BA");
			Assert.Equals(visitedNames[6], "BB");
			Assert.Equals(visitedNames[7], "BBB");
		}

		public void EnumerateAllNodes() {
			var tree = TestData.GetTestTree();
			var allNames = tree.EnumerateAllNodes().Select(n => n.Value?.Name).ToList();
			Assert.Equals(allNames.Count, 8);
			Assert.Equals(allNames[0], null); // Root
			Assert.Equals(allNames[1], "A");
			Assert.Equals(allNames[2], "AA");
			Assert.Equals(allNames[3], "AB");
			Assert.Equals(allNames[4], "B");
			Assert.Equals(allNames[5], "BA");
			Assert.Equals(allNames[6], "BB");
			Assert.Equals(allNames[7], "BBB");
		}
	}
}
