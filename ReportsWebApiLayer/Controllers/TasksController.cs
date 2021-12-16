using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ReportsDataAccessLayer.Services.Interfaces;
using ReportsLibrary.Employees;
using ReportsLibrary.Tasks;
using ReportsLibrary.Tasks.TaskChangeCommands;
using ReportsLibrary.Tools;
using ReportsWebApiLayer.DataTransferObjects;

namespace ReportsWebApiLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        private readonly Employee _lipa =
            new ("Misha", "Libchenko", new Guid("11111111-1111-1111-1111-111111111111"), EmployeeRoles.TeamLead);

        public TasksController(ITaskService taskService, IMapper mapper)
        {
            _taskService = taskService;
            _mapper = mapper;
        }

        // GET: api/Tasks
        [HttpGet]
        public Task<ActionResult<List<ReportsTaskDto>>> Get() =>
            Task.FromResult<ActionResult<List<ReportsTaskDto>>>(
                _mapper.Map<List<ReportsTaskDto>>(_taskService.GetTasks().Result));

        // GET: api/Tasks/1
        [HttpGet("{id}", Name = "GetTask")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<FullReportsTaskDto>> Get(Guid id)
        {
            ReportsTask reportsTask = await _taskService.GetTaskById(id);

            if (reportsTask == null) return NotFound();

            return _mapper.Map<FullReportsTaskDto>(reportsTask);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<CreatedAtRouteResult> Create(string taskName)
        {
            ReportsTask reportsTask = await _taskService.CreateTask(taskName, _lipa, _lipa.Id);

            return CreatedAtRoute(
                "GetTask", new { id = reportsTask.Id }, _mapper.Map<ReportsTaskDto>(reportsTask));
        }

        [HttpPut("{taskId}/owner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> SetOwner(
            [Required] Guid taskId,
            [Required] Guid changerId,
            [FromQuery] Guid ownerId)
        {
            var setOwnerCommand = new SetTaskOwnerCommand(ownerId);

            _taskService.UseChangeTaskCommand(taskId, changerId, setOwnerCommand);

            return Task.FromResult<IActionResult>(NoContent());
        }

        [HttpPut("{changerId}/content")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> SetContent(
            [Required] Guid taskId,
            [Required] Guid changerId,
            [FromQuery] string content)
        {
            var setContentCommand = new SetTaskContentCommand(content);

            _taskService.UseChangeTaskCommand(taskId, changerId, setContentCommand);

            return Task.FromResult<IActionResult>(NoContent());
        }

        [HttpPut("{taskId}/comment")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> AddComment(
            [Required] Guid taskId,
            [FromQuery] Guid changerId,
            [FromQuery] string comment)
        {
            var addCommentCommand = new AddTaskCommentCommand(comment);

            _taskService.UseChangeTaskCommand(taskId, changerId, addCommentCommand);

            return Task.FromResult<IActionResult>(NoContent());
        }

        [HttpPut("{taskId}/title")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> SetTitle(
            [Required] Guid taskId,
            [FromQuery] Guid changerId,
            [FromQuery] string title)
        {
            var setTitleCommand = new SetTaskTitleCommand(title);

            _taskService.UseChangeTaskCommand(taskId, changerId, setTitleCommand);

            return Task.FromResult<IActionResult>(NoContent());
        }

        [HttpPut("{taskId}/state")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> SetState(
            [Required] Guid taskId,
            [FromQuery] Guid changerId,
            [FromQuery] TaskStates state)
        {
            var setStateCommand = new SetTaskStateCommand(state);

            _taskService.UseChangeTaskCommand(taskId, changerId, setStateCommand);

            return Task.FromResult<IActionResult>(NoContent());
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid id)
        {
            ReportsTask reportsTask = await _taskService.GetTaskById(id);

            _taskService.RemoveTaskById(reportsTask.Id);

            return NoContent();
        }
    }
}