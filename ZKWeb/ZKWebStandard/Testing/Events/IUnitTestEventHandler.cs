namespace ZKWebStandard.Testing.Events {
	/// <summary>
	/// Test event handler<br/>
	/// <br/>
	/// </summary>
	public interface ITestEventHandler {
		/// <summary>
		/// On all test starting<br/>
		/// <br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnAllTestStarting(AllTestStartingInfo info);

		/// <summary>
		/// On all test completed<br/>
		/// <br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnAllTestCompleted(AllTestCompletedInfo info);

		/// <summary>
		/// On single test starting<br/>
		/// <br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestStarting(TestStartingInfo info);

		/// <summary>
		/// On single test passed<br/>
		/// <br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestPassed(TestPassedInfo info);

		/// <summary>
		/// On single test failed<br/>
		/// <br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestFailed(TestFailedInfo info);

		/// <summary>
		/// On single test skipped<br/>
		/// <br/>
		/// </summary>
		/// <param name="info">Information object</param>
		void OnTestSkipped(TestSkippedInfo info);

		/// <summary>
		/// On error message<br/>
		/// Create or dispose test instance failed will trigger this event<br/>
		/// <br/>
		/// <br/>
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
