﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Commands.ServiceBus.Models;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.WindowsAzure.Commands.Common.CustomAttributes;
using System.Management.Automation;
using System.Xml;

namespace Microsoft.Azure.Commands.ServiceBus.Commands.Rule
{
    /// <summary>
    /// 'New-AzServiceBusRule' Cmdlet creates a new Rule
    /// </summary>
    [GenericBreakingChange(message: BreakingChangeNotification + "\n- Output type of the cmdlet would change to 'Microsoft.Azure.PowerShell.Cmdlets.ServiceBus.Models.Api202201Preview.IRule'", deprecateByVersion: DeprecateByVersion, changeInEfectByDate: ChangeInEffectByDate)]
    [Cmdlet("New", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "ServiceBusRule", DefaultParameterSetName = RuleResourceParameterSet, SupportsShouldProcess = true), OutputType(typeof(PSRulesAttributes))]
    public class NewAzureRmServiceBusRule : AzureServiceBusCmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "The name of the resource group")]
        [ResourceGroupCompleter]
        [Alias("ResourceGroup")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 1, HelpMessage = "Namespace Name")]
        [Alias(AliasNamespaceName)]
        [ValidateNotNullOrEmpty]
        public string Namespace { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 2, HelpMessage = "Topic Name")]
        [Alias(AliasTopicName)]
        [ValidateNotNullOrEmpty]
        public string Topic { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 3, HelpMessage = "Subscription Name")]
        [Alias(AliasSubscriptionName)]
        [ValidateNotNullOrEmpty]
        public string Subscription { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 4, HelpMessage = "Rule Name")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, Position = 5, HelpMessage = "Sql Filter Expression")]
        [ValidateNotNullOrEmpty]
        public string SqlExpression { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = RuleResourceActionParameterSet, ValueFromPipelineByPropertyName = true, HelpMessage = "Action SqlFilter Expression")]
        [ValidateNotNullOrEmpty]
        public string ActionSqlExpression { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = RuleResourceActionParameterSet, HelpMessage = "Action Requires Preprocessing")]
        public SwitchParameter RequiresPreprocessing { get; set; }

        public override void ExecuteCmdlet()
        {
            PSRulesAttributes ruleAttributes = new PSRulesAttributes();

            if (!string.IsNullOrEmpty(SqlExpression))
                ruleAttributes.SqlFilter.SqlExpression = SqlExpression;

            if (!string.IsNullOrEmpty(ActionSqlExpression))
                ruleAttributes.Action.SqlExpression = ActionSqlExpression;

            if (RequiresPreprocessing.IsPresent)
                ruleAttributes.Action.RequiresPreprocessing = true;

            if (ShouldProcess(target: Name, action: string.Format(Resources.CreateRule, Name, Topic,Namespace)))
            {
                try
                {
                    WriteObject(Client.CreateUpdateRules(ResourceGroupName, Namespace, Topic, Subscription, Name, ruleAttributes));
                }
                catch (ErrorResponseException ex)
                {
                    WriteError(ServiceBusClient.WriteErrorforBadrequest(ex));
                }
            }
        }
    }
}
