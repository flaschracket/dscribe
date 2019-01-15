using Brainvest.Dscribe.Abstractions;
using Brainvest.Dscribe.Abstractions.Models;
using Brainvest.Dscribe.LobTools.Entities;
using Brainvest.Dscribe.LobTools.Models;
using Brainvest.Dscribe.MetadataDbAccess;
using Brainvest.Dscribe.MetadataDbAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainvest.Dscribe.LobTools.DataLog
{
	public class DataLogBusiness : IDataLogImplementation
	{
		private readonly LobToolsDbContext _lobToolsDbContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly MetadataDbContext _metadataDbContext;

		public DataLogBusiness(
			LobToolsDbContext lobToolsDbContext,
			IHttpContextAccessor httpContextAccessor,
			MetadataDbContext metadataDbContext)
		{
			_lobToolsDbContext = lobToolsDbContext;
			_httpContextAccessor = httpContextAccessor;
			_metadataDbContext = metadataDbContext;

		}
		public async Task SaveDataChanges(object businessRepository, string entityTypeName)
		{
			var entityType = await _metadataDbContext.EntityTypes
				.Where(x => x.Name == entityTypeName)
				.Include(x => x.Properties)
				.Include("Properties.GeneralUsageCategory")
				.FirstOrDefaultAsync();

			var dataChanges = (businessRepository as DbContext).ChangeTracker
				.Entries()
				.Where(x => x.State == EntityState.Modified)
				.Select(x => new
				{
					Entity = x.Entity,
					Action = x.State
				})
				.ToList();
			foreach (var dataChange in dataChanges)
			{
				// Note. This history does not support primary kyes which are not int E.g. GUID, String
				var primaryKey = entityType.Properties.Where(x => x.GeneralUsageCategory.Name == "PrimaryKey").FirstOrDefault();

				var data = new Entities.DataLog
				{
					EntityId = entityType.Id,
					// Consider that data always has primary key
					DataId = Convert.ToInt64(dataChange.Entity.GetType().GetProperty(primaryKey.Name).GetValue(dataChange.Entity)),
					Body = JsonConvert.SerializeObject(dataChange.Entity),
					DataRequestAction = (DataRequestAction)dataChange.Action,
					RequestLogId = ((RequestLogModel)_httpContextAccessor.HttpContext.Items["RequestLog"]).Id
				};
				_lobToolsDbContext.DataLogs.Add(data);
			}
			await _lobToolsDbContext.SaveChangesAsync();
		}
	}
}
