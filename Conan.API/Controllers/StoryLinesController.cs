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
    public class StoryLinesController : ControllerBase
    {
        public StoryLinesController(IRepository<StoryLine> storyLineRepository)
        {
            StoryLineRepository = storyLineRepository;
        }

        public IRepository<StoryLine> StoryLineRepository { get; }


        [HttpGet]
        public IEnumerable<StoryLine> GetStoryLines()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IdDto CreateStoryline(CreateStorylineModel model)
        {
            throw new NotImplementedException();
        }

        [HttpPatch("{id}")]
        public void PatchStoryline(PatchStorylineModel model)
        {
            throw new NotImplementedException();
        }
    }
}
