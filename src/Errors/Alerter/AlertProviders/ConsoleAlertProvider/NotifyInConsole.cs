﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus.Management.Errors.Alerter.Messages;
using NServiceBus.Management.Errrors.Alerter.DomainEvents;
using NServiceBus.Management.Errors.Messages;
using NServiceBus.Management.Errors.Alerter.Config;
using System.Configuration;

namespace NServiceBus.Management.Errors.Alerter.AlertProviders.ConsoleAlertProvider
{
    public class NotifyInConsole : IHandleDomainEvents<TimeElapsedForSummaryAlert>,
        IHandleDomainEvents<MaxThresholdLimitReached>
    {
        public void Handle(TimeElapsedForSummaryAlert args)
        {
            // Check to see if this provider is configured
            AlertRulesSection alertSection = (AlertRulesSection)ConfigurationManager.GetSection("AlertRulesConfig");
            AlertRuleCollection ruleCollection = alertSection.RuleCollection;
            AlertRule rule = ruleCollection[args.RuleId];
            AlertProviderCollection alertProviders = rule.AlertProviders;

            foreach (Provider provider in alertProviders)
            {
                if (provider.Type == "Console")
                {
                    Console.WriteLine("ALERT -- (Total:{0})", args.ErrorMessages.Length);
                    foreach (IErrorMessageDetails errorMessage in args.ErrorMessages)
                    {
                        Console.WriteLine("Message failed in Queue: {0}", errorMessage.ProcessingFailedAddress);
                        Console.WriteLine("Message Id: {0}", errorMessage.FailedMessageId);
                        Console.WriteLine("Message received at: {0}", errorMessage.ErrorReceivedTime);
                        Console.WriteLine("Xml contents of message: {0}", errorMessage.Body);
                    }
                }
            }
        }

        public void Handle(MaxThresholdLimitReached args)
        {
            // Check to see if this provider is configured
            AlertRulesSection alertSection = (AlertRulesSection)ConfigurationManager.GetSection("AlertRulesConfig");
            AlertRuleCollection ruleCollection = alertSection.RuleCollection;
            AlertRule rule = ruleCollection[args.RuleId];
            AlertProviderCollection alertProviders = rule.AlertProviders;

            foreach (Provider provider in alertProviders)
            {
                if (provider.Type == "Console")
                {
                    Console.WriteLine("ALERT -- TOO MANY MESSAGES IN Q (Total:{0})", args.TotalErrorsInErrorQueue);
                }
            }
        }
    }
}
