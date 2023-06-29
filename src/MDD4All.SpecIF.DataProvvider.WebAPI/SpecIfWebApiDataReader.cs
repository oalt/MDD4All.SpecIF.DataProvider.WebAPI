/*
 * Copyright (c) MDD4All.de, Dr. Oliver Alt
 */
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace MDD4All.SpecIF.DataProvider.WebAPI
{
    public class SpecIfWebApiDataReader : AbstractSpecIfDataReader
    {
        private string _connectionURL;

        private HttpClient _httpClient = new HttpClient();

        public SpecIfWebApiDataReader(string webApiConnectionURL, string apiKey = null)
        {
            _connectionURL = webApiConnectionURL;

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (apiKey != null)
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            }
        }

        public override List<Node> GetAllHierarchies()
        {
            List<Node> result = new List<Node>();
            bool rootNodesOnly = false;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies");
            uriBuilder.Query = "rootNodesOnly=" + rootNodesOnly.ToString();
            Uri finalUrl = uriBuilder.Uri;

            Task<string> task = _httpClient.GetStringAsync(finalUrl);
            task.Wait();

            result = JsonConvert.DeserializeObject<List<Node>>(task.Result);

            return result;
        }

        public async Task<List<Node>> GetAllHierarchiesAsync()
        {
            List<Node> result = new List<Node>();

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies");

            Uri finalUrl = uriBuilder.Uri;

            string answer = await _httpClient.GetStringAsync(finalUrl);

            result = JsonConvert.DeserializeObject<List<Node>>(answer); 

            return result;
        }

        public override byte[] GetFile(string filename)
        {
            throw new NotImplementedException();
        }

        public override Node GetHierarchyByKey(Key key)
        {
            Node result = null;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies/" + key.ID);

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["revision"] = key.Revision;

            uriBuilder.Query = parameters.ToString();

            Uri finalUrl = uriBuilder.Uri;

            //Task<Node> task = GetDataFromServiceAsync<Node>(finalUrl);
            //task.Wait();

            //return task.Result;

            result = GetDataFromService<Node>(finalUrl);

            return result;
        }

        public override string GetLatestHierarchyRevision(string hierarchyID)
		{
            //Task<string> task = GetLatestRevisionAsync<Node>(hierarchyID, "specif/v1.1/Hierarchy/");
            //task.Wait();

            //return task.Result;





            //string result = GetLatestRevision<Node>(hierarchyID, "/specif/v1.1/hierarchies/");
            return null;
		}


		public override string GetLatestStatementRevision(string statementID)
		{
            //Task<string> task = GetLatestRevisionAsync<Statement>(statementID, "specif/v1.1/Statement/");
            //task.Wait();

            //return task.Result;

            string result = GetLatestRevision<Node>(statementID, "/specif/v1.1/statements/");
            return result;
        }

		public override Resource GetResourceByKey(Key key)
        {
			Resource result;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/resources/" + key.ID);

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["revision"] = key.Revision;
           
            uriBuilder.Query = parameters.ToString();
           
            Uri finalUrl = uriBuilder.Uri;

            Debug.WriteLine(finalUrl);

            //Task<Resource> task = GetDataFromServiceAsync<Resource>(finalUrl);
            //task.Wait();
            //result = task.Result;

            result = GetDataFromService<Resource>(finalUrl);

			if (result != null)
			{
				result.DataSource = DataSourceDescription;
			}

            return result;
        }

		public async Task<Resource> GetResourceByKeyAsync(Key key)
        {
            Resource result = null;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/resources/" + key.ID);

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["revision"] = key.Revision;

            uriBuilder.Query = parameters.ToString();

            Uri finalUrl = uriBuilder.Uri;

            Debug.WriteLine(finalUrl);

            result = await GetDataFromServiceAsync<Resource>(finalUrl);

            //result = GetDataFromService<Resource>(finalUrl);

            return result;
        }

		public override Statement GetStatementByKey(Key key)
		{
            Statement result = null;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/statements/" + key.ID);

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["revision"] = key.Revision;

            uriBuilder.Query = parameters.ToString();

            Uri finalUrl = uriBuilder.Uri;

            //result = await GetDataFromServiceAsync<Statement>(finalUrl);

            result = GetDataFromService<Statement>(finalUrl);

            return result;
		}

		private async Task<T> GetDataFromServiceAsync<T>( Uri uri)
		{
			T result = default(T);

			try
			{
				string answer = await _httpClient.GetStringAsync(uri);

				result = JsonConvert.DeserializeObject<T>(answer, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			}
			catch (Exception exception)
			{
				
				//Console.WriteLine(exception);
			}

			return result;
		}

        private T GetDataFromService<T>(Uri uri)
        {
            T result = default(T);

            try
            {
                Task<string> task = _httpClient.GetStringAsync(uri);
                task.Wait();

                result = JsonConvert.DeserializeObject<T>(task.Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception exception)
            {

                //Console.WriteLine(exception);
            }

            return result;
        }

        public T GetItemByKey<T>(Key itemKey, string apiPath) {
            T result = default(T);

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + apiPath + itemKey.ID);

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["revision"] = itemKey.Revision;

            uriBuilder.Query = parameters.ToString();

            Uri finalUrl = uriBuilder.Uri;

            //Task<Node> task = GetDataFromServiceAsync<Node>(finalUrl);
            //task.Wait();

            //return task.Result;

            result = GetDataFromService<T>(finalUrl);

            return result;
        }

        //public override T GetItemWithLatestRevision<T>(string itemID, string apiPath)
        //{
        //    T result = default(T);

        //    string revisionID = GetLatestRevision<T>(itemID, apiPath);
        //    Key itemKey = new Key(itemID, revisionID);
        //    result = GetItemByKey<T> (itemKey, revisionID);

        //    return result;
        //}

        public async Task<string> GetLatestRevisionAsync<T>(string resourceID, string apiPath)
		{
			string result = null;

			string answer = await _httpClient.GetStringAsync(_connectionURL + apiPath + resourceID + "/latestRevision");

			return result;
		}
        public string GetLatestRevision<T>(string resourceID, string apiPath)
        {
            string result = string.Empty;

            string fullPath = _connectionURL + apiPath + resourceID + "/latestRevision";
            Task<string> task = _httpClient.GetStringAsync(fullPath);
            task.Wait();
            result = task.Result.Trim('"');

            return result;
        }

        public override List<Statement> GetAllStatementsForResource(Key resourceKey)
		{
            List<Statement> result = new List<Statement>();

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/statements");

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["subjectID"] = resourceKey.ID;
            parameters["subjectRevision"] = resourceKey.Revision;
            parameters["objectID"] = resourceKey.ID;
            parameters["objectRevision"] = resourceKey.Revision;

            uriBuilder.Query = parameters.ToString();

            Uri finalUrl = uriBuilder.Uri;

            //Task<List<Statement>> task = GetDataFromServiceAsync<List<Statement>>(finalUrl);
            //task.Wait();

            //return task.Result;

            result = GetDataFromService<List<Statement>>(finalUrl);

            return result;
        }

		public override List<Node> GetContainingHierarchyRoots(Key resourceKey)
		{
			throw new NotImplementedException();
		}

        

        public override List<Resource> GetAllResourceRevisions(string resourceID)
        {
            throw new NotImplementedException();
        }

        public override List<Statement> GetAllStatementRevisions(string statementID)
        {
            throw new NotImplementedException();
        }

        public override List<Statement> GetAllStatements()
        {
            throw new NotImplementedException();
        }

        public override List<Node> GetChildNodes(Key parentNodeKey)
        {
            List<Node> result = new List<Node>();

            Node parentNode = GetNodeByKey(parentNodeKey);

            if (parentNode != null)
            {
                foreach (Key childKey in parentNode.NodeReferences)
                {
                    Node childNode = GetNodeByKey(childKey);
                    if (childNode != null)
                    {
                        result.Add(childNode);
                    }
                    else
                    {
                        result = null;
                        break;
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public override string GetLatestResourceRevisionForBranch(string resourceID, string branchName)
        {
            throw new NotImplementedException();
        }

        public override Node GetNodeByKey(Key key)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies/" + key.ID);
            if (key.Revision != null)
            {
                System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters["revision"] = key.Revision;
                uriBuilder.Query = parameters.ToString();
            }

            Task<HttpResponseMessage> task = _httpClient.GetAsync(uriBuilder.Uri);
            task.Wait();

            string answer = task.Result.Content.ReadAsStringAsync().Result;

            Node result = JsonConvert.DeserializeObject<Node>(answer, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return result;
        }



        public override Node GetParentNode(Key childNodeKey)
        {
            Node result = null;
            List<Node> allHierarchies = GetAllHierarchies();
            List<Node> childNodes = GetAllChildNodesRecursively(allHierarchies);

            List<Node> allNodes = allHierarchies;
            allNodes.AddRange(childNodes);
            
            foreach(Node node in allNodes)
            {
                foreach(Key key in node.NodeReferences)
                {
                    if (key.ID == childNodeKey.ID)
                    {
                        result = node;
                        break;
                    }
                }
            }

            return result;

            //foreach (KeyValuePair<string, DataModels.SpecIF> keyValuePair in SpecIfData)
            //{
            //    DataModels.SpecIF specif = keyValuePair.Value;

            //    Node node = specif.GetParentNode(childNode.ID);
            //    if (node != null)
            //    {
            //        result = node;
            //        break;
            //    }
            //}
        }

        private List<Node> GetAllChildNodesRecursively(List<Node> hierarchiesToSearch)
        {
            List<Node> result = new List<Node>();
            
            foreach(Node node in hierarchiesToSearch) 
            {
                if(node.NodeReferences.Count > 0)
                {
                    List<Node> childNodes = GetChildNodes(new Key(node.ID));
                    result.AddRange(childNodes);
                    List<Node> lowerChildNodes = GetAllChildNodesRecursively(childNodes);
                    result.AddRange(lowerChildNodes);
                }
            }
            return result;
        }

        public override List<ProjectDescriptor> GetProjectDescriptions()
        {
            List<ProjectDescriptor> result;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/projects");

            //Task<List<ProjectDescriptor>> task = GetDataFromServiceAsync<List<ProjectDescriptor>>(uriBuilder.Uri);
            //task.Wait();

            //return task.Result;

            result = GetDataFromService<List<ProjectDescriptor>>(uriBuilder.Uri);

            return result;
        }

        public override List<Node> GetAllHierarchyRootNodes(string projectID = null)
        {
            List<Node> result;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies/");

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["rootNodesOnly"] = true.ToString();

            if (projectID != null)
            {
                parameters["project"] = projectID;
            }

            uriBuilder.Query = parameters.ToString();

            Uri finalUrl = uriBuilder.Uri;

            Debug.WriteLine(finalUrl);

            //Task<List<Node>> task = GetDataFromServiceAsync<List<Node>>(finalUrl);
            //task.Wait();

            //return task.Result;

            result = GetDataFromService<List<Node>>(finalUrl);

            return result;
        }

        public override DataModels.SpecIF GetProject(ISpecIfMetadataReader metadataReader, string projectID, List<Key> hierarchyFilter = null, bool includeMetadata = true)
        {
            DataModels.SpecIF result;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/projects/" + projectID);

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            parameters["includeMetedata"] = includeMetadata.ToString();

            uriBuilder.Query = parameters.ToString();

            Uri finalUrl = uriBuilder.Uri;

            Debug.WriteLine(finalUrl);

            //Task<DataModels.SpecIF> task = GetDataFromServiceAsync<DataModels.SpecIF>(finalUrl);
            //task.Wait();

            //return task.Result;

            result = GetDataFromService<DataModels.SpecIF>(finalUrl);

            return result;
        }
    }
}
