namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Test event handler<br/>
	/// 测试事件处理器<br/>
	/// </summary>
	public interface ITestEventHandler {
		/// <summary>
		/// On all test starting<br/>
		/// 所有测试开始后的处理<br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnAllTestStarting(AllTestStartingInfo info);

		/// <summary>
		/// On all test completed<br/>
		/// 所有测试完成后的处理<br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnAllTestCompleted(AllTestCompletedInfo info);

		/// <summary>
		/// On single test starting<br/>
		/// 单个测试开始时的处理<br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestStarting(TestStartingInfo info);

		/// <summary>
		/// On single test passed<br/>
		/// 单个测试通过时的处理<br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestPassed(TestPassedInfo info);

		/// <summary>
		/// On single test failed<br/>
		/// 单个测试失败时的处理<br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestFailed(TestFailedInfo info);

		/// <summary>
		/// On single test skipped<br/>
		/// 单个测试跳过时的处理<br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestSkipped(TestSkippedInfo info);

		/// <summary>
		/// On error message<br/>
		/// Create or dispose test instance failed will trigger this event<br/>
		/// 错误消息的处理<br/>
		/// 创建或销毁测试实例失败时会触发这个事件<br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnErrorMessage(ErrorMessageInfo info);

		/// <summary>
		/// On debug message<br/>
		/// Call testRunner.WriteDebugMessage will trigger this event<br/>
		/// 除错消息的处理<br/>
		/// 调用testRunner.WriteDebugMessage会触发这个事件<br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnDebugMessage(DebugMessageInfo info);
	}
}
