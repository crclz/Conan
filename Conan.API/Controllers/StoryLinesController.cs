using Conan.API.Dtos;
using Conan.API.ResponseConvention;
using Conan.Domain;
using Conan.Domain.Models;
using Conan.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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

        [HttpPut("{id}")]
        public async Task<IdDto> PutStoryline([FromRoute] string id, [FromBody] PutStorylineModel model)
        {
            Guardian.RequireAdmin();

            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequestException(BadCode.InvalidModel, "id is null or whitespace");

            var line = await StoryLineRepository.SingleAsync(p => p.Name == model.Name && p.Id != id);
            if (line != null)
                throw new BadRequestException(BadCode.UniqueViolation, "重名");

            line = new StoryLine(id, model.Name, model.Description, model.Videos);
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
