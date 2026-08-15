using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{

    private readonly TaskService _taskService;

    private readonly IValidator<CreateTaskRequest>
        _createValidator;

    private readonly IValidator<UpdateTaskRequest>
        _updateValidator;

    public TasksController(TaskService taskService, IValidator<CreateTaskRequest> createValidator, IValidator<UpdateTaskRequest> updateValidator)
    {
        _taskService = taskService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {

        var validationResult =
            await _createValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(
                validationResult.Errors);
        }


        var result = await _taskService.AddTaskAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _taskService.GetTasksAsync(
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _taskService.GetTaskByIdAsync(
            id,
            cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = $"Task with id {id} was not found."
            });
        }

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _updateValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                errors = validationResult.Errors
                    .Select(x => x.ErrorMessage)
            });
        }



        var result = await _taskService.UpdateTaskAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = $"Task with id {id} was not found."
            });
        }

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _taskService.DeleteTaskAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Task with id {id} was not found."
            });
        }

        return NoContent();
    }
}