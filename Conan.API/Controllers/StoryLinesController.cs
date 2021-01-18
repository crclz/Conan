using Conan.API.Dtos;
using Conan.API.ResponseConvention;
using Conan.Domain;
using Conan.Domain.Models;
using Conan.Dtos;
using Conan.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
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
            Guardian guardian, TheContext context)
        {
            StoryLineRepository = storyLineRepository;
            Guardian = guardian;
            Context = context;
        }

        public IRepository<StoryLine> StoryLineRepository { get; }
        public Guardian Guardian { get; }
        public TheContext Context { get; }

        [HttpGet]
        public async Task<IEnumerable<StoryLine>> GetStoryLines()
        {
            var lines = await StoryLineRepository.Query().OrderBy(p => p.Id).HelperToListAsync();

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

            // check each video id exists
            var matchIds = await Context.Videos.Query()
                .Where(p => model.Videos.Contains(p.Id)).Select(p => p.Id)
                .HelperToListAsync();

            var working = model.Videos.ToHashSet();
            working.ExceptWith(matchIds);

            if (working.Count > 0)
                throw new BadRequestException(BadCode.ReferenceNotFound, $"Video {working.First()} not exist");

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
