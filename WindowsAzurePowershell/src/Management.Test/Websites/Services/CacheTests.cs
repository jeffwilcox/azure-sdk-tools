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

namespace Microsoft.WindowsAzure.Management.Test.Websites.Services
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.WindowsAzure.Management.Test.Utilities.Common;
    using Microsoft.WindowsAzure.Management.Utilities.Common;
    using Microsoft.WindowsAzure.Management.Utilities.Websites.Services;
    using Microsoft.WindowsAzure.Management.Utilities.Websites.Services.WebEntities;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CacheTests : TestBase
    {
        public static string SubscriptionName = "fakename";

        public static string WebSpacesFile;

        public static string SitesFile;

        private FileSystemHelper helper;

        [TestInitialize]
        public void SetupTest()
        {
            helper = new FileSystemHelper(this);
            helper.CreateAzureSdkDirectoryAndImportPublishSettings();

            WebSpacesFile =  Path.Combine(GlobalPathInfo.GlobalSettingsDirectory,
                                                          string.Format("spaces.{0}.json", SubscriptionName));

            SitesFile = Path.Combine(GlobalPathInfo.GlobalSettingsDirectory,
                                                          string.Format("sites.{0}.json", SubscriptionName));
            
            if (File.Exists(WebSpacesFile))
            {
                File.Delete(WebSpacesFile);
            }

            if (File.Exists(SitesFile))
            {
                File.Delete(SitesFile);
            }
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (File.Exists(WebSpacesFile))
            {
                File.Delete(WebSpacesFile);
            }

            if (File.Exists(SitesFile))
            {
                File.Delete(SitesFile);
            }

            helper.Dispose();
        }

        [TestMethod]
        public void AddSiteTest()
        {
            Site site = new Site { Name = "newsite" };
            // Add without any cache from before
            Cache.AddSite(SubscriptionName, site);

            Sites getSites = Cache.GetSites(SubscriptionName);
            Assert.IsNotNull(getSites.Find(ws => ws.Name.Equals("newsite")));
        }

        [TestMethod]
        public void RemoveSiteTest()
        {
            Site site = new Site { Name = "newsite" };
            // Add without any cache from before
            Cache.AddSite(SubscriptionName, site);

            Sites getSites = Cache.GetSites(SubscriptionName);
            Assert.IsNotNull(getSites.Find(ws => ws.Name.Equals("newsite")));

            // Now remove it
            Cache.RemoveSite(SubscriptionName, site);
            getSites = Cache.GetSites(SubscriptionName);
            Assert.IsNull(getSites.Find(ws => ws.Name.Equals("newsite")));
        }

        [TestMethod]
        public void GetSetSitesTest()
        {
            Assert.IsNull(Cache.GetSites(SubscriptionName));

            Sites sites = new Sites(new List<Site> { new Site { Name = "site1" }, new Site { Name = "site2" }});
            Cache.SaveSites(SubscriptionName, sites);

            Sites getSites = Cache.GetSites(SubscriptionName);
            Assert.IsNotNull(getSites.Find(s => s.Name.Equals("site1")));
            Assert.IsNotNull(getSites.Find(s => s.Name.Equals("site2")));
        }
    }
}
