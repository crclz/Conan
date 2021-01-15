using Conan.API.Dtos;
using Conan.API.ResponseConvention;
using Conan.Domain;
using Conan.Domain.Models;
using Conan.Dtos;
using Conan.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        public VideosController(IRepository<Video> videoRepository, IAuth auth, Guardian guardian,
            IRepository<VideoView> viewRepository, TheContext context)
        {
            VideoRepository = videoRepository;
            Auth = auth;
            Guardian = guardian;
            ViewRepository = viewRepository;
            Context = context;
        }

        public IRepository<Video> VideoRepository { get; }
        public IAuth Auth { get; }
        public Guardian Guardian { get; }
        public IRepository<VideoView> ViewRepository { get; }
        public TheContext Context { get; }

        [HttpPost]
        public async Task<IdDto> CreateVideo([FromBody] CreateVideoModel model)
        {
            Guardian.RequireAdmin();

            // dedup
            if (model.Deduplicatiion != null)
            {
                var token = await Context.Dedups.SingleAsync(
                    p => p.UserId == Auth.UserId && p.ClientProvidedToken == model.Deduplicatiion.Value);

                // TODO: use exception
                if (token != null)
                    return null;

                token = new DeduplicationToken(Auth.UserId, model.Deduplicatiion.Value);
                await Context.Dedups.SaveAsync(token);
            }

            var video = new Video(model.Title, model.IsTV, model.SeqId, model.Publish, model.BiliPlayId);

            await VideoRepository.SaveAsync(video);
            return video.Id;// implicit
        }

        [HttpGet]
        public async Task<IEnumerable<Video>> GetVideos(string keyword, bool? isTV, int? seqId)
        {
            var videos = await VideoRepository.Query().OrderBy(p => p.Publish).HelperToListAsync();
            return videos;
        }

        [HttpGet("{id}")]
        public async Task<Video> GetVideoById([FromRoute] string id)
        {
            var video = await VideoRepository.ByIdAsync(id);
            return video;
        }

        [HttpPatch("{id}")]
        public async Task PatchVideo([FromRoute] string id, [FromBody] PatchVideoModel model)
        {
            Guardian.RequireAdmin();

            var video = await VideoRepository.ByIdAsync(id);

            if (video == null)
                throw new NotFoundException("Video not found");

            if (model.BiliPlayId != null)
            {
                if (model.BiliPlayId == "_")
                    video.BiliPlayId = null;
                else
                    video.BiliPlayId = model.BiliPlayId;
            }

            if (model.IsTV != null)
                video.IsTV = model.IsTV.Value;

            if (model.Publish != null)
                video.Publish = model.Publish.Value;

            if (model.SeqId != null)
                video.SeqId = model.SeqId.Value;

            if (model.Title != null)
                video.Title = model.Title;

            await VideoRepository.SaveAsync(video);
        }

        [HttpGet("{videoId}/view")]
        public async Task<VideoView> GetView([FromRoute] string videoId)
        {
            Guardian.RequireLogin();

            // video existence
            var video = await VideoRepository.ByIdAsync(videoId);

            if (video == null)
                throw new NotFoundException("Video not found");

            // get view record of current user
            var view = await ViewRepository.SingleAsync(p => p.UserId == Auth.UserId && p.VideoId == videoId);

            return view;
        }

        [HttpPut("{videoId}/view")]
        public async Task<IdDto> SetView([FromRoute] string videoId)
        {
            Guardian.RequireLogin();

            // video existence
            var video = await VideoRepository.ByIdAsync(videoId);

            if (video == null)
                throw new NotFoundException("Video not found");

            // If view exist, update the creation time
            var view = await ViewRepository.SingleAsync(p => p.UserId == Auth.UserId && p.VideoId == videoId);

            if (view != null)
                view.RefreshViewTime();
            else
                view = new VideoView(videoId, Auth.UserId);

            await ViewRepository.SaveAsync(view);

            return view.Id;
        }

        [HttpDelete("{videoId}/view")]
        public async Task<IdDto> UnsetView([FromRoute] string videoId)
        {
            // video existence
            var video = await VideoRepository.ByIdAsync(videoId);

            if (video == null)
                throw new NotFoundException("Video not found");

            var view = await ViewRepository.SingleAsync(p => p.UserId == Auth.UserId && p.VideoId == videoId);

            // If view not exist, return null
            if (view == null)
                return null;

            // remove and return view id
            await ViewRepository.DeleteAsync(view);

            return view.Id;
        }
    }
}
