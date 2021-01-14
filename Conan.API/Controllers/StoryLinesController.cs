using Conan.API.Dtos;
using Conan.API.ResponseConvention;
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
        public StoryLinesController(IRepository<StoryLine> storyLineRepository,
            Guardian guardian)
        {
            StoryLineRepository = storyLineRepository;
            Guardian = guardian;
        }

        public IRepository<StoryLine> StoryLineRepository { get; }
        public Guardian Guardian { get; }

        [HttpGet]
        public async Task<IEnumerable<StoryLine>> GetStoryLines()
        {
            var lines = await StoryLineRepository.Query().HelperToListAsync();

            return lines;
        }

        [HttpPost]
        public async Task<IdDto> CreateStoryline([FromBody] CreateStorylineModel model)
        {
            Guardian.RequireAdmin();

            var line = await StoryLineRepository.SingleAsync(p => p.Name == model.Name);
            if (line != null)
                throw new BadRequestException(BadCode.UniqueViolation, "重名");

            line = new StoryLine(model.Name, model.Description, model.Videos);
            await StoryLineRepository.SaveAsync(line);

            return line.Id;
        }

        [HttpPatch("{id}")]
        public async Task PatchStoryline([FromRoute] string id, [FromBody] PatchStorylineModel model)
        {
            Guardian.RequireAdmin();

            var line = await StoryLineRepository.ByIdAsync(id);

            if (line == null)
                throw new NotFoundException("storyline not found");

            if (model.Name != null)
                line.Name = model.Name;

            if (model.Description != null)
                line.Description = model.Description;

            if (model.Videos != null)
                line.Videos = model.Videos.ToList();

            await StoryLineRepository.SaveAsync(line);
        }
    }
}
