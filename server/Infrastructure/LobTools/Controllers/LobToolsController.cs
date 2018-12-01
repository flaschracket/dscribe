using Brainvest.Dscribe.Abstractions;
using Brainvest.Dscribe.LobTools.Entities;
using Brainvest.Dscribe.LobTools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Brainvest.Dscribe.LobTools.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class LobToolsController : ControllerBase
	{
		private readonly IImplementationsContainer _implementationsContainer;
		private readonly IUsersService _usersService;

		public LobToolsController(IImplementationsContainer implementationsContainer, IUsersService usersService)
		{
			_implementationsContainer = implementationsContainer;
			_usersService = usersService;
		}

		public async Task<ActionResult<LobSummaryResponse>> GetSummary(LobSummaryRequest request)
		{
			var entityTypeId = _implementationsContainer.Metadata[request.EntityTypeName].EntityTypeId;
			using (var dbContext = _implementationsContainer.LobToolsRepositoryFactory() as LobToolsDbContext)
			{
				var commentCountTask = dbContext.Comments
					.Where(x => x.EntityTypeId == entityTypeId && request.Identifiers.Contains(x.Identifier))
					.GroupBy(x => x.Identifier)
					.ToDictionaryAsync(g => g.Key, g => g.Count());

				var attachmentCountTask = dbContext.Attachments
					.Where(x => x.EntityTypeId == entityTypeId && request.Identifiers.Contains(x.Identifier))
					.GroupBy(x => x.Identifier)
					.ToDictionaryAsync(g => g.Key, g => g.Count());

				return new LobSummaryResponse
				{
					EntityTypeName = request.EntityTypeName,
					CommentCounts = await commentCountTask,
					AttachmentCounts = await attachmentCountTask
				};
			}
		}
	}
}