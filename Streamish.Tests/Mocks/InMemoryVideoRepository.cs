using System;
using System.Collections.Generic;
using System.Linq;
using Streamish.Models;
using Streamish.Repositories;

namespace Streamish.Tests.Mocks
{
    class InMemoryVideoRepository : IVideoRepository
    {
        private readonly List<Video> _data;

        public List<Video> InternalData
        {
            get
            {
                return _data;
            }
        }

        public InMemoryVideoRepository(List<Video> startingData)
        {
            _data = startingData;
        }

        public void Add(Video video)
        {
            var lastVideo = _data.Last();
            video.Id = lastVideo.Id + 1;
            _data.Add(video);
        }

        public void Delete(int id)
        {
            var videoToDelete = _data.FirstOrDefault(p => p.Id == id);
            if (videoToDelete == null)
            {
                return;
            }

            _data.Remove(videoToDelete);
        }

        public List<Video> GetAll()
        {
            return _data;
        }

        public Video GetById(int id)
        {
            return _data.FirstOrDefault(p => p.Id == id);
        }

        public void Update(Video video)
        {
            var currentVideo = _data.FirstOrDefault(p => p.Id == video.Id);
            if (currentVideo == null)
            {
                return;
            }

            currentVideo.Description = video.Description;
            currentVideo.Title = video.Title;
            currentVideo.DateCreated = video.DateCreated;
            currentVideo.Url = video.Url;
            currentVideo.UserProfileId = video.UserProfileId;
        }

        public List<Video> Search(string criterion, bool sortDescending)
        {
            List<Video> videos = new List<Video>();
            foreach (Video video in _data)
            {
                if (video.Title.Contains(criterion) || video.Description.Contains(criterion))
                {
                    videos.Add(video);
                }
            }
            if (sortDescending)
            {
                return videos.OrderByDescending(v => v.DateCreated).ToList();
            }
            else
            {
                return videos.OrderBy(v => v.DateCreated).ToList();
            }
        }

        public List<Video> GetAllWithComments()
        {
            throw new NotImplementedException();
        }

        public Video GetByIdWithComments(int id)
        {
            throw new NotImplementedException();
        }

        public List<Video> Hottest(string date)
        {
            throw new NotImplementedException();
        }
    }
}