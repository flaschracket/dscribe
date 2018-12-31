using Brainvest.Dscribe.Abstractions.Models.AppManagement;
using Brainvest.Dscribe.Abstractions.Models.ManageMetadata;
using Brainvest.Dscribe.LobTools.Entities;
using Brainvest.Dscribe.LobTools.Models;
using Brainvest.Dscribe.LobTools.Models.History;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainvest.Dscribe.LobTools.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class RequestLogController : ControllerBase
	{
		public LobToolsDbContext _dbContext;
		public RequestLogController(LobToolsDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task<IEnumerable<EntityTypeHistoryModel>> GetEntityTypeHistory(EntityTypeHistoryModel model)
		{
			var logs = await _dbContext.RequestLogs.Where(x => x.EntityTypeId == model.EntityType.Id && x.Failed == false).OrderByDescending(x => x.Id).ToListAsync();

			var result = logs.Select(x => new EntityTypeHistoryModel
			{
				EntityType = JsonConvert.DeserializeObject<EntityTypeModel>(x.Body),
				LogId = x.Id,
				ProcessDuration = x.ProcessDuration,
				StartTime = x.StartTime,
				UserId = x.UserId,
			}).ToList();
			return result;
		}
		public async Task<IEnumerable<EntityTypeHistoryModel>> GetDeletedEntityTypeHistory(EntityTypeHistoryModel model)
		{
			// TODO. THIS ACTION MAY BE SLOW WHEN REQUEST LOG TABLE WOULD BE LARGE	
			var logs = await _dbContext.RequestLogs
				.Where(x =>
				x.EntityTypeId != null &&
				x.Failed == false &&
				PathChecker(x.Body, "deleteEntityType")).ToListAsync();

			if (!logs.Any())
			{
				return new List<EntityTypeHistoryModel>();
			}

			return logs.Select(x => new EntityTypeHistoryModel
			{
				EntityType = JsonConvert.DeserializeObject<EntityTypeModel>(x.Body),
				LogId = x.Id,
				ProcessDuration = x.ProcessDuration,
				StartTime = x.StartTime,
				UserId = x.UserId,
			}).ToList();
		}
		public async Task<IEnumerable<PropertyHistoryModel>> GetPropertyHistory(PropertyHistoryModel model)
		{
			var logs = await _dbContext.RequestLogs.Where(x => x.PropertyId == model.Property.Id && x.Failed == false).OrderByDescending(x => x.Id).ToListAsync();
			var result = logs.Select(x => new PropertyHistoryModel
			{
				Property = JsonConvert.DeserializeObject<PropertyModel>(x.Body),
				LogId = x.Id,
				StartTime = x.StartTime,
				UserId = x.UserId
			}).ToList();
			return result;
		}
		public async Task<IEnumerable<PropertyHistoryModel>> GetDeletedPropertyHistory(PropertyHistoryModel model)
		{
			var logs = await _dbContext.RequestLogs
				.Where(x =>
				x.PropertyId != null &&
				x.Failed == false &&
				PathChecker(x.Body, "DeleteProperty")).ToListAsync();

			if (!logs.Any())
			{
				return new List<PropertyHistoryModel>();
			}

			return logs.Select(x => new PropertyHistoryModel
			{
				Property = JsonConvert.DeserializeObject<PropertyModel>(x.Body),
				LogId = x.Id,
				StartTime = x.StartTime,
				UserId = x.UserId
			}).ToList();
		}
		public async Task<IEnumerable<AppInstanceHistoryModel>> GetAppInstanceHistory(AppInstanceHistoryModel model)
		{
			var logs = await _dbContext.RequestLogs.Where(x => x.AppInstanceId == model.AppInstance.Id && x.Failed == false).OrderByDescending(x => x.Id).ToListAsync();
			var result = logs.Select(x => new AppInstanceHistoryModel
			{
				AppInstance = JsonConvert.DeserializeObject<AppInstanceInfoModel>(x.Body),
				UserId = x.UserId,
				StartTime = x.StartTime,
				ProcessDuration = x.ProcessDuration,
				LogId = x.Id,
			}).ToList();
			return result;
		}
		public async Task<IEnumerable<AppInstanceHistoryModel>> GetDeletedAppInstanceHistory(AppInstanceHistoryModel model)
		{
			// TODO. THIS ACTION MAY BE SLOW WHEN REQUEST LOG TABLE WOULD BE LARGE	
			var logs = await _dbContext.RequestLogs
				.Where(x =>
				x.AppInstanceId != null &&
				x.Failed == false &&
				PathChecker(x.Body, "DeleteAppInstance")).ToListAsync();

			if (!logs.Any())
			{
				return new List<AppInstanceHistoryModel>();
			}

			return logs.Select(x => new AppInstanceHistoryModel
			{
				AppInstance = JsonConvert.DeserializeObject<AppInstanceInfoModel>(x.Body),
				UserId = x.UserId,
				StartTime = x.StartTime,
				ProcessDuration = x.ProcessDuration,
				LogId = x.Id,
			}).ToList();
		}
		public async Task<IEnumerable<AppTypeHistoryModel>> GetAppTypeHistory(AppTypeHistoryModel model)
		{
			var logs = await _dbContext.RequestLogs.Where(x => x.AppTypeId == model.AppType.Id && x.Failed == false).OrderByDescending(x => x.Id).ToListAsync();
			var result = logs.Select(x => new AppTypeHistoryModel
			{
				AppType = JsonConvert.DeserializeObject<AppTypeModel>(x.Body),
				LogId = x.Id,
				ProcessDuration = x.ProcessDuration,
				StartTime = x.StartTime,
				UserId = x.UserId
			}).ToList();
			return result;
		}
		public async Task<IEnumerable<AppTypeHistoryModel>> GetDeletedAppTypeHistory(AppTypeHistoryModel model)
		{
			// TODO. THIS ACTION MAY BE SLOW WHEN REQUEST LOG TABLE WOULD BE LARGE	
			var logs = await _dbContext.RequestLogs
				.Where(x =>
				x.AppTypeId != null &&
				x.Failed == false &&
				PathChecker(x.Body, "DeleteAppType")).ToListAsync();

			if (!logs.Any())
			{
				return new List<AppTypeHistoryModel>();
			}

			var result = logs.Select(x => new AppTypeHistoryModel
			{
				AppType = JsonConvert.DeserializeObject<AppTypeModel>(x.Body),
				LogId = x.Id,
				ProcessDuration = x.ProcessDuration,
				StartTime = x.StartTime,
				UserId = x.UserId
			}).ToList();
			return result;
		}
		private bool PathChecker(string body, string action)
		{
			var requestAction = body.Split('/').Last();
			return requestAction == action;
		}
	}
}
