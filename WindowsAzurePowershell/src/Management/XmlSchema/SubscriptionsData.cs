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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Management.XmlSchema
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
        Namespace = "urn:Microsoft.WindowsAzure.Management:WaPSCmdlets")]
    [System.Xml.Serialization.XmlRootAttribute(
        Namespace = "urn:Microsoft.WindowsAzure.Management:WaPSCmdlets", IsNullable = false)]
    public partial class Subscriptions
    {

        private SubscriptionsSubscription[] subscriptionField;

        private decimal versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Subscription")]
        public SubscriptionsSubscription[] Subscription
        {
            get { return this.subscriptionField; }
            set { this.subscriptionField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal version
        {
            get { return this.versionField; }
            set { this.versionField = value; }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true,
        Namespace = "urn:Microsoft.WindowsAzure.Management:WaPSCmdlets")]
    public partial class SubscriptionsSubscription
    {

        private string subscriptionIdField;

        private string thumbprintField;

        private string serviceEndpointField;

        private string sQLAzureServiceEndpointField;

        private string currentStorageAccountField;

        private string nameField;

        /// <remarks/>
        public string SubscriptionId
        {
            get { return this.subscriptionIdField; }
            set { this.subscriptionIdField = value; }
        }

        /// <remarks/>
        public string Thumbprint
        {
            get { return this.thumbprintField; }
            set { this.thumbprintField = value; }
        }

        /// <remarks/>
        public string ServiceEndpoint
        {
            get { return this.serviceEndpointField; }
            set { this.serviceEndpointField = value; }
        }

        /// <remarks/>
        public string SQLAzureServiceEndpoint
        {
            get { return this.sQLAzureServiceEndpointField; }
            set { this.sQLAzureServiceEndpointField = value; }
        }

        /// <remarks/>
        public string CurrentStorageAccount
        {
            get { return this.currentStorageAccountField; }
            set { this.currentStorageAccountField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }
    }
}