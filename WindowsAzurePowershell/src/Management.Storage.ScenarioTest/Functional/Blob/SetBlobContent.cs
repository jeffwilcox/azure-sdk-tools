// ----------------------------------------------------------------------------------
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLITest.Common;
using CLITest.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using MS.Test.Common.MsTestLib;
using StorageTestLib;
using Storage = Microsoft.WindowsAzure.Storage.Blob;

namespace CLITest.Functional.Blob
{
    /// <summary>
    /// functional tests for Set-ContainerAcl
    /// </summary>
    [TestClass]
    class SetBlobContent : TestBase
    {
        private static string uploadDirRoot;
        private static List<string> files = new List<string>();

        //TODO upload a already opened read/write file
        [ClassInitialize()]
        public static void ClassInit(TestContext testContext)
        {
            TestBase.TestClassInitialize(testContext);
            uploadDirRoot = Test.Data.Get("UploadDir");
            SetupUploadDir();
        }

        [ClassCleanup()]
        public static void SetBlobContentClassCleanup()
        {
            TestBase.TestClassCleanup();
        }

        /// <summary>
        /// create upload dir and temp files
        /// </summary>
        private static void SetupUploadDir()
        {
            Test.Verbose("Create Upload dir {0}", uploadDirRoot);

            if (!Directory.Exists(uploadDirRoot))
            {
                Directory.CreateDirectory(uploadDirRoot);
            }

            FileUtil.CleanDirectory(uploadDirRoot);

            int minDirDepth = 1, maxDirDepth = 3;
            int dirDepth = random.Next(minDirDepth, maxDirDepth);
            Test.Info("Generate Temp files for Set-AzureStorageBlobContent");
            files = FileUtil.GenerateTempFiles(uploadDirRoot, dirDepth);
            files.Sort();
        }

        /// <summary>
        /// set azure blob content by mutilple files
        /// 8.14	Set-AzureStorageBlobContent
        ///     3.	Upload a list of new blob files
        /// </summary>
        [TestMethod()]
        [TestCategory(Tag.Function)]
        [TestCategory(PsTag.Blob)]
        [TestCategory(PsTag.SetBlobContent)]
        public void SetBlobContentByMultipleFiles()
        {
            string containerName = Utility.GenNameString("container");
            CloudBlobContainer container = blobUtil.CreateContainer(containerName);

            try
            {
                List<IListBlobItem> blobLists = container.ListBlobs(string.Empty, true, BlobListingDetails.All).ToList();
                Test.Assert(blobLists.Count == 0, string.Format("container {0} should contain {1} blobs, and actually it contain {2} blobs", containerName, 0, blobLists.Count));

                DirectoryInfo rootDir = new DirectoryInfo(uploadDirRoot);

                FileInfo[] rootFiles = rootDir.GetFiles();

                ((PowerShellAgent)agent).AddPipelineScript(string.Format("ls -File -Path {0}", uploadDirRoot));
                Test.Info("Upload files...");
                Test.Assert(agent.SetAzureStorageBlobContent(string.Empty, containerName, Storage.BlobType.BlockBlob), "upload multiple files should be successsed");
                Test.Info("Upload finished...");
                blobLists = container.ListBlobs(string.Empty, true, BlobListingDetails.All).ToList();
                Test.Assert(blobLists.Count == rootFiles.Count(), string.Format("set-azurestorageblobcontent should upload {0} files, and actually it's {1}", rootFiles.Count(), blobLists.Count));

                ICloudBlob blob = null;
                for (int i = 0, count = rootFiles.Count(); i < count; i++)
                {
                    blob = blobLists[i] as ICloudBlob;

                    if (blob == null)
                    {
                        Test.AssertFail("blob can't be null");
                    }

                    Test.Assert(rootFiles[i].Name == blob.Name, string.Format("blob name should be {0}, and actully it's {1}", rootFiles[i].Name, blob.Name));
                    string localMd5 = Helper.GetFileContentMD5(Path.Combine(uploadDirRoot, rootFiles[i].Name));
                    Test.Assert(blob.BlobType == Microsoft.WindowsAzure.Storage.Blob.BlobType.BlockBlob, "blob type should be block blob");
                    Test.Assert(localMd5 == blob.Properties.ContentMD5, string.Format("blob content md5 should be {0}, and actualy it's {1}", localMd5, blob.Properties.ContentMD5));
                }
            }
            finally
            {
                blobUtil.RemoveContainer(containerName);
            }
        }

        /// <summary>
        /// upload files in subdirectory
        /// 8.14	Set-AzureStorageBlobContent positive functional cases.
        ///     4. Upload a block blob file and a page blob file with a subdirectory
        /// </summary>
        [TestMethod()]
        [TestCategory(Tag.Function)]
        [TestCategory(PsTag.Blob)]
        [TestCategory(PsTag.SetBlobContent)]
        public void SetBlobContentWithSubDirectory()
        {
            DirectoryInfo rootDir = new DirectoryInfo(uploadDirRoot);

            DirectoryInfo[] dirs = rootDir.GetDirectories();

            foreach (DirectoryInfo dir in dirs)
            {
                string containerName = Utility.GenNameString("container");
                CloudBlobContainer container = blobUtil.CreateContainer(containerName);

                try
                {
                    List<IListBlobItem> blobLists = container.ListBlobs(string.Empty, true, BlobListingDetails.All).ToList();
                    Test.Assert(blobLists.Count == 0, string.Format("container {0} should contain {1} blobs, and actually it contain {2} blobs", containerName, 0, blobLists.Count));

                    Storage.BlobType blobType = Storage.BlobType.BlockBlob;

                    if (dir.Name.StartsWith("dirpage"))
                    {
                        blobType = Microsoft.WindowsAzure.Storage.Blob.BlobType.PageBlob;
                    }

                    ((PowerShellAgent)agent).AddPipelineScript(string.Format("ls -File -Recurse -Path {0}", dir.FullName));
                    Test.Info("Upload files...");
                    Test.Assert(agent.SetAzureStorageBlobContent(string.Empty, containerName, blobType), "upload multiple files should be successsed");
                    Test.Info("Upload finished...");

                    blobLists = container.ListBlobs(string.Empty, true, BlobListingDetails.All).ToList();
                    List<string> dirFiles = files.FindAll(item => item.StartsWith(dir.Name));
                    Test.Assert(blobLists.Count == dirFiles.Count(), string.Format("set-azurestorageblobcontent should upload {0} files, and actually it's {1}", dirFiles.Count(), blobLists.Count));

                    ICloudBlob blob = null;
                    for (int i = 0, count = dirFiles.Count(); i < count; i++)
                    {
                        blob = blobLists[i] as ICloudBlob;

                        if (blob == null)
                        {
                            Test.AssertFail("blob can't be null");
                        }

                        string convertedName = blobUtil.ConvertBlobNameToFileName(blob.Name, dir.Name);
                        Test.Assert(dirFiles[i] == convertedName, string.Format("blob name should be {0}, and actully it's {1}", dirFiles[i], convertedName));
                        string localMd5 = Helper.GetFileContentMD5(Path.Combine(uploadDirRoot, dirFiles[i]));
                        Test.Assert(blob.BlobType == blobType, "blob type should be block blob");
                        Test.Assert(localMd5 == blob.Properties.ContentMD5, string.Format("blob content md5 should be {0}, and actualy it's {1}", localMd5, blob.Properties.ContentMD5));
                    }
                }
                finally
                {
                    blobUtil.RemoveContainer(containerName);
                }
            }
        }

        /// <summary>
        /// set blob content with invalid bob name
        /// 8.14	Set-AzureStorageBlobContent negative functional cases
        ///     1. Upload a block blob file and a page blob file with a subdirectory
        /// </summary>
        [TestMethod()]
        [TestCategory(Tag.Function)]
        [TestCategory(PsTag.Blob)]
        [TestCategory(PsTag.SetBlobContent)]
        public void SetBlobContentWithInvalidBlobName()
        {
            string containerName = Utility.GenNameString("container");
            CloudBlobContainer container = blobUtil.CreateContainer(containerName);

            try
            {
                int MaxBlobNameLength = 1024;
                string blobName = new string('a', MaxBlobNameLength + 1);

                List<IListBlobItem> blobLists = container.ListBlobs(string.Empty, true, BlobListingDetails.All).ToList();
                Test.Assert(blobLists.Count == 0, string.Format("container {0} should contain {1} blobs, and actually it contain {2} blobs", containerName, 0, blobLists.Count));

                Test.Assert(!agent.SetAzureStorageBlobContent(Path.Combine(uploadDirRoot, files[0]), containerName, Storage.BlobType.BlockBlob, blobName), "upload blob with invalid blob name should be failed");
                string expectedErrorMessage = string.Format("Blob name '{0}' is invalid.", blobName);
                Test.Assert(agent.ErrorMessages[0] == expectedErrorMessage, expectedErrorMessage);
            }
            finally
            {
                blobUtil.RemoveContainer(containerName);
            }
        }

        /// <summary>
        /// set blob content with invalid blob type
        /// 8.14	Set-AzureStorageBlobContent negative functional cases
        ///     6.	Upload a blob file with the same name but with different BlobType
        /// </summary>
        [TestMethod()]
        [TestCategory(Tag.Function)]
        [TestCategory(PsTag.Blob)]
        [TestCategory(PsTag.SetBlobContent)]
        public void SetBlobContentWithInvalidBlobType()
        {
            string containerName = Utility.GenNameString("container");
            CloudBlobContainer container = blobUtil.CreateContainer(containerName);

            try
            {
                string blobName = files[0];

                List<IListBlobItem> blobLists = container.ListBlobs(string.Empty, true, BlobListingDetails.All).ToList();
                Test.Assert(blobLists.Count == 0, string.Format("container {0} should contain {1} blobs, and actually it contain {2} blobs", containerName, 0, blobLists.Count));

                Test.Assert(agent.SetAzureStorageBlobContent(Path.Combine(uploadDirRoot, files[0]), containerName, Storage.BlobType.BlockBlob, blobName), "upload blob should be successful.");
                blobLists = container.ListBlobs(string.Empty, true, BlobListingDetails.All).ToList();
                Test.Assert(blobLists.Count == 1, string.Format("container {0} should contain {1} blobs, and actually it contain {2} blobs", containerName, 1, blobLists.Count));
                string convertBlobName = blobUtil.ConvertFileNameToBlobName(blobName);
                Test.Assert(((ICloudBlob)blobLists[0]).Name == convertBlobName, string.Format("blob name should be {0}, actually it's {1}", convertBlobName, ((ICloudBlob)blobLists[0]).Name));

                Test.Assert(!agent.SetAzureStorageBlobContent(Path.Combine(uploadDirRoot, files[0]), containerName, Storage.BlobType.PageBlob, blobName), "upload blob should be with invalid blob should be failed.");
                string expectedErrorMessage = string.Format("Blob type mismatched, the current blob type of '{0}' is BlockBlob.", ((ICloudBlob)blobLists[0]).Name);
                Test.Assert(agent.ErrorMessages[0] == expectedErrorMessage, string.Format("Expect error message: {0} != {1}", expectedErrorMessage, agent.ErrorMessages[0]));
            }
            finally
            {
                blobUtil.RemoveContainer(containerName);
            }
        }

        /// <summary>
        /// upload page blob with invalid file size
        /// 8.14	Set-AzureStorageBlobContent negative functional cases
        ///     8.	Upload a page blob the size of which is not 512*n
        /// </summary>
        [TestMethod()]
        [TestCategory(Tag.Function)]
        [TestCategory(PsTag.Blob)]
        [TestCategory(PsTag.SetBlobContent)]
        public void SetPageBlobWithInvalidFileSize()
        {
            string fileName = Utility.GenNameString("tinypageblob");
            string filePath = Path.Combine(uploadDirRoot, fileName);
            int fileSize = 480;
            Helper.GenerateTinyFile(filePath, fileSize);
            string containerName = Utility.GenNameString("container");
            CloudBlobContainer container = blobUtil.CreateContainer(containerName);

            try
            {
                List<IListBlobItem> blobLists = container.ListBlobs(string.Empty, true, BlobListingDetails.All).ToList();
                Test.Assert(blobLists.Count == 0, string.Format("container {0} should contain {1} blobs, and actually it contain {2} blobs", containerName, 0, blobLists.Count));
                Test.Assert(!agent.SetAzureStorageBlobContent(filePath, containerName, Storage.BlobType.PageBlob), "upload page blob with invalid file size should be failed.");
                string expectedErrorMessage = "The page blob size must be a multiple of 512 bytes.";
                Test.Assert(agent.ErrorMessages[0].StartsWith(expectedErrorMessage), expectedErrorMessage);
                blobLists = container.ListBlobs(string.Empty, true, BlobListingDetails.All).ToList();
                Test.Assert(blobLists.Count == 0, string.Format("container {0} should contain {1} blobs, and actually it contain {2} blobs", containerName, 0, blobLists.Count));
            }
            finally
            {
                blobUtil.RemoveContainer(containerName);
            }
        }
    }
}
