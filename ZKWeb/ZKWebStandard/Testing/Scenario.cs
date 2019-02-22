using System;

namespace ZKWebStandard.Testing {
	/// <summary>
	/// Scenario base class used for BDD<br/>
	/// 用于BDD的场景基础类<br/>
	/// </summary>
	public abstract class ScenarioBase {
		/// <summary>
		/// The pre-conditions of scenario, a scenario should have single given<br/>
		/// 定义场景的前提条件，一个场景应该只有一个Given<br/>
		/// </summary>
		protected void Given(string description, Action action) {
			try {
				action();
			} catch (AssertException ex) {
				throw new ScenarioException(string.Format(
					"Given [{0}] error: (Don't assert in given)\r\n  {1}",
					description, ex.Message));
			}  catch (Exception ex) {
				throw new ScenarioException(string.Format(
					"Given [{0}] error:\r\n  {1}",
					description, ex));
			}
		}

		/// <summary>
		/// The target action of scenario, a scenario should have single when<br/>
		/// 定义场景的目标操作，一个场景应该只有一个When<br/>
		/// </summary>
		protected void When(string description, Action action) {
			try {
				action();
			} catch (AssertException ex) {
				throw new ScenarioException(string.Format(
					"When [{0}] error: (Don't assert in when)\r\n  {1}",
					description, ex.Message));
			} catch (Exception ex) {
				throw new ScenarioException(string.Format(
					"When [{0}] error:\r\n  {1}",
					description, ex.ToString()));
			}
		}

		/// <summary>
		/// The post-conditions of scenario, a scenario should have one or more when<br/>
		/// 定义场景的后置条件，一个场景应该有一个或多个Then<br/>
		/// </summary>
		protected void Then(string description, Action action) {
			try {
				action();
			} catch (AssertException ex) {
				throw new ScenarioException(string.Format(
					"Then [{0}] error:\r\n  {1}",
					description, ex.Message));
			} catch (Exception ex) {
				throw new ScenarioException(string.Format(
					"Then [{0}] error:\r\n  {1}",
					description, ex.ToString()));
			}
		}
	}
}