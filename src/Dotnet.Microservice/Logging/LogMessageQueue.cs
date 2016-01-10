using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Dotnet.Microservice.Logging
{
    /*
    *   Class to handle logging of messages in a background thread to keep logging out of the request processing path
    */
    internal class LogMessageQueue
    {
        private static readonly BlockingCollection<KeyValuePair<string, LogLevel>> MQueue = new BlockingCollection<KeyValuePair<string, LogLevel>>();

        static LogMessageQueue()
        {
            var thread = new Thread(
              () =>
              {
                  while (true)
                  {
                      // Pop a message off the queue
                      KeyValuePair<string, LogLevel> messagePair = MQueue.Take();
                      
                      // Call the WriteLog method of each provider
                      foreach (IApplicationLogProvider provider in ApplicationLog.Providers)
                      {
                          try
                          {
                              provider.WriteLog(messagePair.Key, messagePair.Value);
                          }
                          catch (Exception e)
                          {
                              Console.WriteLine("Caught "+ e.GetType() +" when calling log provider "+ provider.GetType());
                              Console.WriteLine(e.StackTrace);
                          }
                      }

                  }
              });

            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Add a message to the log queue to be picked by the background thread
        /// </summary>
        /// <param name="message">KeyValuePair containing message string and log level</param>
        internal static void HandleMessage(KeyValuePair<string, LogLevel> message)
        {
            MQueue.Add(message);
        }
    }
}
