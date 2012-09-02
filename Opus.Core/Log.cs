﻿// <copyright file="Log.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus logging.</summary>
// <author>Mark Final</author>
namespace Opus.Core
{
    /// <summary>
    /// Opus logging static class.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Log a message, either to the debugger output window or the console.
        /// </summary>
        /// <param name="messageValue">Message to output.</param>
        /// <param name="isError">True if an error message, false if standard output.</param>
        private static void Message(string messageValue, bool isError)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(messageValue);
            }
            else
            {
                if (isError)
                {
                    System.Console.Error.WriteLine(messageValue);
                }
                else
                {
                    System.Console.Out.WriteLine(messageValue);
                }
            }
        }

        /// <summary>
        /// Log a message, either to the debugger output window or the console.
        /// </summary>
        /// <param name="level">Verbose level to use</param>
        /// <param name="format">Format of message to output.</param>
        /// <param name="args">Array of objects used in the formatting.</param>
        public static void Message(EVerboseLevel level, string format, params object[] args)
        {
            if (State.VerbosityLevel >= level)
            {
                System.Text.StringBuilder formattedMessage = new System.Text.StringBuilder();
                formattedMessage.AppendFormat(format, args);
                Message(formattedMessage.ToString(), false);
            }
        }

        public static void MessageAll(string format, params object[] args)
        {
            System.Text.StringBuilder formattedMessage = new System.Text.StringBuilder();
            formattedMessage.AppendFormat(format, args);
            Message(formattedMessage.ToString(), false);
        }

        public static void Info(string format, params object[] args)
        {
            if (State.VerbosityLevel >= EVerboseLevel.Info)
            {
                System.Text.StringBuilder formattedMessage = new System.Text.StringBuilder();
                formattedMessage.AppendFormat(format, args);
                Message(formattedMessage.ToString(), false);
            }
        }

        public static void Detail(string format, params object[] args)
        {
            if (State.VerbosityLevel >= EVerboseLevel.Detail)
            {
                System.Text.StringBuilder formattedMessage = new System.Text.StringBuilder();
                formattedMessage.AppendFormat(format, args);
                Message(formattedMessage.ToString(), false);
            }
        }

        public static void Full(string format, params object[] args)
        {
            if (EVerboseLevel.Full == State.VerbosityLevel)
            {
                System.Text.StringBuilder formattedMessage = new System.Text.StringBuilder();
                formattedMessage.AppendFormat(format, args);
                Message(formattedMessage.ToString(), false);
            }
        }

        public static void ErrorMessage(string format, params object[] args)
        {
            System.Text.StringBuilder formattedMessage = new System.Text.StringBuilder();
            formattedMessage.AppendFormat("ERROR: " + format, args);
            Message(formattedMessage.ToString(), true);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugMessage(string format, params object[] args)
        {
            if (State.VerbosityLevel > EVerboseLevel.Detail)
            {
                System.Text.StringBuilder formattedMessage = new System.Text.StringBuilder();
                formattedMessage.AppendFormat(format, args);
                Message(formattedMessage.ToString(), false);
            }
        }
    }
}