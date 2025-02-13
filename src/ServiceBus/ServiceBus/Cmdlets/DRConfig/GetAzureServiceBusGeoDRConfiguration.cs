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

using Microsoft.Azure.Commands.ServiceBus;
using Microsoft.Azure.Commands.ServiceBus.Models;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.Internal.Resources.Utilities.Models;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.WindowsAzure.Commands.Common.CustomAttributes;

namespace Microsoft.Azure.Commands.ServiceBus.Commands.GeoDR
{
    /// <summary>
    /// 'Get-AzServicebusGeoDRConfigurations' CmdletRetrieves Alias(Disaster Recovery configuration) for primary or secondary namespace    
    /// </summary>
    [GenericBreakingChange(message: BreakingChangeNotification + "\n- Output type of the cmdlet would change to 'Microsoft.Azure.PowerShell.Cmdlets.ServiceBus.Models.Api202201Preview.IArmDisasterRecovery'", deprecateByVersion: DeprecateByVersion, changeInEfectByDate: ChangeInEffectByDate)]
    [Cmdlet("Get", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "ServiceBusGeoDRConfiguration", DefaultParameterSetName = GeoDRParameterSet), OutputType(typeof(PSServiceBusDRConfigurationAttributes))]
    public class GetServiceBusGeoDRConfiguration : AzureServiceBusCmdletBase
    {
        [Parameter(Mandatory = true, ParameterSetName = GeoDRParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "Resource Group Name")]
        [ValidateNotNullOrEmpty]
        [ResourceGroupCompleter]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = GeoDRParameterSet, ValueFromPipelineByPropertyName = true, Position = 1, HelpMessage = "Namespace Name")]
        [ValidateNotNullOrEmpty]
        public string Namespace { get; set; }

        [CmdletParameterBreakingChange("InputObject", OldParamaterType = typeof(PSNamespaceAttributes), NewParameterTypeName = "Microsoft.Azure.PowerShell.Cmdlets.ServiceBus.Models.Api202201Preview.IArmDisasterRecovery", ChangeDescription = "InputObject parameter set is changing. Please refer the migration guide for examples.")]
        [Parameter(Mandatory = true, ParameterSetName = NamespaceInputObjectParameterSet, ValueFromPipeline = true, Position = 0, HelpMessage = "Namespace Object")]
        [ValidateNotNullOrEmpty]
        public PSNamespaceAttributes InputObject { get; set; }

        [CmdletParameterBreakingChange("ResourceId", ReplaceMentCmdletParameterName = "InputObject")]
        [Parameter(Mandatory = true, ParameterSetName = ResourceIdParameterSet, ValueFromPipelineByPropertyName = true, Position = 0, HelpMessage = "Namespace Resource Id")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        [CmdletParameterBreakingChange("Name", ChangeDescription = "'Name' would be removed from " + NamespaceInputObjectParameterSet)]
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "DR Configuration Name")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                if (ParameterSetName == NamespaceInputObjectParameterSet)
                {
                    LocalResourceIdentifier getParamGeoDR = new LocalResourceIdentifier(InputObject.Id);

                    if (getParamGeoDR.ResourceGroupName != null && getParamGeoDR.ResourceName != null)
                    {
                        if (!string.IsNullOrEmpty(Name))
                        {
                            PSServiceBusDRConfigurationAttributes drConfiguration = Client.GetServiceBusDRConfiguration(getParamGeoDR.ResourceGroupName, getParamGeoDR.ResourceName, Name);
                            WriteObject(drConfiguration);
                        }
                        else
                        {
                            IEnumerable<PSServiceBusDRConfigurationAttributes> drConfigurationList = Client.ListAllServiceBusDRConfiguration(getParamGeoDR.ResourceGroupName, getParamGeoDR.ResourceName);
                            WriteObject(drConfigurationList.ToList(), true);
                        }
                    }
                }

                if (ParameterSetName == ResourceIdParameterSet)
                {
                    LocalResourceIdentifier getParamGeoDR = new LocalResourceIdentifier(ResourceId);

                    if (getParamGeoDR.ResourceGroupName != null && getParamGeoDR.ResourceName != null)
                    {
                        if (!string.IsNullOrEmpty(Name))
                        {
                            PSServiceBusDRConfigurationAttributes drConfiguration = Client.GetServiceBusDRConfiguration(getParamGeoDR.ResourceGroupName, getParamGeoDR.ResourceName, Name);
                            WriteObject(drConfiguration);
                        }
                        else
                        {
                            IEnumerable<PSServiceBusDRConfigurationAttributes> drConfigurationList = Client.ListAllServiceBusDRConfiguration(getParamGeoDR.ResourceGroupName, getParamGeoDR.ResourceName);
                            WriteObject(drConfigurationList.ToList(), true);
                        }
                    }
                }

                if (ParameterSetName == GeoDRParameterSet)
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        // Get a DRConfiguration
                        PSServiceBusDRConfigurationAttributes drConfiguration = Client.GetServiceBusDRConfiguration(ResourceGroupName, Namespace, Name);
                        WriteObject(drConfiguration);
                    }
                    else
                    {
                        // Get all DRConfigurations
                        IEnumerable<PSServiceBusDRConfigurationAttributes> drConfigurationList = Client.ListAllServiceBusDRConfiguration(ResourceGroupName, Namespace);
                        WriteObject(drConfigurationList.ToList(), true);
                    }
                }
            }
            catch (ErrorResponseException ex)
            {
                WriteError(ServiceBusClient.WriteErrorforBadrequest(ex));
            }
        }
    }
}
