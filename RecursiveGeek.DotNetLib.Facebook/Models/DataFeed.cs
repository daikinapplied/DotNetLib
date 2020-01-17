using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RecursiveGeek.DotNetLib.Facebook.Models
{
    public class DataFeed
    {
        #region Properties
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("attachments")]
        public Attachment Attachments { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("created_time")]
        public DateTime Created { get; set; }

        [JsonProperty("updated_time")]
        public DateTime Updated { get; set; }

        [JsonProperty("admin_creator")]
        public AdminCreator AdminCreator { get; set; }

        [JsonProperty("from")]
        public From From { get; set; }
        #endregion

        #region Methods
        public List<Image> GetMedia()
        {
            if (!HasMedia()) return null;
            var images = new List<Image>();
            foreach (var item in Attachments.Data)
            {
                if (item.Type == "album" && item.Subattachments != null && item.Subattachments.Data != null)
                {
                    foreach (var subitem in item.Subattachments.Data)
                    {
                        BuildMedia(ref images, subitem);
                    }
                }
                else 
                {
                    BuildMedia(ref images, item);
                }
            }
            return images.Count > 0 ? images : null;
        }

        private void BuildMedia(ref List<Image> images, DataAttachment attachment)
        {
            if (attachment != null && attachment.Media != null && attachment.Media.Image != null)
            {
                GetMediaStyle(ref attachment);
                if (attachment.Media.Image.MediaType != Image.MediaStyle.Unknown)
                {
                    images.Add(attachment.Media.Image);
                }
            }
        }

        private bool HasMedia() // Initial test, no guarantees that desireable media has been found
        {
            return Attachments != null && Attachments.Data != null && Attachments.Data.Count > 0;
        }

        private static void GetMediaStyle(ref DataAttachment attachment)
        {
            // attachment.Type values: photo, video_inline, share, cover_photo
            if (attachment.Type == "photo")
            {
                attachment.Media.Image.MediaType = Image.MediaStyle.Image;
                attachment.Media.Image.Url = attachment.Media.Image.Source; // URL and Source are the same
            }
            else if (attachment.Type == "share")
            {
                attachment.Media.Image.MediaType = Image.MediaStyle.Image;
                attachment.Media.Image.Url = attachment.Url; // URL and Source could be different - source is the preview, URL could be landing page
            }
            else if (attachment.Type == "video_inline")
            {
                attachment.Media.Image.MediaType = Image.MediaStyle.Video;
                attachment.Media.Image.Source = attachment.Media.Source; // replace the sample image from Facebook with the actual video
            }
        }
        #endregion
    }
}
