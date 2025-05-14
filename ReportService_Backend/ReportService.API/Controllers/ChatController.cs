// ChatController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ReportService.Business.Services;
using ReportService.Domain.DTOs;
using ReportService.Domain.Enums;
using System.Collections.Generic;

namespace ReportService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("create-session")]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var session = await _chatService.CreateSessionAsync(request.UserId, request.ReportName);
                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("get-session/{sessionId}")]
        public async Task<IActionResult> GetSession(long sessionId)
        {
            try
            {
                var session = await _chatService.GetSessionAsync(sessionId);
                if (session == null)
                {
                    return NotFound($"Session with ID {sessionId} not found");
                }
                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var chatRequest = new ChatRequest
                {
                    ChatSessionId = request.SessionId,
                    Message = request.Message,
                    UserId = User.Identity?.Name ?? "anonymous"
                };

                var response = await _chatService.ProcessMessageAsync(chatRequest);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request" });
            }
        }

        //[HttpPost("get-session-response")]
        //public async Task<IActionResult> GetSessionResponse([FromBody] GetSessionResponseRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    try
        //    {
        //        var response = await _chatService.GetSessionResponseAsync(request);
        //        return Ok(response);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = "An error occurred while getting the session response" });
        //    }
        //}

        [HttpGet("session/{sessionId}/messages")]
        public async Task<IActionResult> GetSessionMessages(long sessionId)
        {
            try
            {
                var messages = await _chatService.GetSessionMessagesAsync(sessionId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("update-generated-sql")]
        public async Task<IActionResult> UpdateGeneratedSql([FromBody] UpdateGeneratedSqlRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.GeneratedSql))
                return BadRequest("GeneratedSql cannot be empty.");

            var success = await _chatService.UpdateLatestGeneratedSqlAsync(request.SessionId, request.GeneratedSql);

            if (!success)
                return NotFound("No chat message found to update.");

            return Ok("Generated SQL updated successfully.");
        }

        [HttpPost("update-message-or-generated-sql")]
        public async Task<IActionResult> UpdateMessageOrGeneratedSql([FromBody] UpdateGeneratedSqlRequest request)
        {
            if (request.GeneratedSql == null && request.Message == null)
                return BadRequest("At least one of GeneratedSql or Message must be provided.");

            var success = await _chatService.UpdateMessageAndGeneratedSqlAsync(request.SessionId, request.Message, request.GeneratedSql);

            if (!success)
                return NotFound("No chat message found to update.");

            return Ok("Chat message updated successfully.");
        }

        [HttpPost("update-chart")]
        public async Task<IActionResult> UpdateChartConfiguration([FromBody] ChartConfigurationDto config)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedConfig = await _chatService.UpdateChartConfigurationAsync(config);
                return Ok(updatedConfig);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("user-sessions/{userId}")]
        public async Task<ActionResult<IEnumerable<ChatSessionDto>>> GetUserSessions(string userId)
        {
            try
            {
                var sessions = await _chatService.GetUserSessionsAsync(userId);
                return Ok(sessions);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("next-step/{sessionId}")]
        public async Task<IActionResult> GetNextStep(long sessionId)
        {
            var step = await _chatService.GetNextStepAsync(sessionId);
            if (step == null)
                return Ok(new { message = "All steps completed" });

            return Ok(step);
        }

        [HttpPost("update-report-name")]
        public async Task<IActionResult> UpdateReportName([FromBody] UpdateReportNameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ReportName))
                return BadRequest("ReportName cannot be empty.");

            var success = await _chatService.UpdateReportNameAsync(request.SessionId, request.ReportName);

            if (!success)
                return NotFound($"Session with ID {request.SessionId} not found.");

            return Ok("Report name updated successfully.");
        }

        [HttpGet("GetChartTypes")]
        public async Task<IActionResult> GetChartTypes()
        {
            var chartTypes = await _chatService.GetChartTypesAsync();
            return Ok(chartTypes);
        }
    }
}