namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Test event handler
	/// </summary>
	public interface ITestEventHandler {
		/// <summary>
		/// On all test starting
		/// </summary>
		/// <param name="info">Information object</param>
		void OnAllTestStarting(AllTestStartingInfo info);

		/// <summary>
		/// On all test completed
		/// </summary>
		/// <param name="info">Information object</param>
		void OnAllTestCompleted(AllTestCompletedInfo info);

		/// <summary>
		/// On single test starting
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestStarting(TestStartingInfo info);

		/// <summary>
		/// On single test passed
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestPassed(TestPassedInfo info);

		/// <summary>
		/// On single test failed
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestFailed(TestFailedInfo info);

		/// <summary>
		/// On single test skipped
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestSkipped(TestSkippedInfo info);

		/// <summary>
		/// On error message
		/// Create or dispose test instance failed will trigger this event
		/// </summary>
		/// <param name="info">Information object</param>
		void OnErrorMessage(ErrorMessageInfo info);

		/// <summary>
		/// On debug message
		/// Call testRunner.WriteDebugMessage will trigger this event
		/// </summary>
		/// <param name="info">Information object</param>
		void OnDebugMessage(DebugMessageInfo info);
	}
}
