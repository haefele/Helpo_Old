using Helpo.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpo.Services.Questions
{
    public class Question
    {
        public static Question CreateNew(string title, string content, string createdByUserId, List<string> tags, string application)
        {
            Guard.NotNullOrWhiteSpace(title, nameof(title));
            Guard.NotNullOrWhiteSpace(content, nameof(content));
            Guard.NotNullOrWhiteSpace(createdByUserId, nameof(createdByUserId));
            Guard.NotNull(tags, nameof(tags));
            Guard.NotNullOrWhiteSpace(application, nameof(application));

            return new Question
            {
                Id = IdHelper.GenerateNewId(),
                
                Title = title,
                Content = content,
                Tags = tags.ToList(),
                Application = application,

                CreatedByUserId = createdByUserId,
                CreatedAt = DateTimeOffset.Now,
            };
        }

        // For serialization purposes only
        private Question()
        {
            this.Id = default!;

            this.Title = default!;
            this.Content = default!;
            this.Tags = new List<string>();
            this.Application = default!;

            this.CreatedByUserId = default!;
            this.CreatedAt = default!;
        }

        public string Id { get; set; }
        
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }
        public string Application { get; set; }

        public string? AnswerId { get; set; }

        public string CreatedByUserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
