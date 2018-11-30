namespace EventualityPOCApi.Shared.Xapi
{
    public class Verb
    {
        public const string PersonCreated = "http://eventuality.poc/xapi/verb/person/created";
        public const string PersonCreationFailed = "http://eventuality.poc/xapi/verb/person/creation-failed";
        public const string PersonCreationRequested = "http://eventuality.poc/xapi/verb/person/creation-requested";
        public const string PersonRequested = "http://eventuality.poc/xapi/verb/person/requested";
        public const string PersonRetrievalFailed = "http://eventuality.poc/xapi/verb/person/retrieval-failed";
        public const string PersonRetrieved = "http://eventuality.poc/xapi/verb/person/retrieved";
        public const string PersonUpdated = "http://eventuality.poc/xapi/verb/person/updated";
        public const string PersonUpdateFailed = "http://eventuality.poc/xapi/verb/person/updated-failed";
        public const string PersonUpdateRequested = "http://eventuality.poc/xapi/verb/person/update-requested";
        public const string SystemExceptionOcurred = "http://eventuality.poc/xapi/verb/system/exception-ocurred";
        public const string SystemTest = "http://eventualityPOC.com/xapi/verb/system/test";
    }
}
