using Conan.API.Dtos;
using Conan.Domain;
using Conan.Domain.Models;
using Conan.Dtos;
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
        public VideosController(IRepository<Video> videoRepository, IAuth auth)
        {
            VideoRepository = videoRepository;
            Auth = auth;
        }

        public IRepository<Video> VideoRepository { get; }
        public IAuth Auth { get; }


        [HttpPost]
        public IdDto CreateVideo([FromBody] CreateVideoModel model)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public IEnumerable<Video> GetVideos(string keyword, bool? isTV, int? seqId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public Video GetVideoById(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{id}")]
        public Video PatchVideo(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{videoId}/view")]
        public IdDto GetView(string videoId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{videoId}/view")]
        public IdDto SetView(string videoId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{videoId}/view")]
        public IdDto UnsetView(string videoId)
        {
            throw new NotImplementedException();
        }
    }
}
