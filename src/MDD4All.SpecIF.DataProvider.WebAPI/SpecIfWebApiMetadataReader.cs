/*
 * Copyright (c) MDD4All.de, Dr. Oliver Alt
 */
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MDD4All.SpecIF.DataProvider.WebAPI
{
    public class SpecIfWebApiMetadataReader : AbstractSpecIfMetadataReader
	{
		
		private string _connectionURL;

		private HttpClient _httpClient;

		public SpecIfWebApiMetadataReader(string webApiConnectionURL)
		{
			_httpClient = new HttpClient();
			_connectionURL = webApiConnectionURL;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

		public override List<DataType> GetAllDataTypes()
		{
			List<DataType> result = new List<DataType>();

			//Task <List<DataType>> task = GetDataListAsync<DataType>("/specif/v1.1/DataTypes");
			//task.Wait();

			//return task.Result;

			result = GetDataList<DataType>("/specif/v1.1/DataTypes");
			return result;

		}

		
        public override List<PropertyClass> GetAllPropertyClasses()
		{
			//Task<List<PropertyClass>> task = GetDataListAsync<PropertyClass>("/specif/v1.1/propertyClasses");
			//task.Wait();

			//return task.Result;
			List<PropertyClass> result = GetDataList<PropertyClass>("/specif/v1.1/propertyClasses");
			return result;
		}

		public override List<ResourceClass> GetAllResourceClasses()
		{
			//Task<List<ResourceClass>> task = GetDataListAsync<ResourceClass>("/specif/v1.1/resourceClasses");
			//task.Wait();

			//return task.Result;
			List<ResourceClass> result = GetDataList<ResourceClass>("/specif/v1.1/resourceClasses");
			return result;
		}

		


		public override ResourceClass GetResourceClassByKey(Key key)
		{
			//Task task = GetDataByKeyAsync<ResourceClass>(key, "/specif/v1.1/resourceClasses");
			//task.Wait();
			//return task.Result;

			var result = GetDataByKey<ResourceClass>(key, "/specif/v1.1/resourceClasses");
			
			return result;
		}

		public override StatementClass GetStatementClassByKey(Key key)
		{
            //Task task = GetDataByKeyAsync<StatementClass>(key, "/specif/v1.1/statementClasses");
            //task.Wait();
            //return task.Result;

            var result = GetDataByKey<StatementClass>(key, "/specif/v1.1/statementClasses");
			
			return result;
		}

		public override PropertyClass GetPropertyClassByKey(Key key)
		{
            //Task task = GetDataByKeyAsync<PropertyClass>(key, "/specif/v1.1/propertyClasses");
            //task.Wait();
            //return task.Result;

            var result = GetDataByKey<PropertyClass>(key, "/specif/v1.1/propertyClasses");
			
			return result;
		}

		public override string GetLatestPropertyClassRevision(string propertyClassID)
		{
			//Task<string> task = GetLatestRevisionAsync<string>(propertyClassID, "SpecIF/PropertyClass/");
			//task.Wait();

			//return task.Result;

			string result = GetLatestRevision<string>(propertyClassID, "SpecIF/PropertyClass/");
			return result;
        }

		public override string GetLatestResourceClassRevision(string resourceClassID)
		{
            //Task<string> task = GetLatestRevisionAsync<string>(resourceClassID, "SpecIF/ResourceClass/");
            //task.Wait();

            //return task.Result;

            string result = GetLatestRevision<string>(resourceClassID, "SpecIF/ResourceClass/");
            return result;
        }

		public override string GetLatestStatementClassRevision(string statementClassID)
		{
            //Task<string> task = GetLatestRevisionAsync<string>(statementClassID, "SpecIF/StatementClass/");
            //task.Wait();

            //return task.Result;

            string result = GetLatestRevision<string>(statementClassID, "SpecIF/StatementClass/");
            return result;
        }

        private async Task<List<T>> GetDataListAsync<T>(string apiPath)
		{
			List<T> result = new List<T>();

            string answer = await _httpClient.GetStringAsync(_connectionURL + apiPath);

			result = JsonConvert.DeserializeObject<List<T>>(answer);

			return result;
		}

        private List<T> GetDataList<T>(string apiPath)
        {
            List<T> result = new List<T>();

            Task<string> task = _httpClient.GetStringAsync(_connectionURL + apiPath);
			task.Wait();

			result = JsonConvert.DeserializeObject<List<T>>(task.Result);

            return result;
        }

        private async Task<T> GetDataById<T>(string id, string apiPath)
		{
			T result = default(T);

			string answer = await _httpClient.GetStringAsync(_connectionURL + apiPath + "/" + id);

			result = JsonConvert.DeserializeObject<T>(answer);

			return result;
		}
		
		private async Task<T> GetDataByKeyAsync<T>(Key key, string apiPath)
		{
			T result = default(T);

			try
			{
				string answer = await _httpClient.GetStringAsync(_connectionURL + apiPath + "/" + key.ID);

				result = JsonConvert.DeserializeObject<T>(answer);
			}
			catch (Exception exception)
			{
				Console.WriteLine("[ERROR] key=" + key.ID + "--" + key.Revision);
				Console.WriteLine(exception);
			}

			return result;
		}
        private T GetDataByKey<T>(Key key, string apiPath)
        {
            T result = default(T);

            try
            {
                Task<string> task = _httpClient.GetStringAsync(_connectionURL + apiPath + "/" + key.ID);
				task.Wait();

                result = JsonConvert.DeserializeObject<T>(task.Result);
            }
            catch (Exception exception)
            {
                Console.WriteLine("[ERROR] key=" + key.ID + "--" + key.Revision);
                Console.WriteLine(exception);
            }

            return result;
        }

        public async Task<string> GetLatestRevisionAsync<T>(string resourceID, string apiPath)
		{
			string result = "";

			string answer = await _httpClient.GetStringAsync(_connectionURL + apiPath + "/LatestRevision/" + resourceID);

			//result = new Revision(answer);

			return result;
		}
        public string GetLatestRevision<T>(string resourceID, string apiPath)
        {
            string result = "";

            Task<string> task = _httpClient.GetStringAsync(_connectionURL + apiPath + "/LatestRevision/" + resourceID);
			task.Wait();

			result = task.Result;

            //result = new Revision(answer);

            return result;
        }

        public override DataType GetDataTypeByKey(Key key)
        {

            Task<DataType> task = GetDataById<DataType>(key.ID, "/SpecIF/DataType");
            task.Wait();

            return task.Result;
        }

        public override List<StatementClass> GetAllStatementClasses()
        {
			//Task<List<StatementClass>> task = GetDataListAsync<StatementClass>("/specif/v1.1/statementClasses");
			//task.Wait();

			//return task.Result;

			List<StatementClass> result = GetDataList<StatementClass>("/specif/v1.1/statementClasses");
			return result;
        }

        public override List<DataType> GetAllDataTypeRevisions(string dataTypeID)
        {
            throw new NotImplementedException();
        }

        public override List<PropertyClass> GetAllPropertyClassRevisions(string propertyClassID)
        {
            throw new NotImplementedException();
        }

        public override List<ResourceClass> GetAllResourceClassRevisions(string resourceClassID)
        {
            throw new NotImplementedException();
        }

        public override List<StatementClass> GetAllStatementClassRevisions(string statementClassID)
        {
            throw new NotImplementedException();
        }

        public override void NotifyMetadataChanged()
        {
            throw new NotImplementedException();
        }
    }
}
