using System;
using System.Collections.Generic;
using Daikin.DotNetLib.Facebook;
using Daikin.DotNetLib.Facebook.Models;
using Xunit;

namespace Daikin.DotNetLib.Core.Tests
{
    public class FacebookTests
    {
        #region Fields
        private readonly Config _configuration;
        #endregion

        #region Constructors
        public FacebookTests()
        {
            _configuration = Config.GetConfiguration();
        }
        #endregion

        #region Methods
        [Fact]
        public void TestInstanceCursors()
        {
            var cursors = new Cursors();
            Assert.NotNull(cursors);
        }

        [Fact]
        public void TestInstanceFeed()
        {
            var feed = new Feed();
            Assert.NotNull(feed);
            Assert.Null(feed.Data);
            Assert.Null(feed.Paging);
        }

        [Fact]
        public void TestObjectBuild()
        {
            var currentTime = DateTime.Now;
            var pageFeed = GetPageFeed(currentTime);

            Assert.NotNull(pageFeed.Data);
            Assert.NotNull(pageFeed.Paging);
            Assert.NotEmpty(pageFeed.Data[0].Id);
            Assert.NotEmpty(pageFeed.Data[0].Message);
            Assert.NotEmpty(pageFeed.Paging.Cursors.Before);
            Assert.NotEmpty(pageFeed.Paging.Cursors.After);
            Assert.NotEmpty(pageFeed.Paging.Next);

            Assert.Equal(currentTime, pageFeed.Data[0].Created);
        }

        [Fact]
        public void TestPageFeedJsonSimple()
        {
            const string sample = "{\"data\":[{\"created_time\":\"2018-11-21T16:00:47+0000\",\"message\":\"Message1\",\"id\":\"123\"},{\"created_time\":\"2018-11-19T22:52:16+0000\",\"message\":\"Message2\",\"id\":\"456\"}],\"paging\":{\"cursors\":{\"before\":\"before1\",\"after\":\"after1\"},\"next\":\"next1\"}}";
            var pageFeed = Feed.FromJson(sample);
            Assert.NotNull(pageFeed.Data);
            Assert.NotNull(pageFeed.Paging);
            Assert.NotEmpty(pageFeed.Data[0].Id);
            Assert.NotEmpty(pageFeed.Data[0].Message);
            Assert.NotEmpty(pageFeed.Paging.Cursors.Before);
            Assert.NotEmpty(pageFeed.Paging.Cursors.After);
            Assert.NotEmpty(pageFeed.Paging.Next);
        }

        [Fact]
        public void TestPageFeedJasonComplex()
        {
            const string sample = "{\"data\":[{\"message\":\"Happy Sunday all! It’s a great day for some extra movement! \n-1 minute plank\n-hands and knees for 20 donkey kicks (both legs)\n-20 lateral leg lifts (both legs)\n-1 minute plank \n-25 twisting hip dips \n\nRepeat 1-3 times through depending on how much energy you are looking to build and burn! #aosmn #getyourmoveon\",\"created_time\":\"2019-05-19T15:31:48+0000\",\"updated_time\":\"2019-05-19T15:31:48+0000\",\"from\":{\"name\":\"Art of Strength Minnesota\",\"id\":\"137507582955678\"},\"id\":\"137507582955678_2412982475408166\"},{\"attachments\":{\"data\":[{\"description\":\"Happy Tuesday all! Wishing you a beautiful week. And in the moments that aren’t so beautiful wishing you the strength and wisdom to know that those moments will pass.\n#aosmn #enjoythejourney\",\"media\":{\"image\":{\"height\":720,\"src\":\"https://scontent.xx.fbcdn.net/v/t1.0-9/p720x720/60267162_2404330789606668_6093742128330964992_n.jpg?_nc_cat=111&_nc_ht=scontent.xx&oh=1044d1080d9b94b60bd6498d1818c958&oe=5D68FF1A\",\"width\":720}},\"target\":{\"id\":\"2404330786273335\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.170759339630502/2404330786273335/?type=3\"},\"type\":\"photo\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.170759339630502/2404330786273335/?type=3\"}]},\"message\":\"Happy Tuesday all! Wishing you a beautiful week. And in the moments that aren’t so beautiful wishing you the strength and wisdom to know that those moments will pass.\n#aosmn #enjoythejourney\",\"created_time\":\"2019-05-14T22:00:07+0000\",\"updated_time\":\"2019-05-14T22:00:10+0000\",\"from\":{\"name\":\"Art of Strength Minnesota\",\"id\":\"137507582955678\"},\"id\":\"137507582955678_2404334232939657\"},{\"attachments\":{\"data\":[{\"subattachments\":{\"data\":[{\"media\":{\"image\":{\"height\":720,\"src\":\"https://scontent.xx.fbcdn.net/v/t1.0-9/q92/s720x720/59562142_2389679097738504_5515537743859417088_n.jpg?_nc_cat=108&_nc_ht=scontent.xx&oh=59c9c6d4d50bb28230c5553a7071c2c1&oe=5D526F23\",\"width\":540}},\"target\":{\"id\":\"2389679094405171\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.996528367053591/2389679094405171/?type=3\"},\"type\":\"photo\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.996528367053591/2389679094405171/?type=3\"},{\"media\":{\"image\":{\"height\":720,\"src\":\"https://scontent.xx.fbcdn.net/v/t1.0-9/q92/s720x720/59625437_2389679141071833_6973427971163947008_n.jpg?_nc_cat=110&_nc_ht=scontent.xx&oh=56d4b06d1408eefbec0661b169b8de5b&oe=5D564CE9\",\"width\":540}},\"target\":{\"id\":\"2389679137738500\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.996528367053591/2389679137738500/?type=3\"},\"type\":\"photo\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.996528367053591/2389679137738500/?type=3\"},{\"media\":{\"image\":{\"height\":720,\"src\":\"https://scontent.xx.fbcdn.net/v/t1.0-9/q92/s720x720/59652563_2389679194405161_58282762041294848_n.jpg?_nc_cat=107&_nc_ht=scontent.xx&oh=754f5b157f59f2b8b3c9388ac1e98e3d&oe=5D60294D\",\"width\":540}},\"target\":{\"id\":\"2389679187738495\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.996528367053591/2389679187738495/?type=3\"},\"type\":\"photo\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.996528367053591/2389679187738495/?type=3\"},{\"media\":{\"image\":{\"height\":540,\"src\":\"https://scontent.xx.fbcdn.net/v/t1.0-0/q92/p180x540/59931563_2389679211071826_8616782857769058304_n.jpg?_nc_cat=109&_nc_ht=scontent.xx&oh=8228efb188c77ad82898739c125b74af&oe=5D5B614B\",\"width\":720}},\"target\":{\"id\":\"2389679207738493\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.996528367053591/2389679207738493/?type=3\"},\"type\":\"photo\",\"url\":\"https://www.facebook.com/artofstrengthmn/photos/a.996528367053591/2389679207738493/?type=3\"}]},\"target\":{\"id\":\"2389679671071780\",\"url\":\"https://www.facebook.com/137507582955678/posts/2389679671071780/\"},\"title\":\"Photos from Art of Strength Minnesota's post\",\"type\":\"album\",\"url\":\"https://www.facebook.com/137507582955678/posts/2389679671071780/\"}]},\"message\":\"Thank you to all who came out to participate in the Workout and Win yesterday. Coaches Son and Chrissy put together a great challenge! Congratulations to our individual event winner Sophia, our team event winners Brett and Pat and our Spring Fling winner Eric.\",\"created_time\":\"2019-05-05T15:42:45+0000\",\"updated_time\":\"2019-05-05T23:39:46+0000\",\"from\":{\"name\":\"Art of Strength Minnesota\",\"id\":\"137507582955678\"},\"id\":\"137507582955678_2389679671071780\"},{\"attachments\":{\"data\":[{\"media\":{\"image\":{\"height\":720,\"src\":\"https://external.xx.fbcdn.net/safe_image.php?d=AQD9tk7AfPQifBaA&w=720&h=720&url=https%3A%2F%2Fimgssl.constantcontact.com%2Fletters%2Fimages%2F1101116784221%2FS.gif&cfs=1&sx=0&sy=0&sw=1000&sh=1000&_nc_hash=AQDOjQx4wi9lw1f6\",\"width\":720}},\"target\":{\"url\":\"http://l.facebook.com/l.php?u=http%3A%2F%2Fweb-extract.constantcontact.com%2Fv1%2Fsocial_annotation%3Fpermalink_uri%3D2F8Fc43%26image_url%3Dhttps%253A%252F%252Fmlsvc01-prod.s3.amazonaws.com%252Fcd872ae3201%252F72a37e6d-9e94-49f9-b12e-4e08363f19c9.gif%253Fver%253D1485268103000&h=AT140m_QsVauZIxX2XKf3S6uXJY1jl523UnAy-OH2obkTQqWqzXZrvgxe-nN9KQoIHMUH9yPqftL6x0FRMetZZJ2_iKAfbRIAnoIGJR2NDigmwcAwvbaqSRFhc6Rcg&s=1\"},\"title\":\"Spring is in the air! Check out our latest Spring Announcements and save 10% on a 30-day pass.\",\"type\":\"share\",\"url\":\"http://l.facebook.com/l.php?u=http%3A%2F%2Fweb-extract.constantcontact.com%2Fv1%2Fsocial_annotation%3Fpermalink_uri%3D2F8Fc43%26image_url%3Dhttps%253A%252F%252Fmlsvc01-prod.s3.amazonaws.com%252Fcd872ae3201%252F72a37e6d-9e94-49f9-b12e-4e08363f19c9.gif%253Fver%253D1485268103000&h=AT140m_QsVauZIxX2XKf3S6uXJY1jl523UnAy-OH2obkTQqWqzXZrvgxe-nN9KQoIHMUH9yPqftL6x0FRMetZZJ2_iKAfbRIAnoIGJR2NDigmwcAwvbaqSRFhc6Rcg&s=1\"}]},\"message\":\"Spring is in the air! Check out our latest Spring Announcements and save 10% on a 30-day ...\",\"created_time\":\"2019-03-15T16:38:34+0000\",\"updated_time\":\"2019-03-15T16:38:34+0000\",\"from\":{\"name\":\"Art of Strength Minnesota\",\"id\":\"137507582955678\"},\"admin_creator\":{\"category\":\"Business\",\"link\":\"http://www.constantcontact.com/\",\"name\":\"Social Share by Constant Contact\",\"id\":\"219441374737099\"},\"id\":\"137507582955678_2309950095711405\"}],\"paging\":{\"cursors\":{\"before\":\"Q2c4U1pXNTBYM0YxWlhKNVgzTjBiM0o1WDJsa0R5SXhNemMxTURjMU9ESTVOVFUyTnpnNk56ZA3dNemt3T1RnM05UUXhOek0wT1RJd0R3eGhjR2xmYzNSdmNubGZAhV1FQSURFek56VXdOelU0TWprMU5UWTNPRjh5TkRFeU9UZA3lORGMxTkRBNE1UWTJEd1IwYVcxbEJsemhkdVFC\",\"after\":\"Q2c4U1pXNTBYM0YxWlhKNVgzTjBiM0o1WDJsa0R5UXhNemMxTURjMU9ESTVOVFUyTnpnNkxUVXpNRGMzTkRreU5qYzVOak16TnpBd05EQVBER0ZA3YVY5emRHOXllVjlwWkE4ZA01UTTNOVEEzTlRneU9UVTFOamM0WHpJek1EVXdNRGMyTnpJNE56SXpNVFFQQkhScGJXVUdYSWIwU1FFPQZDZD\"},\"next\":\"https://graph.facebook.com/v3.3/137507582955678/feed?fields=attachments%2Cmessage%2Ccreated_time%2Cupdated_time%2Cfrom%2Cadmin_creator&limit=25&after=Q2c4U1pXNTBYM0YxWlhKNVgzTjBiM0o1WDJsa0R5UXhNemMxTURjMU9ESTVOVFUyTnpnNkxUVXpNRGMzTkRreU5qYzVOak16TnpBd05EQVBER0ZA3YVY5emRHOXllVjlwWkE4ZA01UTTNOVEEzTlRneU9UVTFOamM0WHpJek1EVXdNRGMyTnpJNE56SXpNVFFQQkhScGJXVUdYSWIwU1FFPQZDZD\"}}";
            var pageFeed = Feed.FromJson(sample);
            Assert.True(pageFeed.HasData());
            var images = pageFeed.Data[1].GetMedia();
            Assert.Single(images);
            images = pageFeed.Data[2].GetMedia();
            Assert.Equal(4, images.Count);
        }

        [Fact]
        public void TestFeed()
        {
            var pageFeed = Read.Feed(_configuration.FacebookPageOrId, _configuration.FacebookAccessToken, _configuration.FacebookAppSecret);
            Assert.NotNull(pageFeed);
            Assert.Equal(Feed.EdgeType.Feed, pageFeed.Edge);
            Assert.True(pageFeed.HasData());
        }

        [Fact]
        public void TestPost()
        {
            var pagePosts = Read.Feed(_configuration.FacebookPageOrId, _configuration.FacebookAccessToken, _configuration.FacebookAppSecret, Feed.EdgeType.Posts);
            Assert.NotNull(pagePosts);
            Assert.Equal(Feed.EdgeType.Posts, pagePosts.Edge);
            Assert.NotEmpty(pagePosts.Data);
            var allImages = new List<Image>();
            foreach (var items in pagePosts.Data)
            {
                var images = items.GetMedia();
                if (images != null && images.Count > 0)
                {
                    allImages.AddRange(images);
                }
            }
            Assert.NotEmpty(allImages);
        }

        [Fact]
        public void TestTagged()
        {
            var pagePosts = Read.Feed(_configuration.FacebookPageOrId, _configuration.FacebookAccessToken, _configuration.FacebookAppSecret, Feed.EdgeType.Tagged);
            Assert.NotNull(pagePosts);
            Assert.Equal(Feed.EdgeType.Tagged, pagePosts.Edge);
            Assert.NotEmpty(pagePosts.Data);
        }

        [Fact]
        public void TestBadPageInFeed()
        {
            var pagePosts = Read.Feed("NotARealPagePerToken", _configuration.FacebookAccessToken, _configuration.FacebookAppSecret);
            Assert.Null(pagePosts);
        }

        [Fact]
        public void TestPageInfo()
        {
            var page = Read.Page(_configuration.FacebookPageOrId, _configuration.FacebookAccessToken, _configuration.FacebookAppSecret);
            Assert.NotNull(page);
            Assert.NotEmpty(page.Name);
            Assert.NotEmpty(page.Id);
        }

        [Fact]
        public void TestBadPageInfo()
        {
            var page = Read.Page("NotARealPagePerToken", _configuration.FacebookAccessToken, _configuration.FacebookAppSecret);
            Assert.Null(page);
        }

        [Fact]
        public void TestSecretProof()
        {
            // This is hardcoded test
            var hash = Utility.GetAppSecretProof("userToken", "mySecret");
            Assert.Equal("ba418f9e7d1156186b856d03577b2e9c322a63a3614c7e61b438f917da47bde3", hash);
        }

        [Fact]
        public void TestBuildUrl1()
        {
            var url = Utility.BuildUrl("https://www.github.com/", "/mypath/", "/?=123");
            Assert.Equal("https://www.github.com/mypath/?=123", url);
        }

        [Fact]
        public void TestBuildUrl2()
        {
            var url = Utility.BuildUrl("https://www.github.com", "mypath", "?=123");
            Assert.Equal("https://www.github.com/mypath/?=123", url);
        }

        [Fact]
        public void TestBuildUrl3()
        {
            var url = Utility.BuildUrl("https://www.github.com", "/mypath", "/?=123");
            Assert.Equal("https://www.github.com/mypath/?=123", url);
        }

        [Fact]
        public void TestBuildUrl4()
        {
            var url = Utility.BuildUrl("https://www.github.com/", "mypath/", "?=123");
            Assert.Equal("https://www.github.com/mypath/?=123", url);
        }

        [Fact]
        public void TestBuildUrl5()
        {
            var url = Utility.BuildUrl("mypath/", "myfolder", "?=123");
            Assert.Equal("/mypath/myfolder/?=123", url);
        }

        [Fact]
        public void TestBuildUrl6()
        {
            var url = Utility.BuildUrl(false, true, "mypath/", "myfolder", "?=123");
            Assert.Equal("mypath/myfolder/?=123", url);
        }

        [Fact]
        public void TestBuildUrl7()
        {
            var url = Utility.BuildUrl("https://www.github.com", "mypath");
            Assert.Equal("https://www.github.com/mypath/", url);
        }

        [Fact]
        public void TestBuildUrl8()
        {
            var url = Utility.BuildUrl(false, false, "https://www.github.com", "mypath");
            Assert.Equal("https://www.github.com/mypath", url);
        }
        #endregion

        #region Functions
        protected static Feed GetPageFeed(DateTime currentTime)
        {
            var pageFeed = new Feed
            {
                Data = new List<DataFeed>
                {
                    new DataFeed { Created = currentTime, Id = "123", Message = "This is a message"},
                    new DataFeed { Created = DateTime.Now, Id = "456", Message = "This is another message"}
                },
                Paging = new Paging
                {
                    Cursors = new Cursors { After = "after1", Before = "before1" },
                    Next = "next1"
                }
            };
            return pageFeed;
        }
        #endregion
    }
}
