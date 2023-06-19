/*
 * Copyright (c) MDD4All.de, Dr. Oliver Alt
 */
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Helpers;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace MDD4All.SpecIF.DataProvider.WebAPI
{
    public class SpecIfWebApiDataWriter : AbstractSpecIfDataWriter
    {
        private string _connectionURL;

        private HttpClient _httpClient = new HttpClient();

        public SpecIfWebApiDataWriter(string webApiConnectionURL,
                                      string apiKey,
                                      ISpecIfMetadataReader metadataReader, 
                                      ISpecIfDataReader dataReader) : base(metadataReader, dataReader)
        {
            _connectionURL = webApiConnectionURL;

            

            _httpClient.DefaultRequestHeaders.Accept.Clear();

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            if (apiKey != null)
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            }

        }

        public override void AddHierarchy(Node hierarchy, string projectID = null)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies/");

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            if (projectID != null)
            {
                parameters["projectID"] = projectID;

                uriBuilder.Query = parameters.ToString();
            }

            //PostDataAsync<Node, Node>(uriBuilder.Uri, hierarchy).Wait();
            PostData<Node, Node>(uriBuilder.Uri, hierarchy);
        }

        public override void AddNodeAsFirstChild(string parentNodeID, Node newNode)
        {
            string apiPath = "/specif/v1.1/hierarchies/";
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + apiPath);

            Key key = new Key();
            key.ID = parentNodeID;
            key.Revision = _dataReader.GetLatestHierarchyRevision(parentNodeID);

            Node parentNode = _dataReader.GetNodeByKey(key);

            if (parentNodeID != null)
            {

                if(string.IsNullOrEmpty(newNode.Revision))
                {
                    newNode.Revision = SpecIfGuidGenerator.CreateNewRevsionGUID();
                }
                if (string.IsNullOrEmpty(newNode.ID))
                {
                    newNode.ID = SpecIfGuidGenerator.CreateNewSpecIfGUID();
                }

                //PostDataAsync<Node, Node>(uriBuilder.Uri, newNode).Wait();
                PostData<Node, Node>(uriBuilder.Uri, newNode);

                //if(parentNode.Nodes == null)
                //{
                //    parentNode.Nodes = new List<Node>();
                //    parentNode.Nodes.Add(newNode);
                //}
                //else
                //{
                //    parentNode.Nodes.Insert(0, newNode);
                //}
                parentNode.NodeReferences.Insert(0, new Key(newNode.ID, newNode.Revision));

                UpdateHierarchy(parentNode, parentNodeID);
            }
        }

        private const string DEFAULT_PROJECT = "PRJ-DEFAULT";

        public override void AddProject(ISpecIfMetadataWriter metadataWriter, DataModels.SpecIF project, string integrationID = null)
        {
            string ProjectId = DEFAULT_PROJECT;
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/projects");
            PostData<DataModels.SpecIF, DataModels.SpecIF>(uriBuilder.Uri, project);
            IntegrateProjectData(metadataWriter, project, integrationID ?? ProjectId);
        }

        public override void AddResource(Resource resource)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/resources");
            //PostDataAsync<Resource, Resource>(uriBuilder.Uri, resource).Wait();
            PostData<Resource, Resource>(uriBuilder.Uri, resource);
        }

        public override void AddStatement(Statement statement)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/statements/");

            //PostDataAsync<Statement, Statement>(uriBuilder.Uri, statement).Wait();
            PostData<Statement, Statement>(uriBuilder.Uri, statement);
        }

        public override void DeleteProject(string projectID)
        {
            throw new NotImplementedException();
        }

        public override void InitializeIdentificators()
        {
            //throw new NotImplementedException();
        }

        public override void MoveNode(string nodeID, string newParentID, string newSiblingId)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies/move");

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            if (nodeID != null)
            {
                parameters["nodeId"] = nodeID;
            }

            if (newParentID != null)
            {
                parameters["newParentId"] = newParentID;
            }

            if (newSiblingId != null)
            {
                parameters["newSiblingId"] = newSiblingId;
            }

            uriBuilder.Query = parameters.ToString();

            //Task<HttpResponseMessage> responseMessageTask = PutCommandAsync(uriBuilder.Uri);

            //responseMessageTask.Wait();

            //HttpResponseMessage responseMessage = responseMessageTask.Result;

            HttpResponseMessage responseMessage = PutCommand(uriBuilder.Uri);
        }



        public override Node UpdateHierarchy(Node hierarchyToUpdate, string parentID = null, string predecessorID = null)
        {
            Node result = null;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies/");

            Uri finalUrl = uriBuilder.Uri;


            //Task<Node> resourceTask = PutDataAsync<Node, Node>(finalUrl, hierarchyToUpdate);

            //resourceTask.Wait();

            //result = resourceTask.Result;

            result = PutData<Node, Node>(finalUrl, hierarchyToUpdate);

            return result;
        }

        public override void SaveIdentificators()
        {
            //throw new NotImplementedException();
        }

        public override Resource SaveResource(Resource resource, string projectID = null)
        {
            Resource result = null;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/resources/");

            System.Collections.Specialized.NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            if (projectID != null)
            {
                parameters["projectID"] = projectID;
            }

            uriBuilder.Query = parameters.ToString();

            Uri finalUrl = uriBuilder.Uri;
            

            //Task<Resource> resourceTask = PostDataAsync<Resource, Resource>(finalUrl, resource);

            //resourceTask.Wait();

            //result = resourceTask.Result;

            result = PostData<Resource, Resource>(finalUrl, resource);

            return result;
        }

        public override Statement SaveStatement(Statement statement, string projectID = null)
        {
            throw new NotImplementedException();
        }



        public override void UpdateProject(ISpecIfMetadataWriter metadataWriter, DataModels.SpecIF project)
        {
            throw new NotImplementedException();
        }

        public override Resource UpdateResource(Resource resource)
        {
            Resource result = null;

            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/resources/");

            Uri finalUrl = uriBuilder.Uri;

            //Task<Resource> resourceTask = PutDataAsync<Resource, Resource>(finalUrl, resource);

            //resourceTask.Wait();

            //result = resourceTask.Result;

            result = PutData<Resource, Resource>(finalUrl, resource);

            return result;
        }

        private async Task<TResult> PostDataAsync<T, TResult>(Uri url, T data)
        {
            TResult result = default(TResult);

            try
            {
                string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(url, stringContent);

                string jiraResult = response.Content.ReadAsStringAsync().Result;

                result = JsonConvert.DeserializeObject<TResult>(jiraResult);
            }
            catch(Exception exception)
            {
                Debug.WriteLine(exception);
            }

            return result;
        }

        private TResult PostData<T, TResult>(Uri url, T data)
        {
            TResult result = default(TResult);

            try
            {
                string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                
                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> task = _httpClient.PostAsync(url, stringContent);
                
                task.Wait();

                string jiraResult = task.Result.Content.ReadAsStringAsync().Result;

                result = JsonConvert.DeserializeObject<TResult>(jiraResult);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        private async Task<TResult> PutDataAsync<T, TResult>(Uri url, T data)
        {
            TResult result = default(TResult);

            try
            {
                string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync(url, stringContent);

                string jiraResult = response.Content.ReadAsStringAsync().Result;

                result = JsonConvert.DeserializeObject<TResult>(jiraResult);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        private TResult PutData<T, TResult>(Uri url, T data)
        {
            TResult result = default(TResult);

            try
            {
                string json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> task = _httpClient.PutAsync(url, stringContent);
                task.Wait();
                
                string jiraResult = task.Result.Content.ReadAsStringAsync().Result;

                result = JsonConvert.DeserializeObject<TResult>(jiraResult);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            return result;
        }

        private void DeleteData<T>(Uri url, T itemToDelete)
        {
            Task<HttpResponseMessage> taskGet = _httpClient.GetAsync(url);
            taskGet.Wait();

            Task taskDelete = _httpClient.DeleteAsync(url);
        }

        private async Task<HttpResponseMessage> PutCommandAsync(Uri url)
        {
            HttpResponseMessage result = null;

            try
            {
                
                StringContent stringContent = new StringContent("", Encoding.UTF8, "application/json");
                result = await _httpClient.PutAsync(url, stringContent);

                
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            return result;
        }

        private HttpResponseMessage PutCommand(Uri url)
        {
            HttpResponseMessage result = null;

            try
            {

                StringContent stringContent = new StringContent("", Encoding.UTF8, "application/json");
                Task<HttpResponseMessage> task = _httpClient.PutAsync(url, stringContent);
                task.Wait();
                result = task.Result;

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            return result;
        }

        public override void AddNodeAsPredecessor(string predecessorID, Node newNode)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies");

            Node parentNode = _dataReader.GetParentNode(new Key { ID = predecessorID, Revision = null });

            Node predecessor = _dataReader.GetNodeByKey(new Key { ID = predecessorID, Revision = null });

            if (parentNode != null && predecessor != null)
            {

                if (string.IsNullOrEmpty(newNode.Revision))
                {
                    newNode.Revision = SpecIfGuidGenerator.CreateNewRevsionGUID();
                }

                if (string.IsNullOrEmpty(newNode.ID))
                {
                    newNode.ID = SpecIfGuidGenerator.CreateNewSpecIfGUID();
                }

                //int index = 0;

                //foreach (Key nodeKey in parentNode.NodeReferences)
                //{
                //    if (nodeKey.ID == predecessor.ID)
                //    {
                //        break;
                //    }
                //    index++;
                //}

                //_nodeMongoDbAccessor.Add(newNode);
                PostData<Node, Node>(uriBuilder.Uri, newNode);

                parentNode.NodeReferences.Add(new Key(newNode.ID, newNode.Revision));

                //_hierarchyMongoDbAccessor.Update(parentNode, parentNode.Id);
                UpdateHierarchy(parentNode);

            }
        }

        public override void DeleteNode(string nodeID)
        {
            Node nodeToDelete = _dataReader.GetNodeByKey(new Key { ID = nodeID, Revision = null });

            Node parent = _dataReader.GetParentNode(new Key { ID = nodeID, Revision = null });

            if (nodeToDelete != null && parent != null)
            {

                int index = -1;

                for (int counter = 0; counter < parent.NodeReferences.Count; counter++)
                {

                    if (parent.NodeReferences[counter].ID == nodeID)
                    {
                        index = counter;
                        break;
                    }
                }

                if (index > -1)
                {
                    parent.NodeReferences.RemoveAt(index);
                }

                UpdateHierarchy(parent, parent.Id);
            }
            else if (nodeToDelete != null && nodeToDelete.IsHierarchyRoot)
            {
                DeleteHierarchy(nodeToDelete.Id);
            }
        }

        public void DeleteHierarchy(string id)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/hierarchies/");

            Node nodeToDelete = _dataReader.GetNodeByKey(new Key(id));

            DeleteData<Node>(uriBuilder.Uri, nodeToDelete);
        }
    }
}
