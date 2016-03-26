using System;
using CWS.Controllers;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace CWS.Controllers
{
    public class LinksRepository
    {
        private static ConcurrentDictionary<string, List<Uri>> storage = new ConcurrentDictionary<string, List<Uri>>();

        public void Add(string userId, Uri link)
        {
            storage.AddOrUpdate(userId, _ => {
                var result = new List<Uri>();
                result.Add(link);
                return result;
            }, (_, links) => {
                links.Add(link);
                return links;
            });
        }

        public IEnumerable<Uri> Get(string userId)
        {
            List<Uri> result;
            if (storage.TryGetValue(userId, out result))
                return result;

            return new Uri[0];
        }
    }
}

