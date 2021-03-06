﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus.Management.Errors.Messages;
using System.Messaging;
//using System.Transactions;
using System.Configuration;
using NServiceBus.Utils;
using NServiceBus.Tools.Management.Errors.ReturnToSourceQueue;

namespace NServiceBus.Management.Errors.Monitor.MessageHandlers
{
    class ReprocessErrorHandler : IHandleMessages<ReprocessErrorMessage>
    {
        private ErrorManager errorManager = new ErrorManager();
        public IPersistErrorMessages ErrorPersister { get; set; }
        public IBus Bus { get; set; }
        
        public void Handle(ReprocessErrorMessage messageToReprocess)
        {
            errorManager.InputQueue = new Address(string.Format("{0}.Storage", ConfigurationManager.AppSettings["ErrorQueueToMonitor"]), Environment.MachineName);

            // Reprocess the error message.
            errorManager.ReturnMessageToSourceQueue(messageToReprocess.OriginalMessageId);
            
            // Remove message from the persistent store.
            ErrorPersister.DeleteErrorMessage(messageToReprocess.OriginalMessageId);

            // Publish event
            Bus.Publish<ErrorMessageReprocessed>(m => { m.MessageId = messageToReprocess.OriginalMessageId; m.ErrorReprocessedTime = DateTime.Now; });
        }
    }
}
