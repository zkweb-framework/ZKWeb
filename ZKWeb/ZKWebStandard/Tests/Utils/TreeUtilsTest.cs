using System;
using System.Collections.Generic;
using System.Linq;
using ZKWebStandard.Extensions;
using ZKWebStandard.Utils;
using ZKWebStandard.Testing;

namespace ZKWebStandard.Tests.Utils {
	[Tests]
	class TreeUtilsTest {
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

		public void CreateTree() {
			var tree = TestData.GetTestTree();
			Assert.Equals(tree.Value, null); // Root
			var childsLv_1 = tree.Childs.ToList(); // A, B
			Assert.Equals(childsLv_1.Count, 2);
			Assert.Equals(childsLv_1[0].Value.Name, "A");
			Assert.Equals(childsLv_1[1].Value.Name, "B");
			var childsLv_2_A = childsLv_1[0].Childs.ToList(); // AA, AB
			Assert.Equals(childsLv_2_A.Count, 2);
			Assert.Equals(childsLv_2_A[0].Value.Name, "AA");
			Assert.IsTrueWith(!childsLv_2_A[0].Childs.Any(), childsLv_2_A[0].Childs);
			Assert.Equals(childsLv_2_A[1].Value.Name, "AB");
			Assert.IsTrueWith(!childsLv_2_A[1].Childs.Any(), childsLv_2_A[1].Childs);
			var childsLv_2_B = childsLv_1[1].Childs.ToList(); // BA, BB
			Assert.Equals(childsLv_2_B.Count, 2);
			Assert.Equals(childsLv_2_B[0].Value.Name, "BA");
			Assert.IsTrueWith(!childsLv_2_B[0].Childs.Any(), childsLv_2_B[0].Childs);
			Assert.Equals(childsLv_2_B[1].Value.Name, "BB");
			var childsLv_3_BB = childsLv_2_B[1].Childs.ToList(); // BBB
			Assert.Equals(childsLv_3_BB.Count, 1);
			Assert.Equals(childsLv_3_BB[0].Value.Name, "BBB");
			Assert.IsTrueWith(!childsLv_3_BB[0].Childs.Any(), childsLv_3_BB[0].Childs);
			// Test add child nodes
			Assert.Throws<ArgumentNullException>(() => childsLv_3_BB[0].AddChildNode(null)); // Add null
			Assert.Throws<ArgumentException>(() => childsLv_3_BB[0].AddChildNode(childsLv_3_BB[0])); // Add self
			Assert.Throws<ArgumentException>(() => childsLv_3_BB[0].AddChildNode(childsLv_2_B[1])); // Add parent
			Assert.Throws<ArgumentException>(() => childsLv_3_BB[0].AddChildNode(childsLv_2_B[0])); // Add node that already has a parent
			// Test remove child nodes
			var node = childsLv_3_BB[0];
			childsLv_2_B[1].RemoveChildNode(node);
			Assert.IsTrueWith(!childsLv_2_B[1].Childs.Any(), childsLv_2_B[1].Childs);
			Assert.Equals(node.Parent, null);
			childsLv_2_B[1].RemoveChildNode(node);
		}

		public void Transform() {
			var tree = TreeUtils.Transform(TestData.GetTestTree(), d => d?.Name);
			Assert.Equals(tree.Value, null); // Root
			var childsLv_1 = tree.Childs.ToList(); // A, B
			Assert.Equals(childsLv_1.Count, 2);
			Assert.Equals(childsLv_1[0].Value, "A");
			Assert.Equals(childsLv_1[1].Value, "B");
			var childsLv_2_A = childsLv_1[0].Childs.ToList(); // AA, AB
			Assert.Equals(childsLv_2_A.Count, 2);
			Assert.Equals(childsLv_2_A[0].Value, "AA");
			Assert.IsTrueWith(!childsLv_2_A[0].Childs.Any(), childsLv_2_A[0].Childs);
			Assert.Equals(childsLv_2_A[1].Value, "AB");
			Assert.IsTrueWith(!childsLv_2_A[1].Childs.Any(), childsLv_2_A[1].Childs);
			var childsLv_2_B = childsLv_1[1].Childs.ToList(); // BA, BB
			Assert.Equals(childsLv_2_B.Count, 2);
			Assert.Equals(childsLv_2_B[0].Value, "BA");
			Assert.IsTrueWith(!childsLv_2_B[0].Childs.Any(), childsLv_2_B[0].Childs);
			Assert.Equals(childsLv_2_B[1].Value, "BB");
			var childsLv_3_BB = childsLv_2_B[1].Childs.ToList(); // BBB
			Assert.Equals(childsLv_3_BB.Count, 1);
			Assert.Equals(childsLv_3_BB[0].Value, "BBB");
			Assert.IsTrueWith(!childsLv_3_BB[0].Childs.Any(), childsLv_3_BB[0].Childs);
		}
	}
}
