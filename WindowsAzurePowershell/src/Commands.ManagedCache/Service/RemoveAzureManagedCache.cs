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

namespace Microsoft.Azure.Commands.ManagedCache
{
    using System;
    using System.Management.Automation;

    [Cmdlet(VerbsCommon.Remove, "AzureManagedCache"), OutputType(typeof(bool))]
    public class RemoveAzureManagedCache : ManagedCacheCmdletBase
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set;}

        public SwitchParameter Force { get; set; }

        public override void ExecuteCmdlet()
        {
            ConfirmAction(
               Force.IsPresent,
               string.Format(Properties.Resources.RemoveServiceWarning, Name),
               Properties.Resources.RemoveServiceMessage,
               Name,
               () =>
               {
                   WriteVerbose(Properties.Resources.CacheServiceRemoveStarted);
                   CacheClient.DeleteCacheService(Name);
                   WriteVerbose(string.Format(Properties.Resources.CacheServiceRemoved, Name));
                   WriteObject(true);
               });
        }      
    }
}