using Newtonsoft.Json;

namespace RecursiveGeek.DotNetLib.Facebook
{
    public static class Read
    {
        #region Constants
        public const string ReturnFields = "attachments,message,created_time,updated_time,from,admin_creator";
        #endregion

        #region Functions
        public static Models.Page Page(string pageNameOrId, string userToken, string appSecret, string facebookUrl = Utility.FacebookUrl, string facebookVersion = Utility.FacebookVersion)
        {
            var (client, endPoint) = Utility.BuildClient(pageNameOrId, userToken, appSecret, string.Empty, facebookUrl, facebookVersion, string.Empty);
            var response = client.GetAsync(endPoint).Result;
            if (!response.IsSuccessStatusCode) return null;
            var pageTask = response.Content.ReadAsStringAsync();
            var page = JsonConvert.DeserializeObject<Models.Page>(pageTask.Result);
            return page;
        }

        public static Models.Feed Feed(string pageNameOrId, string userToken, string appSecret, Models.Feed.EdgeType edge = Models.Feed.EdgeType.Feed, string facebookUrl = Utility.FacebookUrl, string facebookVersion = Utility.FacebookVersion)
        {
            
            var webMethod = string.Empty;
            var fieldList = string.Empty;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (edge)
            {
                case Models.Feed.EdgeType.Feed:
                    webMethod = "feed";
                    fieldList = ReturnFields;
                    break;
                case Models.Feed.EdgeType.Posts:
                    webMethod = "posts";
                    fieldList = ReturnFields;
                    break;
                case Models.Feed.EdgeType.Tagged:
                    webMethod = "tagged";
                    break;
            }

            var (client, endPoint) = Utility.BuildClient(pageNameOrId, userToken, appSecret, webMethod, facebookUrl, facebookVersion, fieldList);
            var response = client.GetAsync(endPoint).Result;
            if (!response.IsSuccessStatusCode) return null;
            var pageFeedTask = response.Content.ReadAsStringAsync();
            var pageFeed = JsonConvert.DeserializeObject<Models.Feed>(pageFeedTask.Result);
            pageFeed.Edge = edge;
            return pageFeed;
        }
        #endregion
    }
}
