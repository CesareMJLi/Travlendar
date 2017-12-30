﻿using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using System.Collections.Generic;
using System.ComponentModel;
using Travlendar.Core.AppCore.Model;
using Travlendar.Framework.ViewModels;

namespace Travlendar.Core.AppCore.ViewModels
{
    sealed public class CognitoSyncViewModel : AViewModel<CognitoSyncModel>
    {
        private static CognitoSyncViewModel _instance = new CognitoSyncViewModel ();

        CognitoAWSCredentials credentials;
        CognitoSyncManager syncManager;
        Dictionary<string, Dataset> datasets;

        public override event PropertyChangedEventHandler PropertyChanged;

        private CognitoSyncViewModel ()
        {
            credentials = new CognitoAWSCredentials (Constants.AWS_IDENTITY_POOL_ID, RegionEndpoint.EUWest1);
            syncManager = new CognitoSyncManager (credentials, new AmazonCognitoSyncConfig { RegionEndpoint = RegionEndpoint.EUWest1 });
            datasets = new Dictionary<string, Dataset> ();
        }


        static internal CognitoSyncViewModel GetInstance ()
        {
            return _instance;
        }


        internal void AWSLogin (string providerName, string accessToken)
        {
            credentials.AddLogin (providerName, accessToken);
        }

        public void CreateDataset (string name)
        {
            if ( name == null )
                throw new System.Exception ("Do not create empty named datasets");

            Dataset dataset;
            dataset = syncManager.OpenOrCreateDataset (name);
            dataset.SynchronizeAsync ();

            if ( !datasets.ContainsKey (name) )
                datasets.Add (name, dataset);
            //else
            //throw new System.Exception ("Do not create duplicated datasets");
        }

        public void WriteDataset (string dataset, string key, string value)
        {
            Dataset local;
            if ( !datasets.TryGetValue (dataset, out local) )
                throw new System.Exception (string.Format ("Dataset: {0} not found.", dataset));

            if ( !string.IsNullOrEmpty (key) && !string.IsNullOrEmpty (value) )
            {
                local.Put (key, value);
                local.SynchronizeOnConnectivity ();
            }
        }

        public string ReadDataset (string dataset, string key)
        {
            Dataset local;

            if ( !datasets.TryGetValue (dataset, out local) )
                throw new System.Exception (string.Format ("Dataset: {0} not found.", dataset));

            if ( string.IsNullOrEmpty (key) )
            {
                return string.Empty;
            }

            return local.Get (key);
        }

        public IDictionary<string, string> ReadWholeDataset (string dataset)
        {
            Dataset local;
            if ( !datasets.TryGetValue (dataset, out local) )
                throw new System.Exception (string.Format ("Dataset: {0} not found.", dataset));

            return local.ActiveRecords;

        }

        public void RemoveFromDataset (string dataset, string key)
        {
            Dataset local;

            if ( !datasets.TryGetValue (dataset, out local) )
                throw new System.Exception (string.Format ("Dataset: {0} not found.", dataset));

            if ( !string.IsNullOrEmpty (key) )
            {
                local.Remove (key);
                local.SynchronizeOnConnectivity ();
            }
        }

        public void RemoveDataset (string dataset)
        {
            Dataset local;

            if ( !datasets.TryGetValue (dataset, out local) )
                throw new System.Exception (string.Format ("Dataset: {0} not found.", dataset));

            local.Delete ();
            local.SynchronizeOnConnectivity ();

            datasets.Remove (dataset);
        }

        public override string ToString ()
        {
            return base.ToString ();
        }
    }
}
