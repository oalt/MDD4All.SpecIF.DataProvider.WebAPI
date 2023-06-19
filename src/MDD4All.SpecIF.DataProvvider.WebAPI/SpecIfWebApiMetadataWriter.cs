using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MDD4All.SpecIF.DataProvider.WebAPI
{
    public class SpecIfWebApiMetadataWriter : AbstractSpecIfMetadataWriter
    {
        private string _connectionURL;
        private HttpClient _httpClient;

        public SpecIfWebApiMetadataWriter(string connectionURL)
        {
            _httpClient = new HttpClient();
            _connectionURL = connectionURL;
        }

        public override void AddDataType(DataType dataType)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/datatypes/");
            PostData<DataType, DataType>(uriBuilder.Uri, dataType);
        }

        public override void AddPropertyClass(PropertyClass propertyClass)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/propertyClasses/");
            PostData<PropertyClass, PropertyClass>(uriBuilder.Uri, propertyClass);
        }

        public override void AddResourceClass(ResourceClass resourceClass)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/resourceClasses/");
            PostData<ResourceClass, ResourceClass>(uriBuilder.Uri, resourceClass);
        }

        public override void AddStatementClass(StatementClass statementClass)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/statementClasses/");
            PostData<StatementClass, StatementClass>(uriBuilder.Uri, statementClass);
        }
        public override void UpdateDataType(DataType dataType)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/datatypes/");
            PutData<DataType, DataType>(uriBuilder.Uri, dataType);
        }

        public override void UpdatePropertyClass(PropertyClass propertyClass)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/propertyClasses/");
            PutData<PropertyClass, PropertyClass>(uriBuilder.Uri, propertyClass);
        }

        public override void UpdateResourceClass(ResourceClass resourceClass)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/resourceClasses/");
            PutData<ResourceClass, ResourceClass>(uriBuilder.Uri, resourceClass);
        }

        public override void UpdateStatementClass(StatementClass statementClass)
        {
            UriBuilder uriBuilder = new UriBuilder(_connectionURL + "/specif/v1.1/statementClasses/");
            PutData<StatementClass, StatementClass>(uriBuilder.Uri, statementClass);
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
                Debug.WriteLine(exception);
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
    }
}