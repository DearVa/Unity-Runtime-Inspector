using System;
using UnityEngine;

namespace RuntimeInspector {
	public class Log {
		public LogType logType;
		public string tag;
		public object message;
		public UnityEngine.Object context;

		public Log(LogType logType, object message) {
			this.logType = logType;
			this.message = message;
		}

		public Log(LogType logType, string tag, object message) {
			this.logType = logType;
			this.tag = tag;
			this.message = message;
		}

		public Log(LogType logType, object message, UnityEngine.Object context) {
			this.logType = logType;
			this.message = message;
			this.context = context;
		}

		public Log(LogType logType, string tag, object message, UnityEngine.Object context) {
			this.logType = logType;
			this.tag = tag;
			this.message = message;
			this.context = context;
		}
	}

	public class Logger : ILogger {
		public static Action<Log> Logging = null; 

		public Logger(ILogHandler logHandler) {
			this.logHandler = logHandler;
			logEnabled = true;
			filterLogType = LogType.Log;
		}

		public ILogHandler logHandler { get; set; }

		public bool logEnabled { get; set; }

		public LogType filterLogType { get; set; }

		public bool IsLogTypeAllowed(LogType logType) {
			if (this.logEnabled) {
				if (logType == LogType.Exception) {
					return true;
				}
				if (this.filterLogType != LogType.Exception) {
					return logType <= this.filterLogType;
				}
			}
			return false;
		}

		private static string GetString(object message) {
			return (message == null) ? "Null" : message.ToString();
		}

		public void Log(LogType logType, object message) {
			Logging?.Invoke(new Log(logType, message));
			if (IsLogTypeAllowed(logType)) {
				logHandler.LogFormat(logType, null, "{0}", new object[]
				{
					GetString(message)
				});
			}
		}

		public void Log(LogType logType, object message, UnityEngine.Object context) {
			Logging?.Invoke(new Log(logType, message, context));
			if (IsLogTypeAllowed(logType)) {
				logHandler.LogFormat(logType, context, "{0}", new object[]
				{
					GetString(message)
				});
			}
		}

		public void Log(LogType logType, string tag, object message) {
			Logging?.Invoke(new Log(logType, tag, message));
			if (IsLogTypeAllowed(logType)) {
				logHandler.LogFormat(logType, null, "{0}: {1}", new object[]
				{
					tag,
					GetString(message)
				});
			}
		}

		public void Log(LogType logType, string tag, object message, UnityEngine.Object context) {
			Logging?.Invoke(new Log(logType, tag, message, context));
			if (IsLogTypeAllowed(logType)) {
				logHandler.LogFormat(logType, context, "{0}: {1}", new object[]
				{
					tag,
					GetString(message)
				});
			}
		}

		public void Log(object message) {
			Logging?.Invoke(new Log(LogType.Log, message));
			if (IsLogTypeAllowed(LogType.Log)) {
				logHandler.LogFormat(LogType.Log, null, "{0}", new object[]
				{
					GetString(message)
				});
			}
		}

		public void Log(string tag, object message) {
			Logging?.Invoke(new Log(LogType.Log, tag, message));
			if (IsLogTypeAllowed(LogType.Log)) {
				logHandler.LogFormat(LogType.Log, null, "{0}: {1}", new object[]
				{
					tag,
					GetString(message)
				});
			}
		}

		public void Log(string tag, object message, UnityEngine.Object context) {
			Logging?.Invoke(new Log(LogType.Log, message, context));
			if (IsLogTypeAllowed(LogType.Log)) {
				logHandler.LogFormat(LogType.Log, context, "{0}: {1}", new object[]
				{
					tag,
					GetString(message)
				});
			}
		}

		public void LogWarning(string tag, object message) {
			Logging?.Invoke(new Log(LogType.Warning, tag, message));
			if (IsLogTypeAllowed(LogType.Warning)) {
				logHandler.LogFormat(LogType.Warning, null, "{0}: {1}", new object[]
				{
					tag,
					GetString(message)
				});
			}
		}

		public void LogWarning(string tag, object message, UnityEngine.Object context) {
			Logging?.Invoke(new Log(LogType.Warning, message, context));
			if (IsLogTypeAllowed(LogType.Warning)) {
				logHandler.LogFormat(LogType.Warning, context, "{0}: {1}", new object[]
				{
					tag,
					GetString(message)
				});
			}
		}

		public void LogError(string tag, object message) {
			Logging?.Invoke(new Log(LogType.Error, tag, message));
			if (IsLogTypeAllowed(LogType.Error)) {
				logHandler.LogFormat(LogType.Error, null, "{0}: {1}", new object[]
				{
					tag,
					GetString(message)
				});
			}
		}

		public void LogError(string tag, object message, UnityEngine.Object context) {
			Logging?.Invoke(new Log(LogType.Error, message, context));
			if (IsLogTypeAllowed(LogType.Error)) {
				logHandler.LogFormat(LogType.Error, context, "{0}: {1}", new object[]
				{
					tag,
					GetString(message)
				});
			}
		}

		public void LogFormat(LogType logType, string format, params object[] args) {
			if (IsLogTypeAllowed(logType)) {
				logHandler.LogFormat(logType, null, format, args);
			}
		}

		public void LogException(Exception exception) {
			if (this.logEnabled) {
				logHandler.LogException(exception, null);
			}
		}

		public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args) {
			if (IsLogTypeAllowed(logType)) {
				logHandler.LogFormat(logType, context, format, args);
			}
		}

		public void LogException(Exception exception, UnityEngine.Object context) {
			if (this.logEnabled) {
				logHandler.LogException(exception, context);
			}
		}
	}
}
