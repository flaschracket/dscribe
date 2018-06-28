using Brainvest.Dscribe.Abstractions;
using Brainvest.Dscribe.Abstractions.Models;
using Brainvest.Dscribe.Abstractions.Models.ReadModels;
using Brainvest.Dscribe.Helpers.FilterNodeConverter;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Brainvest.Dscribe.Implementations.Ef.BusinessDataAccess
{
	public class EFEntityHandler : IEntityHandler
	{
		private IImplementationsContainer _implementationsContainer;
		EFEntityHandlerInternal _handlerInternal;

		public EFEntityHandler(
			IImplementationsContainer implementationsContainer,
			EFEntityHandlerInternal handlerInternal)
		{
			_implementationsContainer = implementationsContainer;
			_handlerInternal = handlerInternal;
		}

		public async Task<int> CountByFilter(EntityListRequest request)
		{
			var entityType = _implementationsContainer.Reflector.GetType(request.EntityTypeName);
			var method = _handlerInternal.GetType().GetMethod(nameof(EFEntityHandlerInternal.CountByFilterInternal), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType);
			var r = this.GetType().GetMethod(nameof(CreateGenericListRequest), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType)
					.Invoke(this, new object[] { request });
			var awaitable = method.Invoke(_handlerInternal, new object[] { r }) as Task<int>;
			return await awaitable;
		}

		public async Task<IEnumerable> GetByFilter(EntityListRequest request)
		{
			var entityType = _implementationsContainer.Reflector.GetType(request.EntityTypeName);
			var method = _handlerInternal.GetType().GetMethod(nameof(EFEntityHandlerInternal.GetByFilterInternal), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType);
			var r = this.GetType().GetMethod(nameof(CreateGenericListRequest), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType)
					.Invoke(this, new object[] { request });
			var awaitable = method.Invoke(_handlerInternal, new object[] { r }) as Task<IEnumerable>;
			return await awaitable;
		}

		public async Task<int?> GetGroupCount(GrouppedListRequest request)
		{
			var entityType = _implementationsContainer.Reflector.GetType(request.EntityTypeName);
			var method = _handlerInternal.GetType().GetMethod(nameof(EFEntityHandlerInternal.CountGroupsInternal), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType);
			var r = this.GetType().GetMethod(nameof(CreateGenericGroupRequest), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType)
					.Invoke(this, new object[] { request });
			var awaitable = method.Invoke(_handlerInternal, new object[] { r }) as Task<int>;
			return await awaitable;
		}

		public async Task<ExpressionValueResponse> GetExpressionValue(ExpressionValueRequest request)
		{
			var entityType = _implementationsContainer.Reflector.GetType(request.EntityTypeName);
			var keyType = _implementationsContainer.Metadata[request.EntityTypeName].GetPrimaryKey().GetDataType().GetClrType();
			var method = _handlerInternal.GetType().GetMethod(nameof(EFEntityHandlerInternal.GetExpressionValueInternal), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType, keyType);
			var awaitable = method.Invoke(_handlerInternal, new object[] { request.Ids, request.Properties }) as Task;
			await awaitable;
			return awaitable.GetType().GetProperty("Result").GetValue(awaitable) as ExpressionValueResponse;
		}

		public async Task<IEnumerable> GetGroupped(GrouppedListRequest request)
		{
			var entityType = _implementationsContainer.Reflector.GetType(request.EntityTypeName);
			var method = _handlerInternal.GetType().GetMethod(nameof(EFEntityHandlerInternal.GetGrouppedInternal), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType);
			var r = this.GetType().GetMethod(nameof(CreateGenericGroupRequest), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType)
					.Invoke(this, new object[] { request });
			var awaitable = method.Invoke(_handlerInternal, new object[] { r }) as Task<IEnumerable>;
			return await awaitable;
		}

		public async Task<IEnumerable<NameResponseItem>> GetIdAndName(IdAndNameRequest request)
		{
			var entityType = _implementationsContainer.Reflector.GetType(request.EntityType);
			var keyType = _implementationsContainer.Metadata[request.EntityType].GetPrimaryKey().GetDataType().GetClrType();
			var method = _handlerInternal.GetType().GetMethod(nameof(EFEntityHandlerInternal.GetIdAndNameInternal), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType, keyType);
			var awaitable = method.Invoke(_handlerInternal, new object[] { request.Ids }) as Task<IEnumerable<NameResponseItem>>;
			return await awaitable;
		}

		public async Task<IEnumerable<NameResponseItem>> GetAutocomplteItems(AutocompleteItemsRequest request)
		{
			var entityType = _implementationsContainer.Reflector.GetType(request.EntityType);
			var keyType = _implementationsContainer.Metadata[request.EntityType].GetPrimaryKey().GetDataType().GetClrType();
			var method = _handlerInternal.GetType().GetMethod(nameof(EFEntityHandlerInternal.GetAutocompleteItemsInternal), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType, keyType);
			var awaitable = method.Invoke(_handlerInternal, new object[] { request.QueryText }) as Task<IEnumerable<NameResponseItem>>;
			return await awaitable;
		}

		public async Task<ActionResult<object>> Edit(ManageEntityRequest request)
		{
			return await CallMethod(request, nameof(EFEntityHandlerInternal.EditInternal));
		}

		public async Task<ActionResult<object>> Add(ManageEntityRequest request)
		{
			return await CallMethod(request, nameof(EFEntityHandlerInternal.AddInternal));
		}

		public async Task<ActionResult<object>> Delete(ManageEntityRequest request)
		{
			return await CallMethod(request, nameof(EFEntityHandlerInternal.DeleteInternal));
		}

		private async Task<ActionResult<object>> CallMethod(ManageEntityRequest request, string internalMethodName)
		{
			var entityType = _implementationsContainer.Reflector.GetType(request.EntityType);
			var method = _handlerInternal.GetType().GetMethod(internalMethodName, BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(entityType);
			object r = CreateGenericObject(request, entityType);
			var awaitable = method.Invoke(_handlerInternal, new object[] { r }) as Task<ActionResult<object>>;
			return await awaitable;
		}

		private EntityListRequest<TEntity> CreateGenericListRequest<TEntity>(EntityListRequest request)
		{
			return new EntityListRequest<TEntity>()
			{
				Order = request.Order,
				StartIndex = request.StartIndex,
				Count = request.Count,
				Filters = request.Filters?.Select(x => FilterNodeConverter.ToExpression(x, _implementationsContainer.Reflector)).Cast<Expression<Func<TEntity, bool>>>().ToArray()
			};
		}

		private GrouppedListRequest<TEntity> CreateGenericGroupRequest<TEntity>(GrouppedListRequest request)
		{
			return new GrouppedListRequest<TEntity>()
			{
				Order = request.Order,
				StartIndex = request.StartIndex,
				Count = request.Count,
				Aggregations = request.Aggregations,
				GroupBy = request.GroupBy,
				Filters = request.Filters?.Select(x => FilterNodeConverter.ToExpression(x, _implementationsContainer.Reflector)).Cast<Expression<Func<TEntity, bool>>>().ToArray()
			};
		}

		private object CreateGenericObject(ManageEntityRequest request, Type entityType)
		{
			var jEntity = request.Entity as JObject;
			var toObjectMethod = typeof(JObject).GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public)
				.Single(x => x.Name == nameof(JObject.ToObject) && x.GetGenericArguments().Length == 1 && x.GetParameters().Length == 0);
			toObjectMethod = toObjectMethod.MakeGenericMethod(entityType);
			var entity = toObjectMethod.Invoke(jEntity, null);
			var r = typeof(ManageEntityRequest<>).MakeGenericType(entityType).GetConstructors().First().Invoke(new object[] { entity });
			return r;
		}
	}
}