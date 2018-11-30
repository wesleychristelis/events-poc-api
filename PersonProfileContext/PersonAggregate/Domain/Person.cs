using System;

namespace EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        private const string PersonRootUri = "http://eventuality.poc/person/";

        #region Public
        public void PopulateId()
        {
            Id = Id ?? PersonRootUri + Guid.NewGuid().ToString();
        }
        #endregion
    }
}