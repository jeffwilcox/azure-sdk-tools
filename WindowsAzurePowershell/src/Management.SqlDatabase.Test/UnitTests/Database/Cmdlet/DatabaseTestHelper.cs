﻿// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
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

namespace Microsoft.WindowsAzure.Management.SqlDatabase.Test.UnitTests.Database.Cmdlet
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Services.Common;
    using Microsoft.WindowsAzure.Management.SqlDatabase.Test.UnitTests.MockServer;

    public static class DatabaseTestHelper
    {
        /// <summary>
        /// Helper function to validate headers for GetAccessToken request.
        /// </summary>
        public static void ValidateGetAccessTokenRequest(
            HttpMessage expected, 
            HttpMessage.Request actual)
        {
            Assert.IsTrue(
                actual.RequestUri.AbsoluteUri.EndsWith("GetAccessToken"),
                "Incorrect Uri specified for GetAccessToken");
            Assert.IsTrue(
                actual.Headers.Contains("sqlauthorization"),
                "sqlauthorization header does not exist in the request");
            Assert.AreEqual(
                expected.RequestInfo.Headers["sqlauthorization"],
                actual.Headers["sqlauthorization"],
                "sqlauthorization header does not match");
            Assert.IsNull(
                actual.RequestText,
                "There should be no request text for GetAccessToken");
        }

        /// <summary>
        /// Helper function to validate headers for Service request.
        /// </summary>
        public static void ValidateHeadersForServiceRequest(
            HttpMessage expected,
            HttpMessage.Request actual)
        {
            Assert.IsTrue(
                actual.Headers.Contains(DataServiceConstants.AccessTokenHeader),
                "AccessToken header does not exist in the request");
            Assert.AreEqual(
                expected.RequestInfo.Headers[DataServiceConstants.AccessTokenHeader],
                actual.Headers[DataServiceConstants.AccessTokenHeader],
                "AccessToken header does not match");
            Assert.IsTrue(
                actual.Headers.Contains("x-ms-client-session-id"),
                "session-id header does not exist in the request");
            Assert.IsTrue(
                actual.Headers.Contains("x-ms-client-request-id"),
                "request-id header does not exist in the request");
            Assert.IsTrue(
                actual.Cookies.Contains(DataServiceConstants.AccessCookie),
                "AccessCookie does not exist in the request");
            Assert.AreEqual(
                expected.RequestInfo.Cookies[DataServiceConstants.AccessCookie],
                actual.Cookies[DataServiceConstants.AccessCookie],
                "AccessCookie does not match");
        }

        /// <summary>
        /// Helper function to validate headers for OData request.
        /// </summary>
        public static void ValidateHeadersForODataRequest(
            HttpMessage expected,
            HttpMessage.Request actual)
        {
            DatabaseTestHelper.ValidateHeadersForServiceRequest(expected, actual);
            Assert.IsTrue(
                actual.Headers.Contains("DataServiceVersion"),
                "DataServiceVersion header does not exist in the request");
            Assert.AreEqual(
                expected.RequestInfo.Headers["DataServiceVersion"],
                actual.Headers["DataServiceVersion"],
                "DataServiceVersion header does not match");
        }
    }
}
